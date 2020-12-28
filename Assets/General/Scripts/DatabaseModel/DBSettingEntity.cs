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
       LoadSetting();
    }

    [Button(ButtonSizes.Large), GUIColor(.3f, .78f, .78f)][ButtonGroup("Setting")]
    public virtual void SaveSetting()
    {
            
        dbSettings.fileName = name;
        dbSettings.tableName = name;
        dbSettings.dbName = name;
        dbSettings.keyFileName = name + " Online";

        // fetch & Update setting from Setting.json
        JSONSetter jsonSetter = FindObjectOfType<JSONSetter>();
        dbSettings.folderPath = jsonSetter.savePath;

        //  dbSettings.keyDownloadURL = dbSettings.sendURL + ;

        Directory.CreateDirectory(Path.GetDirectoryName(dbSettings.folderPath + "\\Databases\\"));

        // add api to Setting.json - playerdata_sendAPI : submit-player-data
        DBSettingEntity[] dBSettingEntities = FindObjectsOfType<DBSettingEntity>();
        foreach (DBSettingEntity e in dBSettingEntities)
        {
            if(string.IsNullOrEmpty(e.dbSettings.sendAPI)) continue;
            
            jsonSetter.UpdateSetting(e.dbSettings.fileName+ "-API", e.dbSettings.sendAPI);
        }

        // legacy binary formatter save method
        // DBSetting.SaveSetting(dbSettings);

        

        // save to json file
        JSONExtension.SaveObject(dbSettings.folderPath + "\\" + name, dbSettings);
    }

    [Button(ButtonSizes.Large), GUIColor(.3f, .78f, .78f)][ButtonGroup("Setting")]
    public virtual void LoadSetting()
    {

        JSONSetter jsonSetter = FindObjectOfType<JSONSetter>();

        dbSettings.folderPath = jsonSetter.savePath;
        string filePath = dbSettings.folderPath + "\\" + name;


        // Load from json file
        dbSettings = JsonConvert.DeserializeObject<DBEntitySetting>(File.ReadAllText(filePath + ".json"));

        // fetch filePath from jsonSetter
        dbSettings.folderPath = jsonSetter.savePath;


        // fetch & Update setting from global JSONSetter
        JObject jObject = jsonSetter.LoadSetting();
        dbSettings.sendURL = jObject["serverDomainURL"].ToString();
        dbSettings.keyDownloadAPI = jObject["voucherCodeDownloadAPI"].ToString();

        var substrings = new[] {"api"};
        if(!dbSettings.sendURL.ContainsAny(substrings, StringComparison.CurrentCultureIgnoreCase))
            dbSettings.sendURL += "/public/api/";

        // load sendAPI from global setting file
        if(jObject.ContainsKey(dbSettings.fileName+"-API")) dbSettings.sendAPI = jObject[dbSettings.fileName+"-API"].ToString();
    }
    #endregion

}
