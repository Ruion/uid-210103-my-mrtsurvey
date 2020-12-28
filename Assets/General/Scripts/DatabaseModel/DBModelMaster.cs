using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.Data.Common;
using System;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Threading.Tasks;

/// <summary>
/// Sqlite database master class that hold database settings, methods for manipulating user data \n
/// Method Utilities : \n
/// Basic : Get, Insert, Update, Delete \n
/// Server : Fetch string list from server \n
/// Public : Execute custom query on this database with ExecuteCustomNonQuery() and ExecuteCustomQuery() \n
/// Notes: see usage in DBModelEntity which inherit and extend this class
/// </summary>
public class DBModelMaster : DBSettingEntity
{
    #region fields

    private const string CodistanTag = "Codistan: SqliteHelper:\t";

    [HideInInspector] public string db_connection_string;
    [HideInInspector] public IDbConnection db_connection;
    [HideInInspector] public SqliteConnection sqlitedb_connection;

    [FoldoutGroup("Populate Setting")] public int numberToPopulate = 10;
    [FoldoutGroup("Populate Setting")] public int TestIndex = 0;
    [FoldoutGroup("Populate Setting")] public string selectCustomCondition = "is_sync = 'no'";

    [ToggleGroup("hasSync")]
    public bool hasSync = false;

    [ToggleGroup("hasSync")] public GameObject emptyHandler;
    [ToggleGroup("hasSync")] public GameObject internetErrorHandler;
    [ToggleGroup("hasSync")] public GameObject errorHandler;
    [ToggleGroup("hasSync")] public GameObject blockDataHandler;
    [ToggleGroup("hasSync")] public GameObject successBar;
    [ToggleGroup("hasSync")] public GameObject failBar;
    [ToggleGroup("hasSync")] [ReadOnly] public int entityId;

    [HideInInspector] public List<string> serverEmailList;

    [ToggleGroup("hasSync")]
    [ReadOnly] public bool isFetchingData = false;

    #endregion fields

    protected virtual void OnEnable()
    {
        CreateTable();
    }

    #region setUp

    [ContextMenu("CreateTable")]
    public virtual void CreateTable()
    {
        LoadSetting();

        ConnectDb();

        IDbCommand dbcmd = GetDbCommand();

        // columns for table
        dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + dbSettings.tableName + " ( ";

        for (int i = 0; i < dbSettings.columns.Count; i++)
        {
            dbcmd.CommandText += "'" + dbSettings.columns[i].name + "' " + dbSettings.columns[i].attribute;

            if (i != dbSettings.columns.Count - 1) dbcmd.CommandText += " , ";
            else dbcmd.CommandText += " ) ; ";
        }

        try
        {
            dbcmd.ExecuteNonQuery(); Close();
        }
        catch (Exception ex) { Close(); Debug.LogError(ex.Message + "\n" + dbcmd.CommandText); return; }

        Close();
    }

    public virtual void ConnectDb()
    {
        /// we use StreamingAssets folder in pass, but now use C:\UID-APP\APPS folder now
       // db_connection_string = "URI = file:" + Application.streamingAssetsPath + "/" + dbSettings.dbName + ".sqlite";
        //db_connection_string = "URI = file:" + dbSettings.folderPath + "\\Databases\\" + dbSettings.dbName + ".sqlite;PRAGMA journal_mode=WAL;";
        db_connection_string = "Data Source=" + dbSettings.folderPath + "\\Databases\\" + dbSettings.dbName + ".sqlite;PRAGMA journal_mode=WAL;";
        //Debug.Log(db_connection_string, gameObject);
        db_connection = new SqliteConnection(db_connection_string);
        db_connection.Open();
    }

    //helper functions
    protected IDbCommand GetDbCommand()
    {
        return db_connection.CreateCommand();
    }

    #endregion setUp

    #region Insert

