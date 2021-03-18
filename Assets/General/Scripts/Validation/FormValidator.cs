using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections;
using TMPro;
using Sirenix.OdinInspector;
using System.IO;
using UnityEngine.Events;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System;

/// <summary>
/// Validate the user input in Registration Page.
/// Validate Type support : text, email.
/// Notes: By default it get "email" and "contact" list from DBModelEntity to check email, contact duplication
/// </summary>
public class FormValidator : MonoBehaviour
{
    #region variables

    public bool useButton = true;

    [ShowIf("useButton", false)]
    public Button Submit;

    public bool useToggle = true;

    [ShowIf("useToggle", false)]
    public Toggle consent;

    [TableList]
    public List<FormField> formFields = new List<FormField>();

    private FormField invalidField;

    public UnityEvent onValidatePass;
    public UnityEvent onValidateNotPass;
    private bool validatePass;

    public string secretKey = "uidpaneladmin";
    public UnityEvent onSecretKeyDetected;
    public UnityEvent onSecretInCorrect;

    #endregion variables

    private void Start()
    {
        string networkValidationURL = JSONExtension.LoadEnv("NETWORK_VALIDATION_URL");
        for (int i = 0; i < formFields.Count; i++)
        {
            formFields[i].Initialize();

            if (!string.IsNullOrEmpty(formFields[i].networkValidationAPI))
                formFields[i].networkValidationURL = networkValidationURL + "/" + formFields[i].networkValidationAPI;

            formFields[i].onFinishValidate.AddListener(ValidateFields);
        }
    }

    public void ValidateFields()
    {
        if (!gameObject.activeInHierarchy || enabled == false) return;

        invalidField = formFields.FirstOrDefault(f => f.isValid == false);

        if (invalidField == null || invalidField == default)
        {
            if (useButton)
                Submit.interactable = true;

            if (!validatePass)
            {
                validatePass = true;

                for (int i = 0; i < formFields.Count; i++)
                {
                    if (formFields[i].saveValueToPlayerPrefs)
                        PlayerPrefs.SetString(formFields[i].fieldName, formFields[i].processedValue);
                }
            }
            //Debug.Log(name + " Validation pass", gameObject);
            if (onValidatePass.GetPersistentEventCount() > 0) onValidatePass.Invoke();
        }
        else
        {
            if (useButton)
                Submit.interactable = false;

            if (onValidateNotPass.GetPersistentEventCount() > 0) onValidateNotPass.Invoke();
            string consent = invalidField.inputType == SaveType.Consent ? invalidField.consent.isOn.ToString() : ".";
            //Debug.Log(name + " " + invalidField.fieldName + " validation not pass : " + invalidField.isValid + $" {consent}", gameObject);
        }
    }

    public void DetectAdminPassword(InputField input)
    {
        if (gameObject.activeSelf) return;

        if (input.text.Length != 20) return;

        if (input.text.DecodeBase64() == secretKey)
            onSecretKeyDetected.Invoke();
        else
            onSecretInCorrect.Invoke();
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}

[System.Serializable]
public class FormField
{
    #region Fields

    [TabGroup("Field")]
    //[HorizontalGroup("Fields")]
    //[VerticalGroup("Fields/Left")]
    [PropertyTooltip("Label the item with a name. This is important to make the set up clear and easier to manage. Database Model will take coulmn from \"Field Name\" to compare when Is Unique is tick.")]
    public string fieldName;

    [TabGroup("Field")]
    //[VerticalGroup("Fields/Left")]
    public SaveType inputType = SaveType.InputField_TMP;

    [TabGroup("Field")]
    [ShowIf("inputType", SaveType.InputField_TMP, false)]
    //[VerticalGroup("Fields/Left")]
    public TMP_InputField textField;

    [TabGroup("Field")]
    [ShowIf("inputType", SaveType.InputField, false)]
    //[VerticalGroup("Fields/Left")]
    public InputField inputField;

    [TabGroup("Field")]
    [ShowIf("inputType", SaveType.Consent, false)]
    //[VerticalGroup("Fields/Left")]
    public Toggle consent;

    [TabGroup("Field")]
    [HideIf("inputType", SaveType.Consent, false)]
    //[VerticalGroup("Fields/Left")]
    [LabelText("DropDown (optional)")]
    [PropertyTooltip("Provide optional dropdown input. The final value will become DropDown + Text Field. Example : dropdown value + abc")]
    public TMP_Dropdown dropDown;

