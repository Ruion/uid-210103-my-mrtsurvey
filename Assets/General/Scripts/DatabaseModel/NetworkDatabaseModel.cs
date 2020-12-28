using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Net;
using System.Text;
using System.Collections;
using System.Net.NetworkInformation;

public class NetworkDatabaseModel : MonoBehaviour
{
        static string json = File.ReadAllText(@"C:\UID_Toolkit\Global.json");
        static JObject jsonObj = JObject.Parse(json);
        static string PROJECT_CODE = jsonObj["Project_Code"].ToString();
        static string TARGET_ETHERNET_IP = jsonObj["Target_Ethernet_IP"].ToString();
        static string FOLDER = jsonObj["Update_Share_SQlite_App_Folder"].ToString();
        static string FILE_NAME = jsonObj["Update_Share_SQlite_App_File_Name"].ToString() + ".sqlite";
        static string BACKUP_FILE_NAME = jsonObj["Update_Share_SQlite_App_File_Name"].ToString() + "-backup.sqlite";
        //static string backupFileName = "VoucherCodeDBModelEntityBackup.sqlite";
        static string COLUMN_NAME = jsonObj["Update_Share_SQlite_App_Column"].ToString();
        static string COLUMN_VALUE = jsonObj["Update_Share_SQlite_App_Column_Value"].ToString();
        static string COLUMN_FILTER = jsonObj["Update_Share_SQlite_App_Column_Filter"].ToString();
        static string folderPath = $"\\\\{TARGET_ETHERNET_IP}\\uid-app\\{PROJECT_CODE}\\{FOLDER}\\";
        static string SHARE_FILE_PATH = folderPath + FILE_NAME;
        static string SHARE_BACKUP_FILE_PATH = folderPath + BACKUP_FILE_NAME;

        public bool isConnected;

        [Button]
        public async Task<bool> PingHost()
        {
            bool pingable = false;
            System.Net.NetworkInformation.Ping pinger = null;

            try
            {
                pinger = new System.Net.NetworkInformation.Ping();
                PingReply reply = await pinger.SendPingAsync(TARGET_ETHERNET_IP);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }
            UnityEngine.Debug.Log($"{name} - network status : {pingable}");
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

            string cs = $@"URI=file:{SHARE_FILE_PATH}";
            //Console.WriteLine("Connection string " + cs);

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
    }