    public virtual string AddData(List<string> columns_, List<string> values_)
    {
        ConnectDb();

        #region example

        /*
        IDbCommand dbcmd = GetDbCommand();
        dbcmd.CommandText =
            "INSERT INTO " + dbSettings.tableName
            + " ( "
            + KEY_NAME + ", "
            + KEY_EMAIL + ", "
            + KEY_CONTACT + ", "
            + KEY_GAME_SCORE + " ) "

            + "VALUES ( '"
            + user.name + "', '"
            + user.email + "', '"
            + user.contact + "', '"
            + user.score + "' )";
        dbcmd.ExecuteNonQuery();

        foreach (var item in columns_)
        {
        }

        IDbCommand dbcmd2 = GetDbCommand();
        dbcmd2.CommandText =
            "INSERT INTO " + dbSettings.tableName
            + " ( "
            + KEY_NAME + ", "
            + KEY_EMAIL + ", "
            + KEY_CONTACT + ", "
            + KEY_GAME_SCORE + " ) "

            + "VALUES ( '"
            + user.name + "', '"
            + user.email + "', '"
            + user.contact + "', '"
            + user.score + "' )";
        dbcmd.ExecuteNonQuery();
        */

        #endregion example

        IDbCommand dbcmd2 = GetDbCommand();
        dbcmd2.CommandText =
            "INSERT INTO " + dbSettings.tableName
            + " ( ";

        for (int n = 0; n < columns_.Count; n++)
        {
            if (n != columns_.Count - 1)
            { dbcmd2.CommandText += columns_[n] + ", "; }
            else
            {
                dbcmd2.CommandText += columns_[n];
            }
        }

        dbcmd2.CommandText += ")";
        dbcmd2.CommandText += " VALUES ( '";

        for (int v = 0; v < values_.Count; v++)
        {
            if (v != columns_.Count - 1)
            { dbcmd2.CommandText += values_[v] + "', '"; }
            else
            {
                dbcmd2.CommandText += values_[v];
            }
        }

        dbcmd2.CommandText += "')";

        using (db_connection)
        {
            try
            {
                dbcmd2.ExecuteNonQuery(); Close();
                Debug.Log(name + " : Inserted new record \n" + dbcmd2.CommandText);
                return "true";
            }
            catch (DbException ex)
            {
                string msg = string.Format("ErrorCode: {0}", ex.Message + "\n" + dbcmd2.CommandText);
                Debug.LogError(msg); Close();
                return msg;
            }
        }
    }

    #endregion Insert

    #region Get

    [ButtonGroup("DBGet")]
    [Button("Show All", ButtonSizes.Medium)]
    public virtual DataTable GetAllDataInToDataTable()
    {
        ConnectDb();
        try
        {
            string query = "SELECT * FROM " + dbSettings.tableName;
            sqlitedb_connection = new SqliteConnection(db_connection_string);

            SqliteCommand cmd = new SqliteCommand(query, sqlitedb_connection);

            SqliteDataAdapter da = new SqliteDataAdapter(cmd);

            DataTable dt = new DataTable();
            da.Fill(dt);

            /*            Debug.Log("Columns : " + dt.Columns.Count + "| Rows : " + dt.Rows.Count +
                            "\n" +
                            "View in Tools > Local DB (selecting the db gameObject) "
                            );
                            */

            foreach (DataRow r in dt.Rows)
            {
                string record = "";

                foreach (TableColumn col in dbSettings.columns)
                {
                    record += col.name + " : " + r[col.name].ToString() + " | ";
                    // Debug.Log(record);
                }
                Debug.Log(record);
            }

            Close();
            return dt;
        }
        catch (DbException ex)
        {
            Debug.LogError(name + " : Error : " + ex.Message);
            Close();
            return null;
        }
    }