    [TabGroup("Field")]
    [HideIf("inputType", SaveType.Consent, false)]
    //[VerticalGroup("Fields/Left")]
    [LabelText("RegexPattern (optional)")]
    [PropertyTooltip(@"Regex pattern that validate value of Text Field. When dropdown is provided, regext will validate value of Drop Down + Text Field")]
    public string regexPattern;

    [TabGroup("Field")]
    [HideIf("inputType", SaveType.Consent, false)]
    //[VerticalGroup("Fields/Left")]
    [LabelText("RegexPattern After Format(optional)")]
    [PropertyTooltip(@"Regex pattern that validate value of Text Field. When dropdown is provided, regext will validate value of Drop Down + Text Field")]
    public string regexPatternAfterFormat;

    [TabGroup("Field")]
    [HideIf("inputType", SaveType.Consent, false)]
    //[VerticalGroup("Fields/Right")]
    [PropertyTooltip("Tick to check for duplication. You must provide either a Database Model component or Data txt file")]
    public bool isUnique = false;

    [TabGroup("Field")]
    //[VerticalGroup("Fields/Right")]
    public bool useValidator = false;

    [TabGroup("Field")]
    [ShowIf("useValidator")]
    //[VerticalGroup("Fields/Right")]
    [PropertyTooltip("Tick to check for duplication. You must provide either a Database Model component or Data txt file")]
    public DuplicateValidator validator;

    [TabGroup("Field")]
    [HideIf("inputType", SaveType.Consent, false)]
    [ShowIf("isUnique")]
    //[VerticalGroup("Fields/Right")]
    [LabelText("Database Model (optional)")]
    [PropertyTooltip("By providing Database Model Component, duplicate checking will check column in database name with Field Name. For example, Field Name is contact, validation will check \"contact\" column in database")]
    public DBModelMaster dbmodel;

    [TabGroup("Field")]
    [HideIf("inputType", SaveType.Consent, false)]
    [ShowIf("isUnique")]
    //[VerticalGroup("Fields/Right")]
    [FilePath(AbsolutePath = true)]
    [LabelText("Data txt file (optional)")]
    [PropertyTooltip(@"By providing full txt file path, checking will include lines of string inside txt file. Example: C:\UID_Toolkit\output\player_email_list.txt")]
    public string serverDataFilePath;

    [TabGroup("Field")]
    [HideIf("inputType", SaveType.Consent, false)]
    [ShowIf("isUnique")]
    //[VerticalGroup("Fields/Right")]
    [PropertyTooltip("This object will be display when duplicate detected")]
    public GameObject duplicateWarning;

    [TabGroup("Field")]
    [HideIf("inputType", SaveType.Consent, false)]
    //[VerticalGroup("Fields/Right")]
    [PropertyTooltip("Allow the input to use once. When column \"status\" is \"ready\", it will be valid. When it is \"redeemed\", will validate as duplicate")]
    public bool checkMatching = false;

    [TabGroup("Field")]
    [HideIf("inputType", SaveType.Consent, false)]
    [ShowIf("checkMatching")]
    //[VerticalGroup("Fields/Right")]
    [LabelText("Matching Database Model (allow code use once)")]
    [PropertyTooltip("By providing Database Model Component, validation will check the field is match with record in database. If \"card_number\" is match with \"card_number\" data in database, the field is valid")]
    public DBModelMaster matchingDatabase;

    [TabGroup("Field")]
    //[VerticalGroup("Fields/Left")]
    [PropertyTooltip("This object will be display when field is valid")]
    public GameObject validMarker;

    [TabGroup("Field")]
    //[VerticalGroup("Fields/Left")]
    [PropertyTooltip("This object will be display when field is invalid")]
    public GameObject invalidMarker;

    [TabGroup("Field")]
    [HideIf("inputType", SaveType.Consent, false)]
    //[VerticalGroup("Fields/Right")]
    [PropertyTooltip("Only validate number in the input.")]
    public bool ignoreAlphabet = false;

    [TabGroup("Field")]
    [HideIf("inputType", SaveType.Consent, false)]
    //[VerticalGroup("Fields/Right")]
    [PropertyTooltip("Only validate number in the input.")]
    public string[] ignoreCharactersOnValidation;

    [TabGroup("Field")]
    //[VerticalGroup("Fields/Right")]
    public bool saveValueToPlayerPrefs;

    [TabGroup("Events")]
    [ShowIf("isUnique")]
    //[VerticalGroup("Fields/Right")]
    public UnityEvent onDuplicate;

