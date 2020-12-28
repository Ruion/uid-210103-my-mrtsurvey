using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Security;

public class Encryption : MonoBehaviour
{
    [Header("Pages object")]
    #region pages object
    public GameObject license_key_page; // input license key page
    public GameObject loading_page; // checking validation page
    public GameObject error_no_internet;
    public GameObject error_no_internet2;
    public GameObject check_online_validation_page;
    [Header("Error Pages For Registration")]
    public GameObject error_page1; // invalid license key error
    public GameObject error_page2; // expired license key error
    public GameObject error_page3; // used license key error
    public GameObject success_page; // successfully input license key
    [Header("Error Pages For Validation")]
    public GameObject error_page4; // last checkpoint is newer than current time error
    public GameObject error_page5; // checkpoint not in sequence error
    public GameObject error_page6; // session expired error
    #endregion

    [Header("License key Input Field")]
    #region License Key Input Field
    public TMP_InputField Lkey;
    public Button Submit;
    #endregion

    #region PathFile
    string pathfile1;
    string pathfile2;
    string pathfile3;
    string pathfile4;
    string pathfile5;
    #endregion

    [Header("All Variable")]
    #region Variable
    public string licensekey;
    public string lkey_expdate;
    public string machine_id;
    public string session_expdate;
    public string checkpoint;
    public DateTime TempDate;
    public string temp_msg;
    bool Clean2 = false;
    List<string> temp_checkpoint = new List<string>();
    #endregion

    public UnityEvent OnValidateSuccess;

    const string glyphs = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
   // const string passwords_encryption = "Unicom_UID_2019";


/// Game-01: 2NALZTD565JR
/// Game-02: 8VR8DRT9Q7J2
    const string passwords_encryption = "8VR8DRT9Q7J2";
    /// <summary>
    /// Change this URL
    /// </summary>
    private string urlstage2_Register = "activate-license-key";
    private string urlstage2_Validate = "validate-license-key";

    private const string urlRegisterAPI = "/public/api/activate-license-key";
    private const string urlValidateAPI = "/public/api/validate-license-key";

    void Awake()
    {
        transform.SetParent(null);

        GameObject[] objs = GameObject.FindGameObjectsWithTag("LicenseKey");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);

