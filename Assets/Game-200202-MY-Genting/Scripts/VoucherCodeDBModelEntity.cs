using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using System.IO;
using System.Data;

public class VoucherCodeDBModelEntity : DBModelMaster
{
    public List<string> onlineVoucherCodeList { get { return serverEmailList; } set { serverEmailList = new List<string>(); } }

    [FilePath(AbsolutePath = true)]
    public string localTxt;

    public UnityEvent onSyncFinish;

    

    public void UpdateRecord()
    {
        // voucher_status : ready, redeemed
        // ready : can redeemed
        // redeemed : already redeemed, no more redeem

        // Update the voucher_status from ready to "redeemed" when the redeem_code is printed out
        ExecuteCustomNonQuery(
            string.Format("UPDATE {0} SET voucher_status = '{1}' WHERE redeem_code = '{2}'",
                          new System.Object[] { dbSettings.tableName, "redeemed", PlayerPrefs.GetString("redeem_code") }));

        DataRowCollection drc = ExecuteCustomSelectQuery(string.Format("SELECT voucher_category FROM {0} WHERE redeem_code = '{1}'",
            new System.Object[] { dbSettings.tableName, PlayerPrefs.GetString("redeem_code") }));
        PlayerPrefs.SetString("voucher_category", drc[0][0].ToString());

    }

    [Button(ButtonSizes.Medium)]
    public void SyncOnlineVoucherCode()
    {
        StartCoroutine(SyncOnlineVoucherRoutine());
    }

    public IEnumerator SyncOnlineVoucherRoutine()
    {
        // yield return StartCoroutine(GetDataFromServer());
        yield return StartCoroutine(GetDataFromTxtText());

        // if (NetworkExtension.internet == false) yield break;

        //var time = System.Diagnostics.Stopwatch.StartNew();

        List<string> readyCodeList = new List<string>();

        // Select the voucher_code that not yet redeem in this device
        System.Data.DataRowCollection drc =
        ExecuteCustomSelectQuery(string.Format("SELECT redeem_code FROM {0} WHERE voucher_status = 'ready'"
        , dbSettings.tableName));

        // add the not yet redeem results into list
        for (int d = 0; d < drc.Count; d++)
        {
            readyCodeList.Add(drc[d]["redeem_code"].ToString());
        }

        // find the intersect record between server redeemed list and local not redeem list
        string[] serverRedeemedList = readyCodeList.Intersect(serverEmailList).ToArray();

        // clear the list
        serverEmailList = new List<string>();

        for (int n = 0; n < serverRedeemedList.Length; n++)
        {
            // update the voucher_status in local .sqlite to "redeemed"
            ExecuteCustomNonQuery(
                string.Format("UPDATE {0} SET voucher_status = '{1}' WHERE redeem_code = '{2}'",
                new System.Object[] { dbSettings.tableName, "redeemed", serverRedeemedList[n] }));
        }

        // stop timer and log the time taken
        //time.Stop();
        //Debug.Log(name + " - SyncOnlineVoucherRoutine() update redeem_code time : " + time.Elapsed.TotalSeconds);

        if (onSyncFinish.GetPersistentEventCount() > 0) onSyncFinish.Invoke();
    }

    private IEnumerator GetDataFromTxtText()
    {
        //var time = System.Diagnostics.Stopwatch.StartNew();

        if (!dbSettings.hasMultipleLocalDB) yield break;
        isFetchingData = true;

        // dbSettings.SetUpTextPath();
        dbSettings.SetUpTextPath();
        serverEmailList = new List<string>();

        /*
        string HtmlText = GetHtmlFromUri();
        if (HtmlText == "")
        {
            //No internet connection, stop this Coroutine
            Debug.LogError("no internet connection");
            isFetchingData = false;
            yield break;
        }
        */

        // if (!NetworkExtension.CheckForInternetConnection()) yield break;

        using (UnityWebRequest www = UnityWebRequest.Get(localTxt))
        {
            www.timeout = FindObjectOfType<GameSettingEntity>().gameSettings.downloadCodeAPITimeOut;
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(name + "\n" + www.error + "\n" + localTxt);
                isFetchingData = false;
                //time.Stop();
                //Debug.Log(name + " - GetDataFromTxtText() FAILED download codes time : " + time.Elapsed.TotalMilliseconds, gameObject);
                Debug.Log(name + " - GetDataFromTxtText() FAILED download codes", gameObject);
                yield break;
            }
            else
            {
                while (!www.downloadHandler.isDone) yield return null;

                string texts = www.downloadHandler.text;
                //  Debug.Log("raw server key : " + texts);

                //time.Stop();
                //Debug.Log(name + " - GetDataFromTxtText() download codes time : " + time.Elapsed.TotalMilliseconds);

                // clear text file
                //File.WriteAllText(dbSettings.serverEmailFilePath, "");

                // write email list to file
                //StreamWriter writer = new StreamWriter(dbSettings.serverEmailFilePath, true); //open txt file (doesnt actually open it inside the game)
                //writer.Write(texts); //write into txt file the string declared above
                //writer.Close();

                List<string> lines = new List<string>(
                 texts
                 .Split(new string[] { "\r", "\n" },
                 System.StringSplitOptions.RemoveEmptyEntries));

                lines = lines
                    .Where(line => !(line.StartsWith("//")
                                    || line.StartsWith("#")))
                    .ToList();

                // add emails to list
                foreach (string line in lines)
                {
                    serverEmailList.Add(line.ToString());
                }
            }
        }

        isFetchingData = false;
        Debug.Log($"{name} - Sync online voucher codes at {System.DateTime.Now}");
    }
}