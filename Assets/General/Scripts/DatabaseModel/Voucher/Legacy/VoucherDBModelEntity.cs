using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class VoucherDBModelEntity : DBModelMaster
{
    #region Fields

    private DataRowCollection rows;
    [ReadOnly] private int voucher_id;
    [ReadOnly] private string voucher_name;
    [ReadOnly] private int voucher_quantity;
    [SerializeField] private int populate_quantity;

    public UnityEvent OnOutOfStock;
    public UnityEvent OnVoucherPrint;

    public string[] vouchersName;
    public int[] vouchersQuantity;

    [DisableContextMenu]
    public VoucherEntity ChosenVoucherEntity;

    public Print_Program printer;
    public DBModelEntity voucherDistributionDBModelEntity;

    #endregion Fields

    #region SetUp

    protected override void OnEnable()
    {
        // reset the voucher quantity to daily_quantity everyday
        CheckDay();

        // create the directory if the directory not existed
        Directory.CreateDirectory(Path.GetDirectoryName(dbSettings.folderPath + "\\Vouchers\\"));
        DirectoryInfo di = new DirectoryInfo(dbSettings.folderPath + "\\Vouchers\\");

        // Check there is printer exe in the folder, prompt error if not
        if (di.GetFiles("*.exe*").Length < 0)
            Debug.LogError(String.Format("No Printer exe program found. Please put Printer.exe into {0}\\Vouchers\\",
             dbSettings.folderPath));

        if (di.GetFiles("*.pdf*").Length < 0)
            Debug.LogError(String.Format("No voucher pdf found. Please put voucher design pdf into {0} \\Vouchers folder", dbSettings.folderPath));
    }

    // use to generate data in editor
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

            AddData(col, val);
        }

        TestIndex++;

        Close();
    }

    #endregion SetUp

    /// <summary>
    /// Print the voucher out when score win
    /// </summary>
    [ContextMenu("Print")]
    public void PrintVoucher()
    {
        LoadSetting();

        // Set the "voucher_status" of "redeem_code" to "redeemed" in VoucherCodeDatabaseModel.sqlite

        // Insert printed voucher data into VoucherDistributionDatabaseModel.sqlite to be sync to server
        List<string> colV = new List<string>();
        List<string> valV = new List<string>();

        colV.Add("email");
        colV.Add("voucher_code");
        colV.Add("redeem_code");
        colV.Add("is_sync");

        valV.Add(PlayerPrefs.GetString("email"));
        valV.Add(PlayerPrefs.GetString("voucher_code"));
        valV.Add(PlayerPrefs.GetString("redeem_code"));
        valV.Add("no");

        // Insert the printed out voucher_code, voucher_id, redeem_code, is_sync
        // to VoucherDistributionDBModelEntity.sqlite
        voucherDistributionDBModelEntity.AddData(colV, valV);
    }

    public void CheckDay()
    {
        string temp = PlayerPrefs.GetString("TheDate", "");
        if (temp == "")
        {
            string todaydate = DateTime.Now.Date.ToString();
            PlayerPrefs.SetString("TheDate", todaydate);
        }
        else
        {
            DateTime Date1 = DateTime.Parse(temp);
            DateTime Date2 = DateTime.Now.Date;

            int a = DateTime.Compare(Date1, Date2);
            //  if (a < 0) ResetDailyStock();

            string todaydate = DateTime.Now.Date.ToString();
            PlayerPrefs.SetString("TheDate", todaydate);
        }
    }

    [ContextMenu("Reset Daily Stock")]
    private void ResetDailyStock()
    {
        // update all voucher quantity to daily_quantity
        DataRowCollection drc = ExecuteCustomSelectQuery(string.Format("SELECT * FROM {0}", dbSettings.tableName));
        foreach (DataRow r in drc)
        {
            ExecuteCustomNonQuery(string.Format(
                "UPDATE {0} SET quantity = {1} WHERE id = {2}",
                new System.Object[] { dbSettings.tableName, r["daily_quantity"].ToString(), r["id"].ToString() }));
        }

        FindObjectOfType<VendingDailyStockDBModelEntity>().Refill();
    }
}

public class VoucherEntity
{
    public int _id;
    public string _type;
    public int _stock;
    public string _dateCreated; // Auto generated timestamp

    public VoucherEntity(string type, int stock)
    {
        _type = type;
        _stock = stock;
        _dateCreated = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public VoucherEntity(int id, string type, int stock)
    {
        _id = id;
        _type = type;
        _stock = stock;
        _dateCreated = "";
    }

    public VoucherEntity(int id, string type, int stock, string dateCreated)
    {
        _id = id;
        _type = type;
        _stock = stock;
        _dateCreated = dateCreated;
    }
}