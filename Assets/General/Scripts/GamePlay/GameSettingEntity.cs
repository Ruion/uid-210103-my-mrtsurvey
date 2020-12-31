using UnityEngine;

/// <summary>
/// Master class for Saving and Retrieving gameplay settings.
/// Tips: Inherit this class to access same game setting in different scenes, extend the variable in Settings{} class as you like
/// By default gameObject inherit this class will execute LoadGameSettingFromMaster() in Awake()
/// </summary>
public class GameSettingEntity : MonoBehaviour
{
    //  [Header("GameSetting - SAVE setting every new project")]
    public Settings gameSettings;

    public JSONSetter jsonSetter;

    public string Server_URL { get { return JSONExtension.LoadEnv("SERVER_URL").ToString(); } }

    public string Project_Folder
    {
        get
        {
            return @"C:\UID-APP\" + JSONExtension.LoadEnv("PROJECT_CODE").ToString();
        }
    }

    public string SettingFilePath
    {
        get
        {
            return @"C:\UID-APP\" + JSONExtension.LoadEnv("PROJECT_CODE").ToString() + "\\Settings\\Setting";
        }
    }

    /// <summary>
    /// Save the settings value in to file. This function eccesible by right click component in inspector and click "SaveSetting"
    /// </summary>
    [ContextMenu("SaveSetting")]
    public virtual void SaveSetting()
    {
        JSONExtension.SaveValues(SettingFilePath, gameSettings);

        Debug.Log(name + " Save setting success");
    }

    /// <summary>
    /// Load the settings value from file. This function eccesible by right click component in inspector and click "LoadSetting"
    /// </summary>
    [ContextMenu("LoadSetting")]
    public virtual void LoadSetting()
    {
        JSONExtension.LoadValues(SettingFilePath, gameSettings);
    }

    /// <summary>
    /// Load settings from a gameObject call "GameSettingEntity_DoNotChangeName"
    /// </summary>
    [ContextMenu("LoadMasterSetting")]
    public virtual void LoadGameSettingFromMaster()
    {
        GameSettingEntity dm = GameObject.Find("GameSettingEntity_DoNotChangeName").GetComponent<GameSettingEntity>();
        if (dm == this) return;

        dm.LoadSetting();
        gameSettings = dm.gameSettings;
    }

    [ContextMenu("Awake")]
    protected virtual void Awake()
    {
        //if (jsonSetter == null) jsonSetter = FindObjectOfType<JSONSetter>();
        LoadSetting();
        //LoadGameSettingFromMaster();
    }
}

[System.Serializable]
public struct SettingKey
{
    public SettingType type;
    public string value;
}

public enum SettingType
{
    text,
    number,
    boolean
}