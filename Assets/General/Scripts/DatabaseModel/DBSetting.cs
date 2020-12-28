using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using Sirenix.OdinInspector;

#region Save Load Class
/// <summary>
/// Hold various local and server database settings
/// </summary>
public class DBSetting
{
    
    // save the game setting file
    public static void SaveSetting(DBEntitySetting setting)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = setting.folderPath + "\\" + setting.fileName + ".dbsetting";
        setting.SetUpTextPath();
        FileStream stream = new FileStream(path, FileMode.Create);

       // DBEntitySetting s = new DBEntitySetting(setting);
        try
        {
           // formatter.Serialize(stream, s);
            formatter.Serialize(stream, setting);
            stream.Close();
            Debug.Log("Save setting success");

        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    // load the game setting file
    public static DBEntitySetting LoadSetting(string fileName)
    {
        string path = fileName + ".dbsetting";
        Debug.Log(path);
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using(FileStream stream = new FileStream(path, FileMode.Open))
            {
                DBEntitySetting setting = (DBEntitySetting)formatter.Deserialize(stream);
                stream.Close();
                setting.SetUpTextPath();
                return setting;
            }
        }
        else
        {
            Debug.LogError(" Save file not found in " + path);
            return null;
        }
    }

}
#endregion

[System.Serializable]
public class DBEntitySetting
{
    public string fileName;
    [FolderPath(AbsolutePath = true, UseBackslashes = true)]
    public string folderPath;

    [HideInInspector] public string dbName;
    [HideInInspector] public string tableName;

    [TabGroup("LocalDB Settings")] [TableList]
    public List<TableColumn> columns = new List<TableColumn>();

    [TabGroup("Server")] public string sendURL;
    [TabGroup("Server")] public string sendAPI;
    [TabGroup("Server")] public ServerResponses[] serverResponsesArray;

    [TabGroup("Server")]
    public bool hasMultipleLocalDB = false;
    [TabGroup("Server")][ShowIf("hasMultipleLocalDB", false)] public string keyDownloadAPI;
    [TabGroup("Server")][ShowIf("hasMultipleLocalDB", false)] public string keyFileName;
    [TabGroup("Server")][ShowIf("hasMultipleLocalDB", false)] public string serverEmailFilePath;

    public void SetUpTextPath()
    {
        serverEmailFilePath = folderPath + "\\" + keyFileName + ".txt";
        if (!File.Exists(serverEmailFilePath))
        {
            File.WriteAllText(serverEmailFilePath, "");
        }
    }

/*
    public DBEntitySetting(DBEntitySetting setting)
    {
        fileName = setting.fileName;
        dbName = setting.fileName;
        tableName = setting.fileName;
        columns = setting.columns;

        sendURL = setting.sendURL;
        serverResponsesArray = setting.serverResponsesArray;

        hasMultipleLocalDB = setting.hasMultipleLocalDB;
        keyDownloadURL = setting.keyDownloadURL;
        keyFileName = setting.keyFileName;
        serverEmailFilePath = setting.serverEmailFilePath;
    }
    */
}


[Serializable]
public class TableColumn
{
    [TableColumnWidth(80)] public string name;
    [TableColumnWidth(150)] public string attribute;
    

    [TableColumnWidth(30)] public bool sync;

    public string dummyPrefix;

    public TableColumn(string name_, string attribute_, bool sync_)
    {
        name = name_;
        attribute = attribute_;
        sync = sync_;
        dummyPrefix = name_;
    }
}

[Serializable]
public class ServerResponses
{
    public string resultResponse;
    public string resultResponseMessage;
}