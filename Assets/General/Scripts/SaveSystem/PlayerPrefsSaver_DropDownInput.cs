using TMPro;
using UnityEngine;

/// <summary>
/// Save the text of dropdown field and inputfield into PlayerPrefs
/// use this to save mobile contact in Registration Page
/// Tips: call SaveCombineField() to save the combined text to PlayerPrefs
/// </summary>
public class PlayerPrefsSaver_DropDownInput : PlayerPrefsSaver
{
    public TMP_Dropdown dropDown;
    public TMP_InputField inputField;

    public string prefix = "+6";

    public void SaveCombineField()
    {
        PlayerPrefs.SetString(name_, prefix + dropDown.options[dropDown.value].text + inputField.text);
    }
}
