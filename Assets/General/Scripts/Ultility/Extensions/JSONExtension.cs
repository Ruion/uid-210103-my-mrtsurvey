using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

public static class JSONExtension
{
    public static void SaveObject(string filePath, System.Object Object)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath + ".json", JsonConvert.SerializeObject(Object, Formatting.Indented));

    }

    public static void SaveSetting(string filePath, string pName, string pValue)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        if(!File.Exists(filePath + ".json")) File.WriteAllText(filePath + ".json", "{}");

        JObject jsonObj = LoadJson(filePath);
        
        if(!jsonObj.ContainsKey(pName))
        {
            jsonObj.Add(new JProperty(pName, pValue));
        }
        else
        {
            jsonObj[pName] = pValue;
        }
       
        File.WriteAllText(filePath + ".json", JsonConvert.SerializeObject(jsonObj, Formatting.Indented));
        Debug.Log(string.Format("Save to file {0}", filePath + ".json"));

    }

    public static string LoadSetting(string filePath, string pName)
    {
        JObject jsonObj = LoadJson(filePath);

        if(!jsonObj.ContainsKey(pName))
        {
            Debug.LogError("Property not exist in setting : " + pName);
            return null;
        }
        else
        {
            return jsonObj[pName].ToString();
        }
    }

    public static JObject LoadJson(string filePath)
    {
        string json = File.ReadAllText(filePath+".json");
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
            if(fields[i].FieldType == typeof(string))
            {
                fields[i].SetValue(Object, LoadSetting(filePath, fields[i].Name));
            }
            else if(fields[i].FieldType  == typeof(int))
            {
                fields[i].SetValue(Object, System.Int32.Parse(LoadSetting(filePath, fields[i].Name)));
            }
            else if(fields[i].FieldType  == typeof(bool))
            {
                fields[i].SetValue(Object, bool.Parse(LoadSetting(filePath, fields[i].Name)));
            }
           
        }
    }

    #endregion
}
