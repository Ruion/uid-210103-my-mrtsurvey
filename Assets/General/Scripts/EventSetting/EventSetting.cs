using System;
using System.Net;
using System.IO;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Configure the machine setting and save to file in streaming assets folder
/// </summary>
public class EventSetting : SerializedMonoBehaviour
{
    #region Fields & Action
    public EventSettings eventSettings;
    
    public GameObject internetConnectionHandler;
    public GameObject errorHandler;
    public TextMeshProUGUI sourceIdentifierDescription;
    public TextMeshProUGUI sourceIdentifierCode;
    public TMP_Dropdown sourceIdentifierDropdown;

    public TMP_InputField serverField;
    public List<EventFields> eventFields = new List<EventFields>();

    [TableList]
    public Dictionary<string, Button> buttons = new Dictionary<string, Button>();

    private EventCode[] options;

    public JSONSetter jsonSetter {get { return FindObjectOfType<JSONSetter>();} }

    #endregion

    void OnEnable(){
        internetConnectionHandler.SetActive(false);
        errorHandler.SetActive(false);
        LoadSettings();

        FetchServerOptions();
//        if(eventSettings.isVerify) eventSettings = (EventSettings)Data.LoadData(eventSettings.dataSettings.fullFilePath);
    }

    void RefreshSetting()
    {
        sourceIdentifierDescription.text = eventSettings.eventCode.description;
        sourceIdentifierCode.text = eventSettings.eventCode.code;
    }

#region SaveLoad
    [Button(ButtonSizes.Medium)]
    private void SaveSettings(){
       // eventSettings.dataSettings.fileFullName = eventSettings.dataSettings.fileName + "." + eventSettings.dataSettings.extension;
      //  eventSettings.dataSettings.fullFilePath = eventSettings.dataSettings.folderPath + "\\" + eventSettings.dataSettings.fileFullName;
       // Data.SaveData(eventSettings, eventSettings.dataSettings.fullFilePath);

        // JSONSetter jsonSetter = FindObjectOfType<JSONSetter>();
        JSONExtension.SaveObject(jsonSetter.savePath + "\\EventSetting", eventSettings);
        JSONExtension.SaveSetting(jsonSetter.savePath + "\\Setting", "source_identifier_code", eventSettings.eventCode.code);
        RefreshSetting();
    }

    [Button(ButtonSizes.Medium)]
    private void LoadSettings(){
       // eventSettings = (EventSettings)Data.LoadData(eventSettings.dataSettings.fullFilePath);
       // JSONSetter jsonSetter = FindObjectOfType<JSONSetter>();
       eventSettings = JsonConvert.DeserializeObject<EventSettings>(File.ReadAllText(jsonSetter.savePath + "\\EventSetting.json"));
        RefreshSetting();
    }
#endregion

    // verify from server
    [Button(ButtonSizes.Small)]
    public void FetchServerOptions(){
        string HtmlText = GetHtmlFromUri();
        if (HtmlText == "")
        {
            //No connection
            Debug.LogError("no internet connection");
            internetConnectionHandler.SetActive(true);
            return;
        }
        StartCoroutine(FetchServerOptionsRoutine());
    }