    [TabGroup("Events")]
    //[VerticalGroup("Fields/Right")]
    public UnityEvent onInvalid;

    [TabGroup("Events")]
    //[VerticalGroup("Fields/Right")]
    public UnityEvent onValid;

    [HideInInspector]
    public string networkValidationURL;

    [TabGroup("Field")]
    //[VerticalGroup("Fields/Right")]
    [Tooltip("key in the api name for validate this field by network, and fill in network url at Setting.json in ProjectFolder/Settings")]
    public string networkValidationAPI;

    private HttpClient webRequest;

    [TabGroup("Field")]
    public FormValidatorFormatter valueFormatter;

    [TabGroup("Events")]
    //[VerticalGroup("Fields/Left")]
    public UnityEvent onStartValidation;

    [TabGroup("Events")]
    //[VerticalGroup("Fields/Left")]
    public UnityEvent onFinishValidate;

    [HideInInspector] public bool isValid;
    [HideInInspector] public bool isDuplicated;
    private List<string> dataList = new List<string>();
    private bool initialize;

    private bool isValidating;

    [TabGroup("Field")]
    //[VerticalGroup("Fields/Left")]
    [SerializeField] private TimeManager tm;

    [TabGroup("Field")]
    //[VerticalGroup("Fields/Left")]
    public int maxCharacter = 80;

    public string value
    {
        get
        {
            if (dropDown != null)
            {
                if (inputType == SaveType.InputField)
                    return dropDown.options[dropDown.value].text + inputField.text;
                else
                    return dropDown.options[dropDown.value].text + textField.text;
            }
            else
            {
                if (inputType == SaveType.InputField)
                    return inputField.text;
                else
                    return textField.text;
            }
        }
    }

    public string processedValue
    {
        get
        {
            if (inputType == SaveType.Consent)
                return consent.isOn.ToString();

            string newValue = value;

            if (ignoreAlphabet)
                newValue = StringExtensions.RemoveAlphabets(value);

            if (ignoreCharactersOnValidation.Length > 0)
                newValue = StringExtensions.RemoveCharacters(value, ignoreCharactersOnValidation);

            return newValue;
        }
    }

    #endregion Fields

    public void Initialize()
    {
        if (initialize) return;
        initialize = true;

        if (tm != null)
            tm.countdownEndEvents.AddListener(delegate { OnValueChange(); });

        if (dropDown != null)
            dropDown.onValueChanged.AddListener(delegate { OnValueChange(); });

        if (inputType == SaveType.InputField_TMP)
        {
            if (tm == null)
            {
                textField.onValueChanged.AddListener(delegate { OnValueChange(); });
                textField.onDeselect.AddListener(delegate { OnValueChange(); });
            }
            else
            {
                textField.onValueChanged.AddListener(delegate { StartValidateTimer(); });
                textField.onDeselect.AddListener(delegate { StartValidateTimer(); });
            }
        }

        if (inputType == SaveType.InputField)
            if (tm == null)
                inputField.onValueChanged.AddListener(delegate { OnValueChange(); });
            else
                inputField.onValueChanged.AddListener(delegate { StartValidateTimer(); });

        if (inputType == SaveType.Consent)
            consent.onValueChanged.AddListener(delegate { OnValueChange(); });

        if (useValidator) return;

        if (isUnique)
        {
            if (serverDataFilePath != "")
                dataList = GetDataFromTextFile(serverDataFilePath);

            if (dbmodel != null)
                // add distinct item that not exist in dataList
                if (dataList.Count > 0)
                    dataList.AddRange(dbmodel.GetDataByStringToList(fieldName).Except(dataList));
                else
                    dataList.AddRange(dbmodel.GetDataByStringToList(fieldName));
        }
    }

    private List<string> GetDataFromTextFile(string filePath)
    {
        List<string> textList = new List<string>();

        string[] lines = File.ReadAllLines(filePath);

        // add emails to list
        foreach (string line in lines)
        {
            textList.Add(line.ToString());
        }

        return textList;
    }

    private void StartValidateTimer()
    {
        tm.ResetCountDown();
        tm.StartGame();
    }

