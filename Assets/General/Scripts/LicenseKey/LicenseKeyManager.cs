using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using TMPro;

public class LicenseKeyManager : MonoBehaviour
{
    public GameObject emptyHandler;
    public GameObject internetErrorHandler;
    public GameObject errorHandler;
    public GameObject loadingHandler;
    public GameObject successSendDataHandler;

    public GameObject licenseKeyNotSaveHandler;

    public GameObject licenseKeyPanel;

    private void Start()
    {
        licenseKeyPanel.SetActive(true);
        CheckInternetConnectionFrequent();
    }

    public void CheckInternetConnection()
    {
        string HtmlText = GetHtmlFromUri("http://google.com");
        if (HtmlText == "")
        {
            //No connection
            internetErrorHandler.SetActive(true);
        }
        else
        {
            HideAllHandler();

            CancelInvoke("CheckInternetConnection");

            // turn off license key panel
            licenseKeyPanel.SetActive(false);
            
        }
    }

    public string GetHtmlFromUri(string resource)
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

    public void HideAllHandler()
    {
        emptyHandler.SetActive(false);
        internetErrorHandler.SetActive(false);
        errorHandler.SetActive(false);
        loadingHandler.SetActive(false);
        successSendDataHandler.SetActive(false);
    }

    public void CheckInternetConnectionFrequent()
    {
        InvokeRepeating("CheckInternetConnection", .5f, 1f);
    }
}
