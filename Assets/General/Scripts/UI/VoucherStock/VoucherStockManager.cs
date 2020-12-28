using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Data;
using TMPro;
using System.Linq;

/// <summary>
/// Manage the voucher stock, set and save the daily UI distribution
/// </summary>
public class VoucherStockManager : MonoBehaviour
{

    /// <summary>
    /// Prefab of voucher setting inputfield
    /// </summary>
    public GameObject voucherSettingField_ { get { return voucherSettingField; } set { voucherSettingField = value; } }
    private GameObject voucherSettingField;

    private Transform scrollViewParentTransform;

    public KeyboardScript ks;
    public VoucherDBModelEntity vdb;
    [ReadOnly] [SerializeField] private List<TMP_InputField> fields;

    public Button setButton;
    public Button saveButton;

    public TMP_InputField voucherNameField;
    public TMP_InputField voucherDailyQuantityField;

    private void OnEnable()
    {
        if (vdb == null) vdb = FindObjectOfType<VoucherDBModelEntity>();
    }

    [ContextMenu("Reload UI")]
    public void ReloadUI(Transform scrollViewParent)
    {
        scrollViewParentTransform = scrollViewParent;

        #region Clear fields
        if (scrollViewParent.childCount > 0)
        {
            GameObject[] allChildren = new GameObject[scrollViewParent.childCount];

            for (int i = 0; i < scrollViewParent.childCount; i++)
            {
                allChildren[i] = scrollViewParent.GetChild(i).gameObject;
            }

            foreach (GameObject child in allChildren)
            {
                DestroyImmediate(child);
            }

            if (scrollViewParent.childCount > 0)
            {
                foreach (Transform c in scrollViewParent)
                {
                    DestroyImmediate(c.gameObject);
                }
            }
        }
        #endregion

        #region Generate Fields
        // get voucher list
        DataRowCollection drc = vdb.ExecuteCustomSelectQuery("SELECT * FROM " + vdb.dbSettings.tableName);

        fields = new List<TMP_InputField>();

        // spawn field based on items in voucher list 
        for (int r = 0; r < drc.Count; r++)
        {
            GameObject newField = Instantiate(voucherSettingField, scrollViewParent);
            newField.GetComponent<TextMeshProUGUI>().text = (r + 1).ToString();

            // assign keyboard event to InputField
            Transform inputField = newField.GetComponentInChildren<TMP_InputField>().transform;
            TMP_InputField field = inputField.GetComponent<TMP_InputField>();
            field.text = drc[r]["daily_quantity"].ToString();
            field.onSelect.AddListener(delegate { ToggleKeyboard(field); });
            field.onValueChanged.AddListener(delegate { ValidateFields(); });
            newField.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = field.GetComponent<PlayerPrefsSaver>().name_ = drc[r]["voucher_code"].ToString();
            fields.Add(field);

            // assign remaining to Daily Quantity
            Transform dailyQuantity = newField.transform.Find("DailyQuantity");
            if (dailyQuantity != null)
            {
                TextMeshProUGUI remainText = dailyQuantity.GetComponent<TextMeshProUGUI>();
                remainText.text = drc[r]["daily_quantity"].ToString();
                field.text = drc[r]["daily_quantity"].ToString();
            }


            // assign remaining to Current Remaining
            Transform currentRemain = newField.transform.Find("CurrentRemaining");
            if (currentRemain != null)
            {
                TextMeshProUGUI remainText = currentRemain.GetComponent<TextMeshProUGUI>();
                remainText.text = drc[r]["quantity"].ToString();
                field.text = drc[r]["quantity"].ToString();
            }

        }

        ValidateFields();
        #endregion
    }

    public void SetVoucherStockQuantity()
    {
        // vdb.LoadSetting();

        for (int i = 0; i < fields.Count; i++)
        {
            string voucherName = fields[i].GetComponent<PlayerPrefsSaver>().name_;
            PlayerPrefs.SetInt(voucherName, int.Parse(fields[i].text));
            // UPDATE voucher quantity
            vdb.ExecuteCustomNonQuery("UPDATE " + vdb.dbSettings.tableName + " SET daily_quantity = " + PlayerPrefs.GetInt(voucherName) + " WHERE voucher_code = '" + voucherName + "'");

        }
    }

    public void ResetVoucherStockQuantity()
    {
        // vdb.LoadSetting();

        vdb.vouchersQuantity = new int[fields.Count];
        for (int q = 0; q < fields.Count; q++)
        {
            string voucherName = fields[q].GetComponent<PlayerPrefsSaver>().name_;
            vdb.vouchersQuantity[q] = int.Parse(fields[q].text);
            vdb.ExecuteCustomNonQuery("UPDATE " + vdb.dbSettings.tableName + " SET quantity = " + vdb.vouchersQuantity[q] + " WHERE voucher_code = '" + voucherName + "'");
        }

    }

    public void AddVoucher()
    {
        List<string> col = new List<string>();
        List<string> val = new List<string>();

        col.Add("voucher_code"); val.Add(voucherNameField.text);
        col.Add("daily_quantity"); val.Add(voucherDailyQuantityField.text);
        col.Add("quantity"); val.Add(voucherDailyQuantityField.text);
        vdb.AddData(col, val);

        ReloadUI(scrollViewParentTransform);
    }

    void ToggleKeyboard(TMP_InputField _field)
    {
        ks.inputFieldTMPro_ = _field;
        ks.ShowLayout(ks.DigitLayout);
    }

    /// <summary>
    /// Validate the input field's text is empty or not. 
    /// Enable button click when no empty text in input field,
    /// disable button click when there is empty text in input field
    /// </summary>
    public void ValidateFields()
    {
        TMP_InputField field_ = fields.Where(i => string.IsNullOrEmpty(i.text)).FirstOrDefault();

        if (field_ != null) { saveButton.interactable = setButton.interactable = false; return; }

        saveButton.interactable = setButton.interactable = true;

    }

    public void ValidateAddVoucherFields(Button addBtn)
    {
        if (string.IsNullOrEmpty(voucherNameField.text) || string.IsNullOrEmpty(voucherDailyQuantityField.text))
        {
            addBtn.interactable = false;
        }
        else addBtn.interactable = true;
    }

}
