using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

using UnityEngine.Networking;

public class VoucherDistributionDBModelEntity : DBModelEntity
{
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

        //  Debug.Log(name + " : unsync data : " + rows.Count);
        int totalSent = 0;
        int totalNotSent = 0;

        //Debug.Log(name + " : Start sync");
        //ToogleHandler(blockDataHandler, true);

        // Get global event_code
        string source_identifier_code = JSONExtension.LoadSetting(dbSettings.folderPath + "\\Setting", "source_identifier_code");

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

                if (dbSettings.columns[i].name == "email")
                {
                    value = source_identifier_code.ToLower() + "@gmail.com";
                }

                if (dbSettings.columns[i].name == "voucher_code")
                {
                    DataRowCollection drc = ExecuteCustomSelectQuery(string.Format("SELECT voucher_category FROM {0} WHERE id = {1}",
                        new System.Object[] { dbSettings.tableName, entityId }));
                    value = value + "-" + drc[0][0].ToString();
                    //Debug.Log(name + " - voucher_code : " + value);
                }

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
                    ErrorAction(www, "server error \n Values : " + values);
                    totalNotSent++;
                    //ToogleStatusBar(failBar, totalNotSent);
                    //ToogleHandler(errorHandler, true);

                    SyncEnded();
                    StopAllCoroutines();
                    yield break;
                    //ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET is_sync = 'fail' WHERE id = " + entityId);
                }
                else
                {
                    //  yield return new WaitForEndOfFrame();
                    var jsonData = JsonUtility.FromJson<JSONResponse>(www.downloadHandler.text);

                    if (jsonData.result != dbSettings.serverResponsesArray[0].resultResponse)
                    {
                        Debug.LogError(name + " - " + jsonData.result + "\n Values : " + values);

                        totalNotSent++;
                        //ToogleStatusBar(failBar, totalNotSent);

                        //if (!jsonData.result.Contains("Duplicate"))
                        //if (!jsonData.result.Contains("Duplicate Redeem Code"))
                        if (!jsonData.result.Contains("Duplicate"))
                            ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET is_sync = 'fail' WHERE id = " + entityId);
                        else
                        {
                            ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET is_sync = 'duplicate' WHERE id = " + entityId);
                            Debug.LogError($"{name} - {jsonData.voucherDistributionId} is duplicated");
                        }

                        Debug.LogError($"{name} - {jsonData.result} | voucherDistributionId : {jsonData.voucherDistributionId}");
                    }
                    else
                    {
                        // Debug.Log(name + " : " + dbSettings.serverResponsesArray[0].resultResponseMessage);

                        // update successfully sync is_sync to submitted
                        ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET is_sync = 'yes' WHERE id = " + entityId);
                        totalSent++;

                        //ToogleStatusBar(successBar, totalSent);
                    }

                    //  yield return new WaitForEndOfFrame();
                }
            }

            yield return new WaitForSeconds(1.2f);

            #endregion WebRequest
        }
        ////ToogleHandler(blockDataHandler, false);
        //if (hasSync) failBar.GetComponent<StatusBar>().Finish();
        //if (hasSync) successBar.GetComponent<StatusBar>().Finish();
        SyncEnded();
    }

}