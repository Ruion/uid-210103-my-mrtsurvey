using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using System.Collections.Generic;

/// <summary>
/// Save the form field, string value into PlayerPrefs. 
/// Saving type supported are TMP_InputField, TMP_Dropdown, InputField, TextMeshProUGUI, DateTime, manual set string
/// </summary>
public class PlayerPrefsSave_Group : SerializedMonoBehaviour
{
   // [TableList]
    public DataField[] dataFields;

    [TableList][DictionaryDrawerSettings(KeyLabel = "Name", ValueLabel = "Value")]
    [DisableInEditorMode] public Dictionary<string,string> playerprefs;

    [Button(ButtonSizes.Large,ButtonStyle.CompactBox)]
    public void SaveAll()
    {
        for (int d = 0; d < dataFields.Length; d++)
        {
            Save(dataFields[d]);
        }

        playerprefs = new Dictionary<string, string>();
        for (int p = 0; p < dataFields.Length; p++)
        {
            playerprefs.Add(dataFields[p].name_, dataFields[p].value_);
        }
    }

    private void Save(DataField df)
    {
        PlayerPrefs.SetString(df.name_, df.GetValue());
    }

}

[System.Serializable]
public class DataField 
{
    [HorizontalGroup("Field", .5f, LabelWidth = 60)]
    [BoxGroup("Field/Parameter")]
    public string name_;

    [BoxGroup("Field/Parameter")]
    public SaveType savetype = SaveType.InputField_TMP;

    [HideLabel]
    [BoxGroup("Field/Fields")]
    [ShowIf("savetype", SaveType.Text, false)]
    public TextMeshProUGUI textMeshProUGUI;

    [HideLabel][BoxGroup("Field/Fields")][HideIf("savetype", SaveType.DateTime, false)][DisableIf("@savetype==SaveType.Manual", false)]
    public TMP_Dropdown dropdown;

    [HideLabel]
    [BoxGroup("Field/Fields")]
    [ShowIf("savetype", SaveType.InputField_TMP,false)] 
    public TMP_InputField inputField_TMP;

    [HideLabel]
    [BoxGroup("Field/Fields")]
    [ShowIf("savetype", SaveType.InputField, false)]
    public InputField inputField;

    [BoxGroup("Field/Fields")][EnableIf("savetype", SaveType.Manual)]
    public string value_;



    public string GetValue()
    {
        
        string dropDownValue = "";
        if (dropdown != null && savetype != SaveType.DateTime && savetype != SaveType.Manual) dropDownValue = dropdown.options[dropdown.value].text;
        
        switch (savetype)
        {
            case SaveType.InputField_TMP:
                value_ = dropDownValue + inputField_TMP.text;
                break;

            case SaveType.InputField:
                value_ = dropDownValue + inputField.text;
                break;

            case SaveType.DateTime:
                value_ = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                break;

            case SaveType.Text:
                value_ = textMeshProUGUI.text;
                break;

            case SaveType.Manual:
                break;

            case SaveType.GlobalSetting:
             JSONSetter jsonSetter = GameObject.FindObjectOfType<JSONSetter>();
             value_ = JSONExtension.LoadSetting(jsonSetter.savePath + "\\Setting", name_);
            break;

        }

        return value_;
    }


}

public enum SaveType
{
    InputField_TMP = 1,
    DateTime = 3,
    InputField = 4,
    Manual = 5,
    Text = 6,
    GlobalSetting = 7,
}
