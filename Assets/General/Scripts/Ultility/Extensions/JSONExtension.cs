using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

public static class JSONExtension
{
    public static string SERVER_URL { get { return LoadEnv("SERVER_URL").ToString(); } }

    public static string PROJECT_FOLDER
    {
        get
        {
            return @"C:\UID-APP\" + LoadEnv("PROJECT_CODE").ToString();
        }
    }

    public static string SETTING_FILE_PATH
    {
        get
        {
            return @"C:\UID-APP\" + LoadEnv("PROJECT_CODE").ToString() + "\\Settings\\Setting";
        }
    }

    public static void SaveObject(string filePath, System.Object Object)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath + ".json", JsonConvert.SerializeObject(Object, Formatting.Indented));
    }

    public static void SaveSetting(string filePath, string pName, string pValue)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        if (!File.Exists(filePath + ".json")) File.WriteAllText(filePath + ".json", "{}");

        JObject jsonObj = LoadJson(filePath);

        JToken value;
        if (jsonObj.TryGetValue(pName, out value))
            jsonObj.Add(new JProperty(pName, pValue));

        //if (!jsonObj.ContainsKey(pName))
        //jsonObj.Add(new JProperty(pName, pValue));
        else
            jsonObj[pName] = pValue;

        File.WriteAllText(filePath + ".json", JsonConvert.SerializeObject(jsonObj, Formatting.Indented));
        Debug.Log(string.Format("Save to file {0}", filePath + ".json"));
    }

    public static string LoadSetting(string filePath, string pName)
    {
        if (!filePath.Contains(".json"))
            filePath += ".json";

        JObject jsonObj = LoadJson(filePath);

        JToken value;
        if (!jsonObj.TryGetValue(pName, out value))
        //jsonObj.Add(new JProperty(pName, pValue));
        //if (!jsonObj.ContainsKey(pName))
        {
            Debug.LogError("Property not exist in setting : " + pName);
            return null;
        }
        else
        {
            return jsonObj[pName].ToString();
        }
    }

    public static void SaveEnv(string pName, string pValue)
    {
        JObject jsonObj = LoadJson(@"C:\UID-APP\env.json");

        JToken value;
        if (jsonObj.TryGetValue(pName, out value))
            //if (!jsonObj.ContainsKey(pName))
            jsonObj.Add(new JProperty(pName, pValue));
        else
            jsonObj[pName] = pValue;

        File.WriteAllText(@"C:\UID-APP\env.json", JsonConvert.SerializeObject(jsonObj, Formatting.Indented));
    }

    public static string LoadEnv(string pName)
    {
        JObject jsonObj = LoadJson(@"C:\UID-APP\env.json");

        JToken value;
        if (!jsonObj.TryGetValue(pName, out value))
        //if (!jsonObj.ContainsKey(pName))
        {
            Debug.LogError("Property not exist in setting : " + pName);
            return null;
        }
        else
        {
            return jsonObj[pName].ToString();
        }
    }

    public static int LoadEnvInt(string pName)
    {
        JObject jsonObj = LoadJson(@"C:\UID-APP\env.json");

        JToken value;
        if (!jsonObj.TryGetValue(pName, out value))

        //if (!jsonObj.ContainsKey(pName))
        {
            Debug.LogError("Property not exist in setting : " + pName);
            return 999999;
        }
        else
        {
            return System.Int32.Parse(jsonObj[pName].ToString());
        }
    }

    public static bool LoadEnvBool(string pName)
    {
        JObject jsonObj = LoadJson(@"C:\UID-APP\env.json");

        JToken value;
        if (!jsonObj.TryGetValue(pName, out value))

        //if (!jsonObj.ContainsKey(pName))
        {
            Debug.LogError("Property not exist in setting : " + pName);
            return false;
        }
        else
        {
            return bool.Parse(jsonObj[pName].ToString());
        }
    }

    public static JObject LoadJson(string filePath)
    {
        //string json = File.ReadAllText(filePath+".json");
        if (!filePath.Contains(".json"))
            filePath += ".json";
        string json = File.ReadAllText(filePath);
        JObject jsonObj = JObject.Parse(json);

        return jsonObj;
    }

    #region Utilities

    public static void SaveValues(string filePath, System.Object Object)
    {
        FieldInfo[] pros = Object.GetType().GetFields();
        for (int i = 0; i < pros.Length; i++)
        {
            SaveSetting(filePath, pros[i].Name, pros[i].GetValue(Object).ToString());
        }
    }

    public static void LoadValues(string filePath, System.Object Object)
    {
        FieldInfo[] fields = Object.GetType().GetFields();

        for (int i = 0; i < fields.Length; i++)
        {
            if (fields[i].FieldType == typeof(string))
            {
                fields[i].SetValue(Object, LoadSetting(filePath, fields[i].Name));
            }
            else if (fields[i].FieldType == typeof(int))
            {
                fields[i].SetValue(Object, System.Int32.Parse(LoadSetting(filePath, fields[i].Name)));
            }
            else if (fields[i].FieldType == typeof(bool))
            {
                fields[i].SetValue(Object, bool.Parse(LoadSetting(filePath, fields[i].Name)));
            }
        }
    }

    #endregion Utilities
}