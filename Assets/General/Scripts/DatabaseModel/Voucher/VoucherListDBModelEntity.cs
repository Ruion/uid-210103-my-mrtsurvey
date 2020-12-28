using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Data;
using System;
using System.IO;
using Sirenix.OdinInspector;

public class VoucherListDBModelEntity : DBModelMaster
{
    #region Fields
    DataRowCollection rows;
    int voucher_id;
    string voucher_name;
    string voucher_code;
    [SerializeField] private int populate_quantity;

    public UnityEvent OnVoucherPrint;

    public string[] vouchersName;
    public int[] vouchersQuantity;

    [DisableContextMenu]
    public VoucherEntity ChosenVoucherEntity;

    #endregion

    private void Start()
    { }

    #region Setup
    protected override void OnEnable()
    {
        // reset the voucher quantity to daily_quantity everyday

        // create the directory if the directory not existed
        Directory.CreateDirectory(Path.GetDirectoryName(dbSettings.folderPath + "\\Vouchers\\"));
        DirectoryInfo di = new DirectoryInfo(dbSettings.folderPath + "\\Vouchers\\");

        // Check there is printer exe in the folder, prompt error if not
        if (di.GetFiles("*.exe*").Length < 0)
            Debug.LogError(String.Format("No Printer exe program found. Please put Printer.exe into {0}\\Vouchers\\",
             dbSettings.folderPath));

        // Check there is pdf file in the folder, prompt error if not
        if (di.GetFiles("*.pdf*").Length < 0)
            Debug.LogError(String.Format("No voucher pdf found. Please put voucher design pdf into {0} \\Vouchers folder", dbSettings.folderPath));
    }

    protected override void Populate()
    {
        SaveSetting();
        CreateTable();

        
        List<string> col = new List<string>();
        List<string> val = new List<string>();

        for (int v = 1; v < dbSettings.columns.Count; v++)
        {
            col.Add(dbSettings.columns[v].name);
        }

        val.AddRange(col);

        for (int n = 0; n < vouchersName.Length; n++)
        {
            string name = vouchersName[n];
            string quantity = vouchersQuantity[n].ToString();
            string dateCreated = System.DateTime.Now.ToString("yyyy - MM - dd HH:mm:ss");

            val[0] = name;
            val[1] = quantity;
            val[2] = quantity;
            val[val.Count - 1] = dateCreated;

            // add the data into Database
            AddData(col, val);
        }

        TestIndex++;

        Close();
    }

    #endregion

    [ContextMenu("PrintVoucher")]
    public void PrintVoucher()
    {
        #region Get Voucher
        LoadSetting();
        #endregion

        // Invoke OnVoucherPrint event when printing voucher
        if (OnVoucherPrint.GetPersistentEventCount() > 0) OnVoucherPrint.Invoke();

    }
}
