using UnityEngine;
using TMPro;

public class AutoFormFill : ComponentTaskInvoker
{
    public VoucherCodeDBModelEntity vcDB;
    public TMP_InputField field;

    public void AutoFillField()
    {
        VerifyDebugMode();

        if (!isDebugging) return;

        string redeem_code =  vcDB.ExecuteCustomSelectQuery(
           string.Format("SELECT * FROM {0} WHERE voucher_status = 'ready' LIMIT 1", vcDB.dbSettings.tableName))
            [0]["redeem_code"].ToString();

        field.text = redeem_code;
    }
}