    [ButtonGroup("DBGet")]
    [Button("Show Custom", ButtonSizes.Medium)]
    public DataRowCollection GetAllCustomCondition()
    {
        ConnectDb();
        try
        {
            string query = "SELECT * FROM " + dbSettings.tableName + " WHERE " + selectCustomCondition;
            sqlitedb_connection = new SqliteConnection(db_connection_string);

            SqliteCommand cmd = new SqliteCommand(query, sqlitedb_connection);

            SqliteDataAdapter da = new SqliteDataAdapter(cmd);

            DataTable dt = new DataTable();
            da.Fill(dt);

            //            Debug.Log("Columns : " + dt.Columns.Count + "| Rows : " + dt.Rows.Count);

            foreach (DataRow r in dt.Rows)
            {
                string record = "";

                foreach (TableColumn col in dbSettings.columns)
                {
                    record += r[col.name].ToString() + " | ";
                }
                //Debug.Log(record);
            }
            
            Close();
            //Debug.Log("Show Custom : Row count - " + dt.Rows.Count, gameObject);
            return dt.Rows;
        }
        catch (DbException ex)
        {
            //Debug.LogError("Error : " + ex.Message + "\n" + dbSettings.folderPath);
            Close();
            return null;
        }
    }

    [Button("Show AllUnSync", ButtonSizes.Medium)]
    public void GetAllUnSync()
    {
         ConnectDb();
        try
        {
            //Debug.Log(dbSettings.tableName, gameObject);
            string query = "SELECT * FROM " + dbSettings.tableName + " WHERE is_sync = 'no' OR is_sync = 'fail' ORDER BY is_sync DESC";
            sqlitedb_connection = new SqliteConnection(db_connection_string);

            SqliteCommand cmd = new SqliteCommand(query, sqlitedb_connection);

            SqliteDataAdapter da = new SqliteDataAdapter(cmd);

            DataTable dt = new DataTable();
            da.Fill(dt);

            //            Debug.Log("Columns : " + dt.Columns.Count + "| Rows : " + dt.Rows.Count);

            foreach (DataRow r in dt.Rows)
            {
                string record = "";

                foreach (TableColumn col in dbSettings.columns)
                {
                    record += r[col.name].ToString() + " | ";
                }
                //Debug.Log(record);
            }
            Debug.Log("GetAllUnSync Custom : Row count - " + dt.Rows.Count, gameObject);
            Close();
        }
        catch (DbException ex)
        {
            Debug.LogError("Error : " + ex.Message + "\n" + dbSettings.folderPath);
            Close();
        }
    }