    private IEnumerator FetchServerOptionsRoutine(){

        string serverDomainURL = jsonSetter.LoadSetting()["serverDomainURL"].ToString();
        var substrings = new[] {"api"};
        if(!serverDomainURL.ContainsAny(substrings, StringComparison.CurrentCultureIgnoreCase))
            serverDomainURL += "/public/api/";
      //  using (UnityWebRequest www = UnityWebRequest.Get(serverField.text.Trim().Replace(" ", string.Empty))){
        using (UnityWebRequest www = UnityWebRequest.Get(serverDomainURL.Trim().Replace(" ", String.Empty) + "get-source-identifier-list")){
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
                errorHandler.SetActive(true);
                errorHandler.GetComponentInChildren<TextMeshProUGUI>().text = www.error;
                yield break;
            }
            else
            {
                while(!www.downloadHandler.isDone) yield return null;
                Debug.Log(www.downloadHandler.text);
                buttons["saveoptions"].gameObject.SetActive(true);
                sourceIdentifierDropdown.interactable = true;
                options = JsonHelper.getJsonArray<EventCode>(www.downloadHandler.text);
                
                string[] source_identifiers = new string[options.Length];
                for (int e = 0; e < options.Length; e++)
                {
                    source_identifiers[e] = options[e].description;
                }
                AddOptionToDropdown(source_identifiers, sourceIdentifierDropdown, "Source Identifier"); 
                
            }
        }
    }

    public void ResetFields()
    {
        foreach (EventFields f in eventFields)
        {
            if(f.field != null) f.field.text = "";
            if(f.dropdownfield != null) f.dropdownfield.ClearOptions();
        }
      //  buttons["url"].interactable = false;
      //  buttons["url"].gameObject.SetActive(true);

        buttons["saveoptions"].gameObject.SetActive(false);
        buttons["saveoptions"].interactable = false;
        
     //   buttons["done"].interactable = false;
     //   buttons["done"].gameObject.SetActive(false);
    }


    public void SaveSourceIdentifier()
    {
        eventSettings.eventCode = options
                .FirstOrDefault(o => o.description == sourceIdentifierDropdown.options[sourceIdentifierDropdown.value].text);

        eventSettings.isVerify = true;
        // save the selected 
        SaveSettings();
        PlayerPrefs.SetString("source_identifier_code", eventSettings.eventCode.code);
        // submitted selected code to server
        //StartCoroutine(SubmitSourceIdentifierRoutine(sourceIdentifierDropdown.options[sourceIdentifierDropdown.value].text));
    }

    ///////////// AFTER License Key Verify /////////////
    #region Private Method
    private void AddOptionToDropdown(string[] options, TMP_Dropdown dropdown, string firstOption = "Select")
    {
        dropdown.options.Clear();
        List<string> newOptions = options.ToList(); newOptions.Insert(0, firstOption);
        dropdown.AddOptions(newOptions);
    }

    private string GetHtmlFromUri(string resource = "http://google.com")
    {
        string html = string.Empty;
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resource);
        try
        {
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
                if (isSuccess)
                {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        //We are limiting the array to 80 so we don't have
                        //to parse the entire html document feel free to 
                        //adjust (probably stay under 300)
                        char[] cs = new char[80];
                        reader.Read(cs, 0, cs.Length);
                        foreach (char ch in cs)
                        {
                            html += ch;
                        }
                    }
                }
            }
        }
        catch
        {
            return "";
        }
        return html;
    }
    #endregion

    #region Field validation
        public void ValidateInputField(int fieldIndex)
        {
            bool isCorrect = true;
            if(eventFields[fieldIndex].isText)
            {
                if(!string.IsNullOrEmpty(eventFields[fieldIndex].field.text))
                {
                    isCorrect = Regex.IsMatch(eventFields[fieldIndex].field.text, eventFields[fieldIndex].regexPattern);
                }
                else isCorrect = false;
            }
            else
            {
                if(eventFields[fieldIndex].dropdownfield.value < 1) isCorrect = false;
            }
            
            if(isCorrect && eventFields[fieldIndex].successAction.GetPersistentEventCount() > 0 ) eventFields[fieldIndex].successAction.Invoke();

            if(!isCorrect && eventFields[fieldIndex].failAction.GetPersistentEventCount() > 0 ) eventFields[fieldIndex].failAction.Invoke();
        }
    #endregion
}

#region Objects
[Serializable]
public struct EventSettings
{
    [HideInPlayMode]
    public DataSettings dataSettings;
    public string sourceidentifierURL;
    public bool isVerify;

    public EventCode eventCode;
    
    public string selectedSourceIdentifier;
    public string mickey;
}

[Serializable]
public class EventFields
{
    public bool isText;
[ShowIf("isText", true)]  public TMP_InputField field;
[ShowIf("isText", true)]  public string regexPattern;
[HideIf("isText", true)]  public TMP_Dropdown dropdownfield;
[HorizontalGroup] public UnityEvent successAction;
[HorizontalGroup] public UnityEvent failAction;
}

[Serializable]
public class EventCode
{
    public string event_code;
    public string location;
    public string code;
    public string description;
}

#endregion