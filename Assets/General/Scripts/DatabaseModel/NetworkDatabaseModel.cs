using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Net.NetworkInformation;
using System.Collections.Generic;

public class NetworkDatabaseModel : MonoBehaviour
{
    private static string json = File.ReadAllText(@"C:\UID-APP\env.json");
    private static JObject jsonObj = JObject.Parse(json);
    private static string PROJECT_CODE = jsonObj["PROJECT_CODE"].ToString();
    private static string TARGET_ETHERNET_IP = jsonObj["TARGET_ETHERNET_IP"].ToString();
    private static string FOLDER = jsonObj["UPDATE_SHARE_SQLITE_APP_FOLDER"].ToString();
    private static string FILE_NAME = jsonObj["UPDATE_SHARE_SQLITE_APP_FILE_NAME"].ToString() + ".sqlite";
    private static string BACKUP_FILE_NAME = jsonObj["UPDATE_SHARE_SQLITE_APP_FILE_NAME"].ToString() + "-backup.sqlite";

    //static string backupFileName = "VoucherCodeDBModelEntityBackup.sqlite";
    private static string COLUMN_NAME = jsonObj["UPDATE_SHARE_SQLITE_APP_COLUMN"].ToString();

    private static string COLUMN_VALUE = jsonObj["UPDATE_SHARE_SQLITE_APP_COLUMN_VALUE"].ToString();
    private static string COLUMN_FILTER = jsonObj["UPDATE_SHARE_SQLITE_APP_COLUMN_FILTER"].ToString();
    private static string folderPath = $"\\\\{TARGET_ETHERNET_IP}\\uid-app\\{PROJECT_CODE}\\{FOLDER}\\";
    private static string SHARE_FILE_PATH = folderPath + FILE_NAME;
    private static string SHARE_BACKUP_FILE_PATH = folderPath + BACKUP_FILE_NAME;
    private static string cs = $@"URI=file:{SHARE_FILE_PATH};PRAGMA journal_mode=WAL;";
    private static int TARGET_PING_TIMEOUT = JSONExtension.LoadEnvInt("TARGET_PING_TIMEOUT");

    public bool isConnected;

    [Button]
    public async Task<bool> PingHost()
    {
        bool pingable = false;
        System.Net.NetworkInformation.Ping pinger = null;
        pinger = new System.Net.NetworkInformation.Ping();

        try
        {
            await Task.Run(() =>
            {
                pinger = new System.Net.NetworkInformation.Ping();
                PingReply reply = pinger.Send(TARGET_ETHERNET_IP, TARGET_PING_TIMEOUT);
                pingable = reply.Status == IPStatus.Success;
            });
        }
        catch (PingException)
        {
            // Discard PingExceptions and return false;
            if (pinger != null)
                pinger.Dispose();
            return false;
        }
        finally
        {
            if (pinger != null)
                pinger.Dispose();
        }

        return pingable;
    }

    public void UpdateSharedDatabase(string redeemCode)
    {
        // counting 1
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        string cs = $@"URI=file:{SHARE_FILE_PATH};PRAGMA journal_mode=WAL;";
        using (SqliteConnection con = new SqliteConnection(cs))
        {
            con.Open();

            SqliteCommand cmd = new SqliteCommand(con);

            cmd.CommandText = $"UPDATE VoucherCodeDBModelEntity SET {COLUMN_NAME} = '{COLUMN_VALUE}' WHERE {COLUMN_FILTER} = '{redeemCode}'";
            cmd.ExecuteNonQuery();

            con.Close();
        }

        // counting 2
        stopWatch.Stop();
        // Get the elapsed time as a TimeSpan value.
        TimeSpan ts = stopWatch.Elapsed;

        // Format and display the TimeSpan value.
        string elapsedTime = String.Format("{0:00}.{1:00}",
            ts.Seconds,
            ts.Milliseconds / 10);
        UnityEngine.Debug.Log("RunTime " + elapsedTime);
    }

    public async Task<string> ReadData(string redeemCode)
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        using (SqliteConnection con = new SqliteConnection(cs))
        {
            con.Open();

            SqliteCommand cmd = new SqliteCommand(con);

            cmd.CommandText = $"SELECT voucher_status FROM VoucherCodeDBModelEntity WHERE {COLUMN_FILTER} = '{redeemCode}'";
            //cmd.CommandText = $"SELECT redeem_code, voucher_status FROM VoucherCodeDBModelEntity WHERE {COLUMN_FILTER} = '{redeemCode}'";

            SqliteDataReader reader = cmd.ExecuteReader();
            //UnityEngine.Debug.Log($"{reader[0]} | {reader[1]}");

            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}.{1:00}",
                ts.Seconds,
                ts.Milliseconds / 10);
            UnityEngine.Debug.Log($"{name} - network data status - {reader[0]}. Read time " + elapsedTime);

            //con.Close();
            return reader[0].ToString();
        }
    }

    [Button]
    public string SelectCustomData(string query)
    {
        //Stopwatch stopWatch = new Stopwatch();
        //stopWatch.Start();
        try
        {
            using (SqliteConnection con = new SqliteConnection(cs))
            {
                con.Open();

                SqliteCommand cmd = new SqliteCommand(con);

                cmd.CommandText = query;
                //cmd.CommandText = $"SELECT redeem_code, voucher_status FROM VoucherCodeDBModelEntity WHERE {COLUMN_FILTER} = '{redeemCode}'";

                SqliteDataReader reader = cmd.ExecuteReader();

                //stopWatch.Stop();

                //UnityEngine.Debug.Log($"network data status - {reader[0]}. Read time " + stopWatch.Elapsed);

                return reader[0].ToString();
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError(ex.Message);
            return "0";
        }
    }

    public void AddData(List<string> columns_, List<string> values_, string tableName)
    {
        try
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            using (SqliteConnection con = new SqliteConnection(cs))
            {
                con.Open();

                SqliteCommand cmd = new SqliteCommand(con);

                values_[columns_.IndexOf("is_sync")] = "other";

                cmd.CommandText =
               "INSERT INTO " + tableName
               + " ( ";

                for (int n = 0; n < columns_.Count; n++)
                {
                    if (n != columns_.Count - 1)
                    { cmd.CommandText += columns_[n] + ", "; }
                    else
                    {
                        cmd.CommandText += columns_[n];
                    }
                }

                cmd.CommandText += ")";
                cmd.CommandText += " VALUES ( '";

                for (int v = 0; v < values_.Count; v++)
                {
                    if (v != columns_.Count - 1)
                    { cmd.CommandText += values_[v] + "', '"; }
                    else
                    {
                        cmd.CommandText += values_[v];
                    }
                }

                cmd.CommandText += "')";

                cmd.ExecuteNonQuery();

                stopWatch.Stop();

                UnityEngine.Debug.Log($"network data status - execute time { stopWatch.Elapsed}");
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError(ex.Message);
        }
    }
}