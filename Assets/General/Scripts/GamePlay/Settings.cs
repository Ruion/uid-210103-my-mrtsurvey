using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Settings to be used by gameplay objects
/// add variable like speed, hp, game time and make other class to use those values, to made tweaking and testing easier.
/// </summary>
[System.Serializable]
public class Settings
{
    // public string scoreName = "game_score";
    public int gameTime = 25;

    public bool debugMode = false;

    public string serverDomainURL;
    public string voucherCodeDownloadAPI;
    public string userPrimaryKeyName = "userPrimaryKey";
    public bool realTimeOnlineValidate = true;
    public int checkInternetTimeOut = 5000;
    public int downloadCodeAPITimeOut = 10;
    public int tier1Score = 1;
    public int tier2Score = 2;
    public int tier3Score = 7;

    public Settings(Settings setting)
    {
        //  Debug.Log(setting.savePath);
        //   savePath = setting.savePath;

        //   DebugMode = setting.DebugMode;

        //  fileName = setting.fileName;
        //   scoreName = setting.scoreName;
        //   scoreToWin = setting.scoreToWin;
    }
}