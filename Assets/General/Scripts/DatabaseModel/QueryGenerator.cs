using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Mono.Data.Sqlite;
using System.Data;

public class QueryGenerator : MonoBehaviour
{

    public VoucherCodeDBModelEntity voucherCodeDatabaseModel;
    public DBModelEntity databaseModel2;
    public string tableName;

    public enum QueryType { select = 1, update = 2, delete = 3 }

    public QueryType queryType = QueryType.select;

    [Button(ButtonSizes.Medium)]
    private void LogQuery()
    {
     
        // select test redeem_code from VoucherCodeDatabaseModel.sqlite
        DataRowCollection drc = voucherCodeDatabaseModel.ExecuteCustomSelectQuery("SELECT redeem_code FROM " + voucherCodeDatabaseModel.dbSettings.tableName + " WHERE campaign_code is null");

        string log = "";

        string customQuery = "UPDATE voucher_distributions SET WHERE redeem_code IN(";

        Debug.Log(name + " - " + drc.Count + " record");

        // DELETE FROM voucher_distributions WHERE redeem_code IN('Df6svVk7','wpd1LVAc')

        foreach (DataRow r in drc)
        {
            log += r["redeem_code"] + "\n";

            // create custom query string
            customQuery += "'"+ r["redeem_code"] + "',";
        }

        Debug.Log(log);

        customQuery += ")";

        Debug.Log(customQuery);


        

    }
}
