using Sirenix.OdinInspector;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System;

/// <summary>
/// Base class for DBModelMaster
/// Tips: Inherit this class to allow accessing database setting
/// </summary>
public class DBSettingEntity : SerializedMonoBehaviour
{
    public DBEntitySetting dbSettings;

    #region Basics

    public virtual void Awake()
    {
        // LoadSetting();
    }

    [Button(ButtonSizes.Large), GUIColor(.3f, .78f, .78f)]
    [ButtonGroup("Setting")]
    public virtual void SaveSetting()
    {
        dbSettings.fileName = name;
        dbSettings.tableName = name;
        dbSettings.dbName = name;
        dbSettings.keyFileName = name + " Online";

        GameSettingEntity gse = FindObjectOfType<GameSettingEntity>().GetComponent<GameSettingEntity>();

        dbSettings.sendURL = gse.Server_URL;

        // fetch & Update setting from Setting.json

        dbSettings.folderPath = gse.Project_Folder;

        Directory.CreateDirectory(Path.GetDirectoryName(dbSettings.folderPath + "\\Databases\\"));

        // add api to Setting.json - playerdata_sendAPI : submit-player-data
        DBSettingEntity[] dBSettingEntities = FindObjectsOfType<DBSettingEntity>();

        //JSONSetter jsonSetter = gse.jsonSetter;
        foreach (DBSettingEntity e in dBSettingEntities)
        {
            if (string.IsNullOrEmpty(e.dbSettings.sendAPI)) continue;

            //jsonSetter.UpdateSetting(e.dbSettings.fileName + "-API", e.dbSettings.sendAPI);

            // save to Settings.json
            JSONExtension.SaveSetting(gse.SettingFilePath, e.dbSettings.fileName + "-API", e.dbSettings.sendAPI);
        }

        // legacy binary formatter save method
        // DBSetting.SaveSetting(dbSettings);

        // save to json file
        JSONExtension.SaveObject(dbSettings.folderPath + "\\Settings\\" + name, dbSettings);
    }

    [Button(ButtonSizes.Large), GUIColor(.3f, .78f, .78f)]
    [ButtonGroup("Setting")]
    public virtual void LoadSetting()
    {
        GameSettingEntity gse = FindObjectOfType<GameSettingEntity>().GetComponent<GameSettingEntity>();

        // fetch & Update setting from global JSONSetter

        dbSettings.folderPath = gse.Project_Folder;

        string filePath = dbSettings.folderPath + "\\Settings\\" + name;

        // Load from json file
        dbSettings = JsonConvert.DeserializeObject<DBEntitySetting>(File.ReadAllText(filePath+".json"));

        // fetch & Update setting from global JSONSetter
        //JSONSetter jsonSetter = gse.jsonSetter;

        //JObject jObject = jsonSetter.LoadSetting();
        JObject jObject = JSONExtension.LoadJson(gse.SettingFilePath);
        dbSettings.sendURL = gse.Server_URL;

        dbSettings.folderPath = gse.Project_Folder;

        var substrings = new[] { "api" };
        if (!dbSettings.sendURL.ContainsAny(substrings, StringComparison.CurrentCultureIgnoreCase))
            dbSettings.sendURL += "public/api/";

        // load sendAPI from global setting file
        if (jObject.ContainsKey(dbSettings.fileName + "-API")) dbSettings.sendAPI = jObject[dbSettings.fileName + "-API"].ToString();
    }

    #endregion Basics
}