    public List<string> GetDataByStringToList(string item)
    {
        List<string> list = new List<string>();

        try
        {
            ConnectDb();
            DataRowCollection drc = ExecuteCustomSelectQuery("SELECT " + item + " FROM " + dbSettings.tableName);

            for (int d = 0; d < drc.Count; d++)
            {
                list.Add(drc[d][0].ToString());
            }

            // add close 1
            Close();
            return list;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }

    #endregion Get

    #region Delete

    [ContextMenu("DropTable")]
    protected virtual void DeleteAllData()
    {
        ConnectDb();
        IDbCommand dbcmd = db_connection.CreateCommand();
        dbcmd.CommandText = "DROP TABLE IF EXISTS " + dbSettings.tableName;
        dbcmd.ExecuteNonQuery();
        Close();
        TestIndex++;
    }

    [Button(ButtonSizes.Medium)]
    [FoldoutGroup("Populate Setting")]
    [HorizontalGroup("Populate Setting/Btn")]
    protected virtual void ClearAllData()
    {
        ConnectDb();
        IDbCommand dbcmd = db_connection.CreateCommand();
        dbcmd.CommandText = "DELETE FROM " + dbSettings.tableName;
        dbcmd.ExecuteNonQuery();
        Close();
        TestIndex++;
    }

    #endregion Delete

    #region Update

    public void UpdateData(List<string> columns_, List<string> values_, string conditions)
    {
        /*
        UPDATE table_name
        SET column1 = value1, column2 = value2...., columnN = valueN
        WHERE[condition];
        */
        ConnectDb();

        IDbCommand dbcmd2 = GetDbCommand();
        dbcmd2.CommandText =
            "UPDATE " + dbSettings.tableName
            + " SET ";

        for (int c = 0; c < columns_.Count; c++)
        {
            dbcmd2.CommandText += columns_[c] + " = '";
        }

        for (int v = 0; v < values_.Count; v++)
        {
            if (v != values_.Count - 1) dbcmd2.CommandText += values_[v] + "' ,";
            else dbcmd2.CommandText += values_[v] + "'";
        }

        dbcmd2.CommandText += " WHERE ";
        dbcmd2.CommandText += conditions + " ;";

        try
        {
            int result = dbcmd2.ExecuteNonQuery();
            if (result == 0) Debug.LogError("query not successful");

            // add close 2
            Close();
        }
        catch (DbException ex)
        {
            string msg = string.Format("ErrorCode: {0}", ex.Message);
            Debug.LogError(dbcmd2.CommandText);
            Debug.LogError(msg);

            // add close 2
            Close();
        }

        Close();
    }

    #endregion Update

    #region Custom Query

    public virtual void ExecuteCustomNonQuery(string query)
    {
        #region Usage

        // Usage
        // ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET quantity = " + voucher_quantity + " WHERE name = '" + voucher_name + "'");

        #endregion Usage

        ConnectDb();
        Close();
        // db_connection_string = "URI = file:" + dbSettings.folderPath + "\\Databases\\"+ dbSettings.dbName + ".sqlite";

        sqlitedb_connection = new SqliteConnection(db_connection_string);
        sqlitedb_connection.Open();

        SqliteCommand cmd = new SqliteCommand(query, sqlitedb_connection);

        try
        {
            cmd.ExecuteNonQuery();

            Debug.Log(name + " - Custom Query success" + "\n" + cmd.CommandText);
            sqlitedb_connection.Close();
        }
        catch (DbException ex)
        {
            Debug.Log(name + " - Error : " + ex.Message + "\n" + cmd.CommandText);
            sqlitedb_connection.Close();
        }
    }

    public virtual DataRowCollection ExecuteCustomSelectQuery(string query)
    {
        #region Usage

        // Usage
        // DataRowCollection drc = ExecuteCustomSelectQuery("SELECT " + item + " FROM " + dbSettings.tableName);
        //    for (int d = 0; d < drc.Count; d++)
        //    {
        //        list.Add(drc[0][0].ToString()); drc[0][0] means drc[row0][column0]
        //    }

        #endregion Usage

        ConnectDb();

        // add close
        Close();
        try
        {
            sqlitedb_connection = new SqliteConnection(db_connection_string);

            SqliteCommand cmd = new SqliteCommand(query, sqlitedb_connection);

            SqliteDataAdapter da = new SqliteDataAdapter(cmd);

            DataTable dt = new DataTable();

            da.Fill(dt);

            Close();

            return dt.Rows;
        }
        catch (DbException ex)
        {
            Debug.LogError(name + " : Error : " + ex.Message);
            Close();
            return null;
        }
    }

    public virtual SqliteDataReader ExecuteCustomSelectSingle(string query)
    {
        #region Usage

        // Usage
        // DataRowCollection drc = ExecuteCustomSelectQuery("SELECT " + item + " FROM " + dbSettings.tableName);
        //    for (int d = 0; d < drc.Count; d++)
        //    {
        //        list.Add(drc[0][0].ToString()); drc[0][0] means drc[row0][column0]
        //    }

        #endregion Usage

        ConnectDb();

        // add close
        Close();

        sqlitedb_connection = new SqliteConnection(db_connection_string);
        sqlitedb_connection.Open();

        SqliteCommand cmd = new SqliteCommand(query, sqlitedb_connection);

        try
        {
            SqliteDataReader reader = cmd.ExecuteReader();
            reader.Read();

            //if(reader.Read())
            // add close 3
            //sqlitedb_connection.Close();

            return reader;
        }
        catch (DbException ex)
        {
            Debug.LogError(name + " : Error : " + ex.Message);
            sqlitedb_connection.Close();
            return null;
        }

    }

    public virtual object ExecuteCustomSelectObject(string query)
    {
        #region Usage

        // Usage
        // DataRowCollection drc = ExecuteCustomSelectQuery("SELECT " + item + " FROM " + dbSettings.tableName);
        //    for (int d = 0; d < drc.Count; d++)
        //    {
        //        list.Add(drc[0][0].ToString()); drc[0][0] means drc[row0][column0]
        //    }

        #endregion Usage

        ConnectDb();

        sqlitedb_connection = new SqliteConnection(db_connection_string);
        sqlitedb_connection.Open();

        SqliteCommand cmd = new SqliteCommand(query, sqlitedb_connection);

        try
        {
            object queryObject = cmd.ExecuteScalar();
            // add close 4
            sqlitedb_connection.Close();

            return queryObject;
        }
        catch (DbException ex)
        {
            Debug.LogError(name + " : Error : " + ex.Message);
            sqlitedb_connection.Close();
            return null;
        }
    }

    #endregion Custom Query

    public virtual void Close()
    {
        db_connection.Close();
    }

    [Button(ButtonSizes.Medium)]
    [HorizontalGroup("Populate Setting/Btn")]
    protected virtual void Populate()
    {
        CreateTable();

        List<string> col = new List<string>();
        List<string> val = new List<string>();

        for (int v = 1; v < dbSettings.columns.Count; v++)
        {
            col.Add(dbSettings.columns[v].name);
        }

        val.AddRange(col);

        for (int n = 0; n < numberToPopulate; n++)
        {
            for (int i = 0; i < col.Count; i++)
            {
                val[i] = dbSettings.columns[i + 1].dummyPrefix + ((n + 1).ToString());
            }

            val[val.Count - 1] = "no";

            AddData(col, val);
        }

        TestIndex++;
    }

    #region handler

    public virtual void HideAllHandler()
    {
        if (!hasSync) return;

        if (emptyHandler != null) emptyHandler.SetActive(false);
        internetErrorHandler.SetActive(false);
        errorHandler.SetActive(false);
        blockDataHandler.SetActive(false); ;
        //  successBar.SetActive(false);
        //  failBar.SetActive(false);
    }

    protected virtual void ToogleHandler(GameObject handler, bool state = false)
    {
        if (handler == null) return;

        if (handler.transform.parent.gameObject.activeInHierarchy) handler.SetActive(state);
    }

    protected virtual void ToogleStatusBar(GameObject bar, int total)
    {
        if (bar == null) return;
        ToogleHandler(bar, true);
        bar.GetComponentInChildren<TextMeshProUGUI>().text = total.ToString();
    }

    #endregion handler

    #region Save & Sync

    public virtual void SaveToLocal()
    {
        LoadSetting();
    }

    [DisableIf("@String.IsNullOrEmpty(dbSettings.sendURL)")]
    [Button(ButtonSizes.Medium)]
    public virtual void Sync()
    {
        //LoadSetting();
        HideAllHandler();
    }

    #endregion Save & Sync

    #region Online Server Fetching

    public void AddUniqueDataToStringList(string text, List<string> dataList)
    {
        if (!dataList.Exists(i => i == text)) dataList.Add(text);
    }

    public bool RemoveDuplicateStringItem(string text, List<string> list)
    {
        string foundData = list.FirstOrDefault(i => i == text);
        if (foundData != null)
        {
            list.Remove(foundData);
            Debug.LogWarning("Remove duplicate item with " + foundData);
            return true;
        }
        else
        {
            return false;
        }
    }

    #region Legacy

    private void SetUpTextPath()
    {
        dbSettings.serverEmailFilePath = dbSettings.folderPath + "\\" + dbSettings.keyFileName + ".txt";
        if (!File.Exists(dbSettings.serverEmailFilePath))
        {
            File.WriteAllText(dbSettings.serverEmailFilePath, "");
        }

        isFetchingData = true;
    }

    #endregion Legacy

    public void DoGetDataFromServer()
    {
        try
        {
            if (isFetchingData) return;
            StartCoroutine(GetDataFromServer());
        }
        catch
        {
            Debug.LogError(name);
        }
    }

    public IEnumerator GetDataFromServer()
    {
        if (!dbSettings.hasMultipleLocalDB) yield break;
        isFetchingData = true;

        dbSettings.SetUpTextPath();

        serverEmailList = new List<string>();

        NetworkExtension.timeOut = FindObjectOfType<GameSettingEntity>().gameSettings.checkInternetTimeOut;

        Debug.Log(name + " GetDataFromServer() started");

        var watchCon = System.Diagnostics.Stopwatch.StartNew();
        watchCon.Start();

        yield return StartCoroutine(NetworkExtension.CheckForInternetConnectionRoutine());

        if (NetworkExtension.internet == false)
        {
            //No internet connection, stop this Coroutine
            isFetchingData = false;
            watchCon.Stop();
            Debug.LogError(name + " - GetDataFromServer() FAILED. No internet connection. Stop GetDataFromServer() check for internet duration taken " + watchCon.Elapsed.TotalMilliseconds);
            yield break;
        }

        watchCon.Stop();
        Debug.Log(name + " - GetDataFromServer() check for internet duration taken " + watchCon.Elapsed.TotalMilliseconds);

        // var time = System.Diagnostics.Stopwatch.StartNew();

        using (UnityWebRequest www = UnityWebRequest.Get(dbSettings.sendURL + dbSettings.keyDownloadAPI))
        {
            //  ulong downloadBytesOrigin = new ulong();
            www.timeout = FindObjectOfType<GameSettingEntity>().gameSettings.downloadCodeAPITimeOut;

            var watchRequest = System.Diagnostics.Stopwatch.StartNew();
            watchRequest.Start();

            yield return www.SendWebRequest();
            watchRequest.Stop();
            Debug.Log(name + " - GetDataFromServer() send request duration taken " + watchRequest.Elapsed.TotalMilliseconds);

            var watch = System.Diagnostics.Stopwatch.StartNew();
            watch.Start();
            Debug.Log(name + " start downloading redeem codes");

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(name + "\n" + www.error + "\n" + dbSettings.sendURL + dbSettings.keyDownloadAPI);
                Debug.LogError(name + " - GetDataFromServer() FAILED. No internet connection. Stop GetDataFromServer() check for internet duration taken " + watchCon.Elapsed.TotalMilliseconds);

                isFetchingData = false;
                yield break;
            }
            else
            {
                while (!www.downloadHandler.isDone)
                {
                    yield return null;
                }

                watch.Stop();
                Debug.Log(name + " - GetDataFromServer() download codes time : " + watch.Elapsed.TotalSeconds);

                string texts = www.downloadHandler.text;
                // Debug.Log("download used redeem list :\n"+texts);

                // clear text file
                File.WriteAllText(dbSettings.serverEmailFilePath, "");

                // write email list to file
                StreamWriter writer = new StreamWriter(dbSettings.serverEmailFilePath, true); //open txt file (doesnt actually open it inside the game)
                writer.Write(texts); //write into txt file the string declared above
                writer.Close();

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

                Debug.Log(name + " - GetDataFromServer() SUCCESS");
            }
        }

        // time.Stop();
        // Debug.Log(name + " - GetDataFromServer() download codes time : " + time.Elapsed.TotalMilliseconds);

        isFetchingData = false;
    }

    /// <summary>
    /// call GetDataFromServer() coroutine to get the list of email from server, call UpdateEmailExistedOnline() to
    /// update "is_sync = 'duplicate'" of email in local database if the email already exist in server
    /// </summary>
    /// <returns></returns>
    public IEnumerator CompareServerData()
    {
        yield return StartCoroutine(GetDataFromServer());
        UpdateEmailExistedOnline();
    }

    /// <summary>
    /// update "is_sync = 'duplicate'" of email in local database if the email already exist in server
    /// </summary>
    public void UpdateEmailExistedOnline()
    {
        if (serverEmailList.Count < 0) return;

        for (int o = 0; o < serverEmailList.Count; o++)
        {
            ExecuteCustomNonQuery("UPDATE " + dbSettings.tableName + " SET is_sync = 'duplicate' WHERE email = '" + serverEmailList[o] + "'");
        }

        serverEmailList = new List<string>();
    }

    #endregion Online Server Fetching

    private void OnDisable()
    {
        Close();
    }
}