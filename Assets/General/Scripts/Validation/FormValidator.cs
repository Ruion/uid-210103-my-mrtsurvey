using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Data;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using Sirenix.OdinInspector;
using Mono.Data.Sqlite;
using System.Threading.Tasks;

/// <summary>
/// Validate the user input in Registration Page.
/// Validate Type support : text, email.
/// Notes: By default it get "email" and "contact" list from DBModelEntity to check email, contact duplication
/// </summary>
public class FormValidator : MonoBehaviour
{
    #region variables

    private bool Text1OK = false;

    public Button Submit;

    public int minimumCodeLength;
    public TMP_InputField NameText;
    public GameObject[] Ok_Markers;
    public GameObject[] NotOk_Markers;
    public GameObject duplicateMsg;
    public GameObject InvalidMsg;
    public GameObject verifyingPanel;

    public float validateFrequency;

    public VoucherCodeDBModelEntity voucherCodeDBModelEntity;

    public UnityEvent onValidateStart;
    public UnityEvent onOnlineValidateStart;
    public UnityEvent onValidateEnd;
    public UnityEvent onValidateSuccess;

    private string suspendRedeemCode;
    private string suspendVoucherStatus;

    public ExecutableLauncher executableLauncher;
    public NetworkDatabaseModel networkDBModel;
    private string networkDataStatus;

    #endregion variables

    public void Validate()
    {
        StartCoroutine(ValidateRoutine());
    }

    private IEnumerator ValidateRoutine()
    {
        yield return new WaitForSeconds(.1f);

        onValidateStart.Invoke();

        T1Change();
    }

    public void T1Change()
    {
        Text1OK = InputNotEmpty(NameText);

        if (!Text1OK) return;

        if (FindObjectOfType<GameSettingEntity>().gameSettings.realTimeOnlineValidate && NetworkExtension.CheckForInternetConnection())
            RealtimeValidate();
        else ValidateDuplicate();
    }

    private string CheckExistence()
    {
        if (NameText.text.Length >= minimumCodeLength)
        {
            if (VerifyRedeemCodeFromDB())
            {
                //StartCoroutine(ReadNetworkData(NameText.text));
                return "exist";
            }
            else
            {
                Text1OK = false;
                Debug.Log("Incorrect code");
                return "please type in correct voucher code";
            }
        }
        else { Text1OK = false; return "card number incomplete"; }
    }

    private async Task<string> CheckExistenceAsync()
    {
        if (NameText.text.Length >= minimumCodeLength)
        {
            if (VerifyRedeemCodeFromDB())
            {
                //StartCoroutine(ReadNetworkData(NameText.text));
                await ReadNetworkData(NameText.text);
                return "exist";
            }
            else
            {
                Text1OK = false;
                Debug.Log("Incorrect code");
                return "please type in correct voucher code";
            }
        }
        else { Text1OK = false; return "card number incomplete"; }
    }

    private async void ValidateDuplicate()
    {
        if (CheckExistence() == "exist")
        //if (await CheckExistenceAsync() == "exist")
        {
            //if (suspendVoucherStatus == "redeemed" || networkDataStatus == "redeemed")
            if (suspendVoucherStatus == "redeemed")
            {
                Text1OK = false;
                duplicateMsg.SetActive(true);
            }
            else
            {
                // Voucher code pass
                Text1OK = true;
                InvalidMsg.SetActive(true);
                onValidateSuccess.Invoke();

                // Launch the app to sync used code
                executableLauncher.arguments = NameText.text.Trim();
                executableLauncher.LaunchExecutable();
            }
        }
        if (!Text1OK) { ChangeHint(0, false); }
        else { ChangeHint(0, true); }

        onValidateEnd.Invoke();
    }

    private bool VerifyRedeemCodeFromDB()
    {
        SqliteDataReader reader = voucherCodeDBModelEntity.ExecuteCustomSelectSingle
        (string.Format("SELECT redeem_code,voucher_status FROM {0} WHERE redeem_code ='{1}'",
        new System.Object[] { voucherCodeDBModelEntity.dbSettings.tableName, NameText.text }));

        //Debug.Log(reader["redeem_code"].ToString());
        suspendRedeemCode = reader["redeem_code"].ToString();
        suspendVoucherStatus = reader["voucher_status"].ToString();

        voucherCodeDBModelEntity.Close();
        voucherCodeDBModelEntity.sqlitedb_connection.Close();

        bool valid = string.IsNullOrEmpty(suspendRedeemCode);

        return !valid;
    }

    public void RealtimeValidate()
    {
        if (NameText.text.Length >= minimumCodeLength)
            StartCoroutine(RealtimeChecking());
        else
        {
            Text1OK = false;
            if (!Text1OK) { ChangeHint(0, false); }
            else { ChangeHint(0, true); }
            onValidateEnd.Invoke();
        }
    }

    private IEnumerator RealtimeChecking()
    {
        onValidateStart.Invoke();
        onOnlineValidateStart.Invoke();

        var time = System.Diagnostics.Stopwatch.StartNew();

        WWWForm form = new WWWForm();
        form.AddField("redeem_code", NameText.text);

        Debug.Log(voucherCodeDBModelEntity.dbSettings.sendURL + "check-duplicate-redeem-code");
        using (UnityWebRequest www = UnityWebRequest.Post(voucherCodeDBModelEntity.dbSettings.sendURL + "check-duplicate-redeem-code", form))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(name + "\n" + www.error + "\n" + NameText.text);
                voucherCodeDBModelEntity.isFetchingData = false;
                yield break;
            }
            else
            {
                while (!www.downloadHandler.isDone) yield return null;
                JSONResponse response = JsonUtility.FromJson<JSONResponse>(www.downloadHandler.text);

                string responseString = response.result;

                if (CheckExistence() == "exist")
                {
                    if (responseString == "New")
                    {
                        Text1OK = true;
                        onValidateSuccess.Invoke();
                    }
                    else if (responseString == "Duplicate")
                    {
                        Text1OK = false;
                        duplicateMsg.SetActive(true);
                    }
                    else if (responseString == "Invalid Request")
                    {
                        Text1OK = false;
                        InvalidMsg.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("empty response");
                    }

                    Debug.Log(responseString);
                }
            }
        }

        time.Stop();
        Debug.Log(name + " - Real time online validate time : " + time.Elapsed.TotalMilliseconds);

        if (Text1OK)
        {
            Submit.interactable = true;
        }
        else
        {
            Submit.interactable = false;
        }

        if (!Text1OK) { ChangeHint(0, false); }
        else { ChangeHint(0, true); }

        onValidateEnd.Invoke();
    }

    private bool InputNotEmpty(TMP_InputField text)
    {
        bool notEmpty = true;

        if (text.text == "" || text.text == null) notEmpty = false;

        return notEmpty;
    }

    private void ChangeHint(int InputIndex, bool isPass = false)
    {
        Ok_Markers[InputIndex].SetActive(isPass);
        NotOk_Markers[InputIndex].SetActive(!isPass);
    }

    private async Task ReadNetworkData(string filter)
    {
        verifyingPanel.SetActive(true);
        await Task.Delay(100);
        if (await networkDBModel.PingHost())
        {

            networkDataStatus = await networkDBModel.ReadData(filter);
        }
        //await Task.Run(()=>networkDBModel.ReadData(filter, networkDataStatus));
        //networkDataStatus = ;
        verifyingPanel.SetActive(false);
    }
}