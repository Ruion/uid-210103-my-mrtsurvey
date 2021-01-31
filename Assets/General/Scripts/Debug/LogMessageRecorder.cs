using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Threading;

public class LogMessageRecorder : MonoBehaviour
{
    private static readonly ReaderWriterLockSlim _rwLockSlim = new ReaderWriterLockSlim();

    private StreamWriter stream;
    private StreamWriter csvStream;

    private SendErrorEmail emailSender;

    private List<Task> writeTasks = new List<Task>();
    private bool txtWriting;
    private bool isSending;

    private List<int> csvsWriting = new List<int>();

    private string FilePath
    {
        get
        {
            return string.Format(
                JSONExtension.PROJECT_FOLDER + "{0}{1}{2}",
                new System.Object[] { "\\DebugMessages\\", System.DateTime.Now.ToShortDateString().Replace("/", "-"), "_DebugMessage.txt" });
        }
    }

    private string logCSVFilePath
    {
        get
        {
            return string.Format(JSONExtension.PROJECT_FOLDER + "{0}{1}{2}",
            new System.Object[] { "\\DebugMessages\\", System.DateTime.Now.ToShortDateString().Replace("/", "-"), "_DebugMessage.csv" });
        }
    }

    [Button]
    private void OnEnable()
    {
        emailSender = FindObjectOfType<SendErrorEmail>();
        Parallel.Invoke(() =>
            {
                if (Directory.Exists(Path.GetDirectoryName(FilePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(FilePath));

                // File not exist yet, create first column
                if (!File.Exists(logCSVFilePath))
                {
                    string column = "Time,Type,Message\n";
                    File.WriteAllText(logCSVFilePath, column);
                }

                Application.logMessageReceived += LogHandler;
            },
            () => emailSender.attachmentPath = logCSVFilePath,
            () =>
            {
                string emailFolder = Path.Combine(Path.GetDirectoryName(logCSVFilePath), "Email");
                if (Directory.Exists(emailFolder))
                    Directory.Delete(emailFolder, true);

                Directory.CreateDirectory(emailFolder);
            }
        );
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= LogHandler;
    }

    private async void LogHandler(string logMsg, string stack, LogType logType)
    {
        csvsWriting.Add(0);
        string dateTime = System.DateTime.Now.ToString();
        writeTasks.Add(LogTxt(logMsg, logType == LogType.Error || logType == LogType.Exception ? true : false, dateTime));
        writeTasks.Add(LogCSV(logMsg, logType == LogType.Error || logType == LogType.Exception ? true : false, dateTime));

        await WriteLogs(logMsg).ConfigureAwait(false);
    }

    private async Task WriteLogs(string logMsg)
    {
        await Task.WhenAll(writeTasks)
        .ContinueWith((task) =>
        {
            writeTasks = new List<Task>();
        });
    }

    private async Task LogTxt(string logMsg, bool _hasError, string dateTime)
    {
        try
        {
            while (txtWriting) await Task.Delay(1000);
            txtWriting = true;
            using (stream = new StreamWriter(FilePath, true))
            {
                string type = _hasError == true ? "ERROR" : "NORMAL";
                await stream.WriteLineAsync("[" + dateTime + $"] {type} -- " + logMsg);
            }
            txtWriting = false;
        }
        catch (Exception ex)
        {
            txtWriting = false;
            Debug.LogError(ex.Message);
        }
    }

    private async Task LogCSV(string logMsg, bool _hasError, string dateTime)
    {
        _rwLockSlim.EnterWriteLock();
        try
        {
            using (csvStream = File.AppendText(logCSVFilePath))
            {
                string type = _hasError == true ? "ERROR" : "NORMAL";
                await csvStream.WriteLineAsync("[" + DateTime.Now.ToString() + $"] {type} -- " + logMsg);
            }
            csvsWriting.RemoveAt(0);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        finally
        {
            _rwLockSlim.ExitWriteLock();
            while (isSending) await Task.Delay(1000);
            if (_hasError)
            {
                while (csvsWriting.Count > 0) await Task.Delay(1000);
                isSending = true;
                await SendErrorEmail();
                isSending = false;
            }
        }
    }

    private async Task SendErrorEmail()
    {
        // delete all emails before sending new
        string emailFolder = Path.Combine(Path.GetDirectoryName(logCSVFilePath), "Email");
        if (Directory.Exists(emailFolder))
            Directory.Delete(emailFolder, true);

        Directory.CreateDirectory(emailFolder);

        try
        {
            await emailSender.SendFileToEmail(logCSVFilePath);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    [Button]
    private void LogErrorMessage()
    {
        Debug.LogError("Manually Triggered Error Message");
    }
}