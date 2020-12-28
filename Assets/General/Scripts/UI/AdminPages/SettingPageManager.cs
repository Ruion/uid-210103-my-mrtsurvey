using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using TMPro;
using System.Linq;

public class SettingPageManager : MonoBehaviour
{
    #region Public Fields

    public JSONSetter jsonSetter;
    public GameObject settingFieldPrefab;
    public Transform settingFieldContainer;

    public GameObject popUpModal;
    public Button doneButton;

    public TextMeshProUGUI selectedField_
    {
        get { return selectedField; }
        set
        {
            selectedField = value;
            propertyTitle.text = selectedField.text;
            valueField.text =
            jsonSetter.LoadSetting()[properties.FirstOrDefault(f => f.Equals(selectedField.text, System.StringComparison.InvariantCultureIgnoreCase))].ToString();
            popUpModal.SetActive(true);
        }
    }

    public TextMeshProUGUI propertyTitle;
    public TMP_InputField valueField;

    #endregion Public Fields

    #region Private Fields

    private TextMeshProUGUI selectedField;
    private List<string> properties = new List<string>();

    #endregion Private Fields

    [ContextMenu("LoadGlobaSettings")]
    private void OnEnable()
    {
        if (jsonSetter == null) jsonSetter = FindObjectOfType<JSONSetter>();

        if (settingFieldContainer.childCount < 1)
        {
            JObject globalSettings = jsonSetter.LoadSetting();

            foreach (JProperty p in globalSettings.Properties())
            {
                GameObject newField = Instantiate(settingFieldPrefab, settingFieldContainer);

                // assign property name
                TextMeshProUGUI property = newField.GetComponent<TextMeshProUGUI>();
                property.text = StringExtensions.FirstCharToUpper(p.Name);
                properties.Add(p.Name);

                // assign On Click () event ot btn
                Button btn = newField.GetComponentInChildren<Button>();
                btn.onClick.AddListener(delegate
                {
                    selectedField_ = btn.GetComponentInParent<TextMeshProUGUI>();
                });

                // assign value to text
                TextMeshProUGUI valueText = btn.GetComponentInChildren<TextMeshProUGUI>();
                valueText.text = p.Value.ToString();
            }
        }
    }

    private void RefreshSetting()
    {
        //Clear all fields
        for (int i = 0; i < settingFieldContainer.childCount; i++)
        {
            Destroy(settingFieldContainer.GetChild(i).gameObject);
        }

        JObject globalSettings = jsonSetter.LoadSetting();

        foreach (JProperty p in globalSettings.Properties())
        {
            GameObject newField = Instantiate(settingFieldPrefab, settingFieldContainer);

            // assign property name
            TextMeshProUGUI property = newField.GetComponent<TextMeshProUGUI>();
            property.text = StringExtensions.FirstCharToUpper(p.Name);
            properties.Add(p.Name);

            // assign On Click () event ot btn
            Button btn = newField.GetComponentInChildren<Button>();
            btn.onClick.AddListener(delegate
            {
                selectedField_ = btn.GetComponentInParent<TextMeshProUGUI>();
            });

            // assign value to text
            TextMeshProUGUI valueText = btn.GetComponentInChildren<TextMeshProUGUI>();
            valueText.text = p.Value.ToString();
        }
    }

    public void UpdateProperty()
    {
        // jsonSetter.UpdateSetting(selectedField.text, valueField.text);
        jsonSetter.UpdateSetting(StringExtensions.FirstCharToLower(selectedField.text), valueField.text);

        // RefreshSetting();
        selectedField.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text
         = jsonSetter.LoadSetting()[StringExtensions.FirstCharToLower(selectedField.text)].ToString();
    }

    public void ValidateField()
    {
        if (string.IsNullOrEmpty(valueField.text)) doneButton.interactable = false;
        else doneButton.interactable = true;
    }
}