using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Net.Http;
using System.Net;
using System.IO;

public class HttpClientCall : MonoBehaviour
{
    public string url;

    // Start is called before the first frame update
    private void Start()
    {
    }

    public void HttpCall()
    {
        //HttpClient webRequest = new HttpClient();

        //networkResponse = await webRequest.GetStringAsync(networkValidationURL + "/" + processedValue);
        //if (System.Convert.ToBoolean(networkResponse) != true)
        //{
        //    isDuplicated = true;
        //    Debug.Log($"network validation {processedValue} is duplicated");
        //}

        var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "POST";

        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        {
            string json = "{\"user\":\"test\"," +
                          "\"password\":\"bla\"}";

            streamWriter.Write(json);
        }

        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
            var result = streamReader.ReadToEnd();
            Debug.Log(result);
        }
    }
}