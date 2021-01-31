using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using System.Linq;
using System.Collections.Generic;
using System.Data;

public class VoucherDistributionDBModelEntity : DBModelEntity
{
    public string syncFileName = "";
    public string syncDeleteFileName = "";
    private string filePath { get { return System.IO.Path.Combine(JSONExtension.SERVER_URL, syncFileName); } }
    private string deletedListFilePath { get { return System.IO.Path.Combine(JSONExtension.SERVER_URL, syncDeleteFileName); } }
    private string file;

    protected override void OnEnable()
    {
        base.OnEnable();
        file = filePath;
    }

    protected override IEnumerator SyncToServer()
    {
        yield return StartCoroutine(NetworkExtension.CheckForInternetConnectionRoutine());

        if (NetworkExtension.internet == false)
        {
            //No connection
            //ToogleHandler(blockDataHandler, false);
            //ToogleHandler(internetErrorHandler, false);
            Debug.Log(name + " - No internet. Stop SyncToServer() coroutine");

            SyncEnded();
            StopAllCoroutines();
            yield break;
        }

        if (OnSyncStart.GetPersistentEventCount() > 0) OnSyncStart.Invoke();

        yield return StartCoroutine(CompareServerData());

        // get all unsync data
        rows = GetAllCustomCondition();

        if (rows.Count < 1)
        {
            SyncEnded();
            StopAllCoroutines();
            Debug.Log(name + " no data to sync. Stop SyncToServer() coroutine");
            yield break;
        }

        //int totalSent = 0;
        //int totalNotSent = 0;

        // Get global event_code
        string source_identifier_code = JSONExtension.LoadEnv("SOURCE_IDENTIFIER_CODE");

        for (int u = 0; u < rows.Count; u++)
        {
            yield return StartCoroutine(NetworkExtension.CheckForInternetConnectionRoutine());

            if (NetworkExtension.internet == false)
            {
                //No connection
                //ToogleHandler(blockDataHandler, false);
                //ToogleHandler(internetErrorHandler, false);
                Debug.Log(name + "SyncToServer() Failed. No internet. Stop SyncToServer() coroutine");
                SyncEnded();
                StopAllCoroutines();
                yield break;
            }

            #region WWW Form

            WWWForm form = new WWWForm();

            // Debug.Log("field to sent : " + dbSettings.columnsToSync.Count);
            // show value send to server - 1
            string values = "";
            entityId = int.Parse(rows[u]["id"].ToString());

            for (int i = 0; i < dbSettings.columns.Count; i++)
            {
                if (!dbSettings.columns[i].sync) continue;

                string value = rows[u][dbSettings.columns[i].name].ToString();

                if (dbSettings.columns[i].name == "source_identifier_code") value = source_identifier_code;

                //if (dbSettings.columns[i].name == "email")
                //    value = source_identifier_code.ToLower() + "@gmail.com";

                // concatenate voucher_category to become "TIER1-MBGP"
                /* if (dbSettings.columns[i].name == "voucher_code")
                 {
                     DataRowCollection drc = ExecuteCustomSelectQuery(string.Format("SELECT voucher_category FROM {0} WHERE id = {1}",
                         new System.Object[] { dbSettings.tableName, entityId }));
                     value = value + "-" + drc[0][0].ToString();
                 }*/

                // show value send to server - 2
                values += value + " | ";

                form.AddField(
                    dbSettings.columns[i].name,
                   value);
            }

            // show value send to server - 3
            // Debug.Log(values);

            #endregion WWW Form

            #region WebRequest

            using (UnityWebRequest www = UnityWebRequest.Post((dbSettings.sendURL + dbSettings.sendAPI).Replace(" ", string.Empty), form))
            {
                //Debug.Log($"{name} - sending web request");
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    ErrorAction(www, $"server error \n Values: " + values);
                    ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET is_sync = 'fail' WHERE id = " + entityId);

                    // SyncEnded();
                    // StopAllCoroutines();
                    // yield break;
                }
                else
                {
                    //  yield return new WaitForEndOfFrame();
                    var jsonData = JsonUtility.FromJson<JSONResponse>(www.downloadHandler.text);

                    // request error handling
                    if (jsonData.result != dbSettings.serverResponsesArray[0].resultResponse)
                    {
                        Debug.LogError(name + " - " + jsonData.result + "\n Values : " + values);

                        if (!jsonData.result.Contains("Duplicate"))
                        { ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET is_sync = 'fail' WHERE id = " + entityId); }
                        else
                        {
                            ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET is_sync = 'duplicate' WHERE id = " + entityId);
                            Debug.LogError($"{name} - {jsonData.voucherDistributionId} is duplicated");
                        }

                        Debug.LogError($"{name} - {jsonData.result} | voucherDistributionId : {jsonData.voucherDistributionId} \n Error : {JObject.Parse(www.downloadHandler.text).SelectToken("error")}");
                    }
                    else
                    {
                        // update successfully sync is_sync to submitted
                        ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET is_sync = 'yes' WHERE id = " + entityId);
                    }
                }
            }

            yield return new WaitForSeconds(1.2f);

            #endregion WebRequest
        }
        SyncEnded();
    }

    [Button]
    public void SyncOnlineRedeemRecord()
    {
        string[] lines = System.IO.File.ReadAllLines(file);
        DataRowCollection drc = ExecuteCustomSelectQuery($"SELECT member_id, created_at, redeem_code FROM {dbSettings.tableName}");

        List<string> dataLines = new List<string>();
        for (int d = 0; d < drc.Count; d++)
            dataLines.Add($"{drc[d][0]},{drc[d][1]},{drc[d][2]}");

        List<string> uniqueList = lines.Except(dataLines).ToList();
        ConnectDb();

        //Debug.Log(uniqueList.Count);

        using (var transaction = db_connection.BeginTransaction())
        {
            var command = db_connection.CreateCommand();

            for (var i = 0; i < uniqueList.Count; i++)
            {
                string[] values = uniqueList[i].Split(',');

                //Debug.Log(values[2]);

                command.CommandText =
                $"INSERT INTO {dbSettings.tableName} (redeem_code, member_id, created_at, is_sync, source_identifier_code) VALUES ('{values[2]}', '{values[0]}', '{values[1]}', 'd', 'd')";

                command.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        Close();
        SyncOnlineDeletedList();
    }

    public void SyncOnlineDeletedList()
    {
        // DataRowCollection drcDeleted = ExecuteCustomSelectQuery($"SELECT redeem_code FROM {dbSettings.tableName}");
        string[] lines = System.IO.File.ReadAllLines(deletedListFilePath);

        // List<string> dataLines = new List<string>();
        // for (int d = 0; d < drcDeleted.Count; d++)
        //     dataLines.Add($"{drcDeleted[d][0]}");
        ConnectDb();

        using (var transaction = db_connection.BeginTransaction())
        {
            var command = db_connection.CreateCommand();

            for (var i = 0; i < lines.Length; i++)
            {
                //Debug.Log(lines[i]);
                string[] values = lines[i].Split(',');

                command.CommandText = $"DELETE FROM {dbSettings.tableName} WHERE redeem_code = '{lines[i]}'";

                command.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        Close();
    }
}