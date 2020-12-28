﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Data;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;

/// <summary>
/// Create/delete table, insert/update/delete data, and send data to server \n
/// Tips: Set the field variable in inspector, use button to help create, delete, update table \n
/// Local > set fileName in inspector, table columns and attributes \n
/// Server > set server url, get data url \n
/// Handler > toogle hasSync and drag message gameObjects into corresponsding field \n
/// Tips: \n
/// Insert player data > use PlayerPrefsSaver or PlayerPrefsSave_Group to save the value
/// with name same with table column, call SaveToLocal() to insert data into database
/// </summary>
public class DBModelEntity : DBModelMaster
{
    protected DataRowCollection rows;
    public UnityEvent OnSyncStart;
    public UnityEvent OnSyncEnd;
    protected bool Saved = false;

    /// <summary>
    /// Save PlayerPrefs value into all table column set in inspector
    /// </summary>
    public override void SaveToLocal()
    {
        base.SaveToLocal();

        if (Saved)
        {
            Debug.Log(name + " : Data already saved to local");
            return;
        }

        Saved = true;

        List<string> col = new List<string>();
        List<string> val = new List<string>();

        for (int v = 1; v < dbSettings.columns.Count; v++)
        {
            col.Add(dbSettings.columns[v].name);
        }

        val.AddRange(col);

        for (int i = 0; i < col.Count; i++)
        {
            val[i] = PlayerPrefs.GetString(col[i]);
        }

        AddData(col, val);
    }

    /// <summary>
    /// Load setting like send url and call SyncToServer() coroutine to Send data to server
    /// </summary>
    public override void Sync()
    {
        base.Sync();
        StartCoroutine(SyncToServer());
    }

    /// <summary>
    /// Make web request and send data to server, data will continue to send regardless of any error encounter
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator SyncToServer()
    {
        
        yield return StartCoroutine(NetworkExtension.CheckForInternetConnectionRoutine());

        if (NetworkExtension.internet == false)
        {
            //No connection
            //ToogleHandler(blockDataHandler, false);
            //ToogleHandler(internetErrorHandler, false);
            Debug.Log(name + " No internet. Stop SyncToServer() coroutine");
            SyncEnded();
            StopAllCoroutines();
            yield break;
        }

        if (OnSyncStart.GetPersistentEventCount() > 0) OnSyncStart.Invoke();

        yield return StartCoroutine(CompareServerData());

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

        Debug.Log(name + " : Start sync");
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
                    //Debug.Log(name + " email : " + value);
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
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    ErrorAction(www, "server error \n Values : " + values);
                    totalNotSent++;
                    //ToogleStatusBar(failBar, totalNotSent);
                    //ToogleHandler(errorHandler, true);

                    ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET is_sync = 'fail' WHERE id = " + entityId);
                    SyncEnded();
                    StopAllCoroutines();
                    yield break;
                }
                else
                {
                    //  yield return new WaitForEndOfFrame();
                    var jsonData = JsonUtility.FromJson<JSONResponse>(www.downloadHandler.text);

                    if (jsonData.result != dbSettings.serverResponsesArray[0].resultResponse)
                    {
                        Debug.LogError(name + " - " + jsonData.result + "\n Values : " + values);

                        totalNotSent++;
                        ToogleStatusBar(failBar, totalNotSent);

                        if (!jsonData.result.Contains("Duplicate"))
                            ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET is_sync = 'fail' WHERE id = " + entityId);
                        else
                            ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET is_sync = 'duplicate' WHERE id = " + entityId);
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
        //ToogleHandler(blockDataHandler, false);
        //if (hasSync) failBar.GetComponent<StatusBar>().Finish();
        //if (hasSync) successBar.GetComponent<StatusBar>().Finish();
        SyncEnded();
    }

    protected virtual void SyncEnded()
    {
        if(rows != null)
            if(rows.Count > 0)
                rows.Clear();

        Close();
        if (OnSyncEnd.GetPersistentEventCount() > 0) OnSyncEnd.Invoke();
    }

    /// <summary>
    /// Show error message and handler when encounter error sending data to server
    /// </summary>
    /// <param name="www"></param>
    /// <param name="errorMessage"></param>
    protected void ErrorAction(UnityWebRequest www, string errorMessage)
    {
        ToogleHandler(blockDataHandler, false);
        ToogleHandler(failBar, true);

        Debug.LogError(name + " :\n" + errorMessage + "\n" + www.error + "\n" + " server url: " + dbSettings.sendURL + dbSettings.sendAPI);
    }
}