using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Net;
using Sirenix.OdinInspector;
using System.Threading.Tasks;
using System;
using System.IO;

public class APICall : MonoBehaviour
{
    public string url = "http://200202-my-genting.unicom-interactive-digital.com/public/api/download-used-redeem-code-list";

    [Header("Time out in seconds")]
    public int requestTimeout = 5;

    public int requestInvokeDelay = 2;

    [Header("Download Path")]
    [FolderPath]
    public string downloadPath;

    [Header("Name of downloaded file")]
    public string fileName;

    private string folder;
    private string downloadFileNameWithoutExtension;
    private string downloadFileName;
    private string extension;
    private string downloadDestination;

    private GameSettingEntity gm;

    public UnityEvent onRequestSuccess;

    private void OnEnable()
    {
        gm = FindObjectOfType<GameSettingEntity>();

        if (fileName != null || fileName != "")
        {
            folder = gm.Project_Folder;
            downloadFileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            extension = Path.GetExtension(fileName);
            downloadFileName = Path.Combine(folder, $"{downloadFileNameWithoutExtension}-download{extension}");
            downloadDestination = Path.Combine(folder, fileName);
        }
    }

    public async void GetRequest()
    {
        HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
        myRequest.Timeout = requestTimeout * 1000;
        HttpWebResponse response = (HttpWebResponse)myRequest.GetResponse();

        if (response.StatusCode == HttpStatusCode.OK)
        {
            response.Close();
            await Task.Delay(requestInvokeDelay * 1000);
            if (onRequestSuccess.GetPersistentEventCount() > 0) onRequestSuccess.Invoke();
        }

        response.Close();
    }

    [Button]
    public void DownloadFile()
    {
        //Debug.Log(downloadDestination);
        gm = FindObjectOfType<GameSettingEntity>();

        WebClient webClient = new WebClient();
        webClient.DownloadFileAsync(new Uri(url), Path.Combine(folder, downloadFileName));
        webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
        // write downloaded file to exact file
    }

    private async void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
        FileStream destination = new FileStream(downloadDestination, FileMode.Open);

        using (FileStream source = File.Open(downloadFileName,
            FileMode.Open))
        {
            Console.WriteLine("Source length: {0}", source.Length.ToString());

            // Copy source to destination.
            //source.CopyTo(destination);

            await source.CopyToAsync(destination);

            source.Close();
            destination.Close();
        }

        await Task.Delay(requestInvokeDelay * 1000);

        if (onRequestSuccess.GetPersistentEventCount() > 0) onRequestSuccess.Invoke();
    }
}