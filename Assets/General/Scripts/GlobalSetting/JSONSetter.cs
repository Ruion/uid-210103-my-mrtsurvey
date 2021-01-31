using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

public class JSONSetter : MonoBehaviour
{
    private string globalFileName = "GlobalSetting.json";
    private string fileName = "Setting.json";

    [FolderPath(AbsolutePath = true, UseBackslashes = true)]
    public string savePath { get { return LoadGlobalSettingFile(@"C:\UID-APP\GLOBAL")["projectFolder"].ToString(); } }

    [FolderPath(AbsolutePath = true, UseBackslashes = true)]
    public string globalSavePath;

    public void SaveSetting(JObject jsonObj, bool global = false)
    {
        string savePath = this.savePath;

        string fileName = this.fileName;

        if (global)
        {
            savePath = globalSavePath;
            fileName = this.globalFileName;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(savePath + "\\" + fileName));
        // write JSON directly to a file
        using (StreamWriter file = File.CreateText(savePath + "\\" + fileName))
        using (JsonTextWriter writer = new JsonTextWriter(file))
        {
            jsonObj.WriteTo(writer);
        }
    }

    public void UpdateSetting(string name, string value)
    {
        JObject jsonObj = LoadSetting();

        JToken jvalue;
        if (!jsonObj.TryGetValue(name, out jvalue) && !string.IsNullOrEmpty(value))
        //if (!jsonObj.ContainsKey(name) && !string.IsNullOrEmpty(value))
        {
            jsonObj.Add(new JProperty(name, value));
        }
        else
        {
            jsonObj[name] = value;
        }

        SaveSetting(jsonObj);
    }

    public void UpdateSettingGlobal(string name, string value)
    {
        JObject jsonObj = LoadSetting();

        JToken jvalue;
        if (!jsonObj.TryGetValue(name, out jvalue) && !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value))

        //if (!jsonObj.ContainsKey(name) && !string.IsNullOrEmpty(value))
        {
            jsonObj.Add(new JProperty(name, value));
        }
        else
        {
            jsonObj[name] = value;
        }

        SaveSetting(jsonObj, true);
    }

    public JObject LoadSetting(bool global = false)
    {
        string savePath = this.savePath;
        if (global)
        {
            savePath = globalSavePath;
            fileName = this.globalFileName;
        }

        string json = File.ReadAllText(savePath + "\\" + "Setting.json");
        JObject jsonObj = JObject.Parse(json);

        return jsonObj;
    }

    public JObject LoadGlobalSettingFile(string filePath)
    {
        string json = File.ReadAllText(filePath + "\\" + globalFileName);
        JObject jsonObj = JObject.Parse(json);

        return jsonObj;
    }
}