        JSONSetter jsonSetter = FindObjectOfType<JSONSetter>();
        string folder = "/LicenseKey";
        Directory.CreateDirectory(Path.GetDirectoryName(jsonSetter.savePath + folder + "/Lkey.txt"));
        //declare local data path
        pathfile1 = jsonSetter.savePath + folder + "/Lkey.txt";
        pathfile2 = jsonSetter.savePath + folder  + "/key_exp_date.txt";
        pathfile3 = jsonSetter.savePath + folder + "/Identity.txt";
        pathfile4 = jsonSetter.savePath + folder + "/session_exp_date.txt";
        pathfile5 = jsonSetter.savePath + folder + "/checkpoint.txt";
    }

    // Start is called before the first frame update
    void Start()
    {
        /*string timenow = DateTime.Now.Date.ToString("dd/MM/yyyy");
        string a = GetMD5HashString("Testing");
        string b = GetMD5HashString("Testing");
        Debug.Log(a + " is A value & " + b + " is B value");

        string lala = "Testing";
        int hhhh = lala.GetHashCode();
        Debug.Log("Hash Value for hhhh = " + hhhh);
        int haha = a.GetHashCode();
        Debug.Log("Hash Value for haha = " + haha);
        int hihi = b.GetHashCode();
        Debug.Log("Hash Value for hihi = " + hihi);
        Debug.Log(timenow);*/

        CheckFile();
    }

    void OnEnable()
    {
        JSONSetter jsonSetter = FindObjectOfType<JSONSetter>();
        urlstage2_Register = jsonSetter.LoadSetting()["ServerDomainURL"].ToString() + urlRegisterAPI;
        urlstage2_Validate = jsonSetter.LoadSetting()["ServerDomainURL"].ToString() + urlValidateAPI;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Set_Interactable_SubmitButton()
    {
        if(Lkey.text != null)
        {
            Submit.interactable = true;
        }
    }

    //Initial check if got any license key data or not
    void CheckFile()
    {
        if (!File.Exists(pathfile1) || !File.Exists(pathfile2) || !File.Exists(pathfile3) || !File.Exists(pathfile4) || !File.Exists(pathfile5))
        {
            //go to stage 1 Register
            Stage1Register();
        }
        else
        {
            //go to validation stage 1
            Validation_Stage1();
        }
    }
    //------------------------------------------------------------LICENSE KEY VALIDATION CHECK STAGES--------------------------------------------------------------------------------
    #region Stages for Checking validation of license key
    void Validation_Stage1()
    {
        Debug.Log("Start Validation Stage 1");
        loading_page.SetActive(true);
        GetLocal_Checkpoint();

        DateTime Date1 = DateTime.Now;
        DateTime Date2 = DateTime.Parse(temp_checkpoint[temp_checkpoint.Count - 1]);

        int compare_stage1 = DateTime.Compare(Date1, Date2);
        if(compare_stage1 < 0)
        {
            //current time < last checkpoint = something wrong
            //show error page
            error_page4.SetActive(true);
            loading_page.SetActive(false);
        }
        else if(compare_stage1 == 0)
        {
            //current time is the same as last checkpoint = impossible to open app 2 time simultaneously = something wrong
            //show error page
            error_page4.SetActive(true);
            loading_page.SetActive(false);
        }
        else
        {
            //current time > last checkpoint = correct
            //set validation 1 clear
            //go to next stage
            Validation_Stage2();
        }
    }

    void Validation_Stage2()
    {
        Debug.Log("Start Validation Stage 2");

        for(int i = 1; i < temp_checkpoint.Count; i++)
        {
            DateTime Date1 = DateTime.Parse(temp_checkpoint[i - 1]);
            DateTime Date2 = DateTime.Parse(temp_checkpoint[i]);
            
            int compare_stage2 = DateTime.Compare(Date1, Date2);
            if(compare_stage2 < 0)
            {
                //date & time 1 < next date & time = true
                //check if its the last element checking
                if(i == temp_checkpoint.Count-1)
                {
                    Clean2 = true;
                }
            }
            else if(compare_stage2 == 0)
            {
                //date & time 1 == next date & time = impossible to open app 2 time simultaneously = something wrong
                //show error page
                error_page5.SetActive(true);
                loading_page.SetActive(false);
                break;
            }
            else
            {
                //date & time 1 > next date & time = something wrong with the sequence of checkpoint
                //show error page
                error_page5.SetActive(true);
                loading_page.SetActive(false);
                break;
            }
        }

        if (temp_checkpoint.Count == 1)
        {
            DateTime Date1 = DateTime.Parse(temp_checkpoint[0]);
            DateTime Date2 = DateTime.Now;
            int compare_stage2 = DateTime.Compare(Date1, Date2);
            if (compare_stage2 < 0)
            {
                //date & time 1 < next date & time = true
                //check if its the last element checking
                Clean2 = true;
            }
            else if (compare_stage2 == 0)
            {
                //date & time 1 == next date & time = impossible to open app 2 time simultaneously = something wrong
                //show error page
                error_page5.SetActive(true);
                loading_page.SetActive(false);
            }
            else
            {
                //date & time 1 > next date & time = something wrong with the sequence of checkpoint
                //show error page
                error_page5.SetActive(true);
                loading_page.SetActive(false);
            }
        }

        //check if clean2 get true
        if(Clean2)
        {
            //go to Validation Stage 3
            Validation_Stage3();
        }
        else
        {
            //show error page while checking sequence of checkpoint
            error_page5.SetActive(true);
            loading_page.SetActive(false); ;
        }
    }

    void Validation_Stage3()
    {
        Debug.Log("Start Validation Stage 3");

        GetLocal_SessionExpDate();

        DateTime Date1 = DateTime.Now;
        DateTime Date2 = DateTime.Parse(session_expdate);

        int compare_stage3 = DateTime.Compare(Date1, Date2);

        if(compare_stage3 < 0)
        {
            //current time < session expiry date = nothings wrong
            Validation_Stage4();
        }
        else if(compare_stage3 == 0)
        {
            //current time == session expiry date = the session expired
            //promp user to connect internet and renew session expiry date
            //go to Renew Session Expiry Stage 1
            error_page6.SetActive(true);
            loading_page.SetActive(false);
        }
        else
        {
            //current time > session expiry date = the session expired
            //promp user to connect internet and renew session expiry date
            //go to Renew Session Expiry Stage 1
            error_page6.SetActive(true);
            loading_page.SetActive(false);
        }
    }
    
    void Validation_Stage4()
    {
        Debug.Log("Start Validation Stage 4");

        GetLocal_LicenseKey_ExpDate();
        DateTime Date1 = DateTime.Now;
        DateTime Date2 = DateTime.Parse(lkey_expdate);

        int compare_stage4 = DateTime.Compare(Date1, Date2);

        if(compare_stage4 < 0)
        {
            //set current checkpoint
            SetLocal_Checkpoint();

            //go to game
            //close all license key page
            loading_page.SetActive(false);
        }
        else if(compare_stage4 == 0)
        {
            //license key expired since it is the same time as current time
            error_page2.SetActive(true);
            loading_page.SetActive(false);
        }
        else
        {
            //license key expired since it is older than the current time
            error_page2.SetActive(true);
            loading_page.SetActive(false);
        }
    }

    void Error_Validation_Stage1()
    {
        //show error message & to connect internet
        check_online_validation_page.SetActive(true);
        error_page4.SetActive(false);
        error_page5.SetActive(false);
        error_page6.SetActive(false);
        error_no_internet2.SetActive(false);
        //check online
        GetLocal_MachineIdentity();
        GetLocal_LicenseKey();

        StartCoroutine(Error_Validation_Stage1_SendData());
    }
    IEnumerator Error_Validation_Stage1_SendData()
    {
        WWWForm form = new WWWForm();
        form.AddField("license_key", licensekey);
        form.AddField("mic", machine_id);
        
        using (UnityWebRequest www = UnityWebRequest.Post(urlstage2_Validate, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                //show error no internet
                error_no_internet2.SetActive(true);
                check_online_validation_page.SetActive(false);
            }
            else
            {
                Debug.Log("Form upload complete! " + www.downloadHandler.text);
                var Stage2jsonData = JsonUtility.FromJson<Stage2Data>(www.downloadHandler.text);
                temp_msg = Stage2jsonData.result;
                Debug.Log(temp_msg);
                if (temp_msg == "Invalid License Key")
                {
                    //show error invalid
                    error_page1.SetActive(true);
                    check_online_validation_page.SetActive(false);
                }
                else if (temp_msg == "Expired License Key")
                {
                    //show error expired
                    error_page2.SetActive(true);
                    check_online_validation_page.SetActive(false);
                }
                else if (temp_msg == "Used License Key")
                {
                    //show error used license key
                    error_page3.SetActive(true);
                    check_online_validation_page.SetActive(false);
                }
                else if (temp_msg == "Valid License Key")
                {
                    lkey_expdate = Stage2jsonData.license_key_expiry_date;
                    //go to stage 2 error validation
                    Error_Validation_Stage2();
                }
            }
        }
    }
    void Error_Validation_Stage2()
    {
        Debug.Log("Reset The local Data for trying to modify license key data");
        check_online_validation_page.SetActive(false);

        File.WriteAllText(pathfile4, "");
        File.WriteAllText(pathfile5, "");

        SetLocal_Checkpoint();
        SetLocal_SessionExpDate();
    }
    #endregion
    //----------------------------------------------------------LICENSE KEY REGISTRATION STAGES--------------------------------------------------------------------------------------
    #region Stages for Registration license key
    //stage 1 Registration license key
    void Stage1Register()
    {
        Debug.Log("Start Stage 1");
        license_key_page.SetActive(true);
    }
    //Stage 2 Registration license key (called from button onclick event)
    public void Stage2Register()
    {
        Debug.Log("Start Stage 2");
        license_key_page.SetActive(false);
        loading_page.SetActive(true);
        StartCoroutine(Stage2Register_SendData());

        //show loading page and hide insert license key page
    }
    IEnumerator Stage2Register_SendData()
    {
        Debug.Log("Stage 2 Sending Data...");
        SetMachineID();
        licensekey = Lkey.text;
        WWWForm form = new WWWForm();
        form.AddField("license_key", licensekey);
        form.AddField("mic", machine_id);

        //http://www.test.com/index.htm?name1=value1&name2=value2 // FOR GET METHOD {license_key}/mic/{mic}
        Debug.Log(urlstage2_Register);
       using (UnityWebRequest www = UnityWebRequest.Post(urlstage2_Register, form)) 
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
                //show error no internet
                error_no_internet.SetActive(true);
                loading_page.SetActive(false);
            }
            else
            {
                Debug.Log("Form upload complete! " + www.downloadHandler.text);
                var Stage2jsonData = JsonUtility.FromJson<Stage2Data>(www.downloadHandler.text);
                temp_msg = Stage2jsonData.result;
                Debug.Log(temp_msg);
                if (temp_msg == "Invalid License Key")
                {
                    //show error invalid
                    error_page1.SetActive(true);
                    loading_page.SetActive(false);
                }
                else if(temp_msg == "Expired License Key")
                {
                    //show error expired
                    error_page2.SetActive(true);
                    loading_page.SetActive(false);
                }
                else if (temp_msg == "Used License Key")
                {
                    //show error used license key
                    error_page3.SetActive(true);
                    loading_page.SetActive(false);
                }
                else if (temp_msg == "Valid License Key")
                {
                    Debug.Log(Stage2jsonData.license_key_expiry_date);
                    lkey_expdate = Stage2jsonData.license_key_expiry_date;
                    //go to stage 3 Registration
                    Stage3Registration();
                }
            }
        }
    }

    void Stage3Registration()
    {
        Debug.Log("Start Stage 3");
        //Create 5 new file to store data
        File.WriteAllText(pathfile1, "");
        File.WriteAllText(pathfile2, "");
        File.WriteAllText(pathfile3, "");
        File.WriteAllText(pathfile4, "");
        File.WriteAllText(pathfile5, "");

        //call stage 4 which is to store data
        Stage4Registration();
    }

    void Stage4Registration()
    {
        Debug.Log("Start Stage 4");
        //set the license key
        SetLocal_LicenseKey();

        //set the expiry date
        SetLocal_LicenseKey_ExpDate();

        //set the machine id
        SetLocal_MachineIdentity();

        //set session expiry date
        SetLocal_SessionExpDate();

        //set checkpoint
        SetLocal_Checkpoint();

        success_page.SetActive(true);
        if(OnValidateSuccess.GetPersistentEventCount() > 0) OnValidateSuccess.Invoke();
        
        loading_page.SetActive(false);
    }
    #endregion
    //--------------------------------------------------------SET AND GET FOR CHECKING LOCAL DATA------------------------------------------------------------------------------------
    #region Set&Get
    //set and get for license key
    public void SetLocal_LicenseKey()
    {
        //license key get from developer
        StreamWriter writer = new StreamWriter(pathfile1, true);
        writer.Write(licensekey + "\n");
        Debug.Log("set license key : " + licensekey);
        writer.Close();
    }
    public void GetLocal_LicenseKey()
    {
        StreamReader reader = new StreamReader(pathfile1, true);
        licensekey = reader.ReadLine();
        Debug.Log(licensekey);
        reader.Close();
    }

    //set and get for license key expired date
    public void SetLocal_LicenseKey_ExpDate()
    {
        //license key expired date get from server
        string temp_lkey_expdate = HashedClass.StringCipher.Encrypt(lkey_expdate, passwords_encryption);
        StreamWriter writer = new StreamWriter(pathfile2, true);
        writer.Write(temp_lkey_expdate + "\n");
        Debug.Log("set license key expiry date : " + lkey_expdate);
        writer.Close();
    }
    public void GetLocal_LicenseKey_ExpDate()
    {
        StreamReader reader = new StreamReader(pathfile2, true);
        string temp_lkey_expdate = reader.ReadLine();
        string temp_get_lkeyexpdate = HashedClass.StringCipher.Decrypt(temp_lkey_expdate, passwords_encryption);
        lkey_expdate = temp_get_lkeyexpdate;
        Debug.Log(lkey_expdate);
        reader.Close();
    }

    public void SetMachineID()
    {
        machine_id = "";
        //random 256 character for machine id
        for (int i = 0; i < 256; i++)
        {
            machine_id += glyphs[UnityEngine.Random.Range(0, glyphs.Length)];
        }
        Debug.Log("generate machine ID : " +machine_id);
    }

    //set and get for machine identity
    public void SetLocal_MachineIdentity()
    {
        StreamWriter writer = new StreamWriter(pathfile3, true);
        writer.Write(machine_id + "\n");
        Debug.Log("set machine ID : " +machine_id);
        writer.Close();
    }
    public void GetLocal_MachineIdentity()
    {
        StreamReader reader = new StreamReader(pathfile3, true);
        machine_id = reader.ReadLine();
        Debug.Log(machine_id);
        reader.Close();
    }
    //set and get for session expire date
    public void SetLocal_SessionExpDate()
    {
        DateTime ExpiryDateSession = DateTime.Now.AddDays(7);
        session_expdate = ExpiryDateSession.ToString();
        string temp_sessionexpdate = HashedClass.StringCipher.Encrypt(session_expdate, passwords_encryption);
        StreamWriter writer = new StreamWriter(pathfile4, true);
        writer.Write(temp_sessionexpdate + "\n");
        Debug.Log("set session expiry date : " + session_expdate);
        writer.Close();
    }
    public void GetLocal_SessionExpDate()
    {
        StreamReader reader = new StreamReader(pathfile4, true);
        string temp_sessionexpdate = reader.ReadLine();
        string temp_decode = HashedClass.StringCipher.Decrypt(temp_sessionexpdate, passwords_encryption);
        session_expdate = temp_decode;
        Debug.Log(session_expdate);
        reader.Close();
    }

    //set and get for checkpoint
    public void SetLocal_Checkpoint()
    {
        //checkpoint is the datetime of local
        checkpoint = DateTime.Now.ToString();
        string temp_checkpoint = HashedClass.StringCipher.Encrypt(checkpoint, passwords_encryption);
        StreamWriter writer = new StreamWriter(pathfile5, true);
        writer.Write(temp_checkpoint + "\n");
        Debug.Log("set checkpoint : " + checkpoint);
        writer.Close();
    }
    public void GetLocal_Checkpoint()
    {
        //need to include function to check checkpoint 1 and 2
        StreamReader reader = new StreamReader(pathfile5, true);
        while (!reader.EndOfStream)
        {
            string temp_read = reader.ReadLine();
            string temp_read_decrypt = HashedClass.StringCipher.Decrypt(temp_read, passwords_encryption);
            temp_checkpoint.Add(temp_read_decrypt);
        }
        reader.Close();
    }
    #endregion



    public void Close_Success_Page()
    {
        license_key_page.SetActive(false); // input license key page
        loading_page.SetActive(false); // checking validation page
        error_page1.SetActive(false); // invalid license key error
        error_page2.SetActive(false); // expired license key error
        error_page3.SetActive(false); // used license key error
        success_page.SetActive(false); // successfully input license key
        error_page4.SetActive(false); // last checkpoint is newer than current time error
        error_page5.SetActive(false); // checkpoint not in sequence error
        error_page6.SetActive(false); // session expired error
        error_no_internet.SetActive(false);
        check_online_validation_page.SetActive(false);
}
    public void Close_Error_Registration123()
    {
        license_key_page.SetActive(true);
        error_page1.SetActive(false);
        error_page2.SetActive(false);
        error_page3.SetActive(false);
        error_no_internet.SetActive(false);
    }
    public void Close_Error_Validation4567()
    {
        Error_Validation_Stage1();
    }


    //json data class
    [System.Serializable]
    public class Stage2Data
    {
        public string result;
       // public string license_expiry_date;
        public string license_key_expiry_date;
    }
}
