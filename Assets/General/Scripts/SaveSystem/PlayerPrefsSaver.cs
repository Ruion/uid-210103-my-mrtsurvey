using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Save the value of field / ScriptableScore / DataTime into PlayerPrefs
/// Tips: set the name_ in inspector, drag this component into UnityEvent 
/// and select which function to execute, and drop the TMP UI Object into the function field.
/// You can also call various Save() function via script.
/// Supported type : TMP_InputField, InputField, TexhMeshProUGUI, ScriptableScore, DateTime, string
/// Note: DataTime parameter require no parameter.
/// </summary>
public class PlayerPrefsSaver : MonoBehaviour
{
    public string name_;

    public void Save(InputField inputField)
    {
        PlayerPrefs.SetString(name_, inputField.text.ToString());
    }

    public void Save(TMP_InputField inputField)
    {
        PlayerPrefs.SetString(name_, inputField.text.ToString());
    }

    public void Save(TextMeshProUGUI text_)
    {
        PlayerPrefs.SetString(name_, text_.text.ToString());
    }

    public void Save(string value)
    {
        PlayerPrefs.SetString(name_, value);
    }

    public void Save(ScriptableScore scoreCard)
    {
        PlayerPrefs.SetString(name_, scoreCard.score.ToString());
    }

    [ContextMenu("DateTime")]
    public void SaveDateTime()
    {
        PlayerPrefs.SetString(name_, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        Debug.Log(PlayerPrefs.GetString(name_));
       // Debug.Log(System.DateTime.UtcNow);
    }
}