    public async void OnValueChange()
    {
        if (onStartValidation.GetPersistentEventCount() > 0) onStartValidation.Invoke();

        if (!string.IsNullOrEmpty(networkValidationURL) && webRequest != null)
            webRequest.CancelPendingRequests();

        if (duplicateWarning != null)
            duplicateWarning.SetActive(false);

        if (inputType == SaveType.Consent)
        {
            isValid = consent.isOn;
        }
        else
        {
            isDuplicated = false;

            if (inputType == SaveType.InputField_TMP)
                // check empty string
                isValid = !string.IsNullOrEmpty(textField.text) && (processedValue.Length <= maxCharacter);

            if (inputType == SaveType.InputField)
                // check empty string
                isValid = !string.IsNullOrEmpty(inputField.text) && (processedValue.Length <= maxCharacter);

            if (isValid && !string.IsNullOrEmpty(regexPattern))
                // check regex match
                //isValid = Regex.IsMatch(processedValue, regexPattern);
                isValid = Regex.Matches(processedValue, regexPattern).Count == 1;

            if (valueFormatter != null && isValid)
            {
                string valiedateProcessedValue = valueFormatter.FormatTextForValidate(processedValue);
                if (!string.IsNullOrEmpty(regexPatternAfterFormat)) isValid = Regex.Matches(valiedateProcessedValue, regexPatternAfterFormat).Count >= 1;

                //if (!isValid) Debug.LogError($"{fieldName} condition not match 2nd regex - {valiedateProcessedValue}");
            }

            if (useValidator)
            {
                if (isValidating) { isValidating = false; return; }
                if (isValid)
                {
                    isValidating = true;
                    inputField.interactable = false;

                    isValid =
                    await validator.ValidateAsync(value, async () =>
                    {
                        await ValidationAction();
                    });
                    return;
                }
            }
            else
            {
                // string is not empty and match
                if (isValid)
                {
                    if (checkMatching)
                    {
                        string query = $"SELECT {fieldName} FROM {matchingDatabase.dbSettings.tableName} WHERE {fieldName} IN ('{processedValue}')";
                        object status = matchingDatabase.ExecuteCustomSelectObject(query);
                        if (status == null)
                        {
                            Debug.Log($"matching not found : {value}");
                            isValid = false;
                            return;
                        }
                    }

                    // Check duplication
                    if (isUnique)
                    {
                        // check duplicate from txt
                        string same = dataList.FirstOrDefault(t => t == processedValue);

                        string networkResponse;

                        if (string.IsNullOrEmpty(same))
                            same = dbmodel.ExecuteCustomSelectSingle($"SELECT {fieldName} FROM {dbmodel.dbSettings.tableName} WHERE {fieldName} = '{processedValue}'")[0].ToString();

                        if (!string.IsNullOrEmpty(same))
                        {
                            Debug.Log($"local validation - {same} is use");
                            isDuplicated = true;
                            isValid = false;
                        }

                        if (!isDuplicated)
                        {
                            if (!string.IsNullOrEmpty(networkValidationURL))
                            {
                                HttpClient webRequest = new HttpClient();

                                networkResponse = await webRequest.GetStringAsync(networkValidationURL + "/" + processedValue);
                                if (System.Convert.ToBoolean(networkResponse) != true)
                                {
                                    isDuplicated = true;
                                    isValid = false;
                                    Debug.Log($"network validation {processedValue} is duplicated");
                                }
                            }
                        }
                        DuplicateAction();
                    }
                }
            }
        }

        isValidating = false;
        await ValidationAction();
    }

    private async Task ValidationAction()
    {
        await Task.Delay(JSONExtension.LoadEnvInt("VALIDATE_ACTION_DELAY")).ContinueWith((task) =>
        {
            if (!isValid)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    if (invalidMarker != null)
                        invalidMarker.SetActive(true);

                    if (validMarker != null)
                        validMarker.SetActive(false);

                    if (onInvalid.GetPersistentEventCount() > 0)
                        onInvalid.Invoke();

                    //Debug.Log(fieldName + " invalid");
                });
            }
            else
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    if (invalidMarker != null)
                        invalidMarker.SetActive(false);

                    if (validMarker != null)
                        validMarker.SetActive(true);

                    if (onValid.GetPersistentEventCount() > 0)
                        onValid.Invoke();
                    //Debug.Log(fieldName + " valid");
                });
            }
            UnityMainThreadDispatcher.Instance().Enqueue(() => { onFinishValidate.Invoke(); });
        });
    }

    private void DuplicateAction()
    {
        if (isDuplicated)
        {
            isValid = false;
            if (duplicateWarning != null)
                duplicateWarning.SetActive(true);

            if (onDuplicate.GetPersistentEventCount() > 0) onDuplicate.Invoke();
        }
    }
}