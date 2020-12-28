using UnityEngine;
using System.IO;
using System.Text;

public class LogMessageRecorder : MonoBehaviour
{
    private GameSettingEntity gameSettingEntity;

    private StreamWriter stream;
    private StreamWriter csvStream;

    private string FilePath
    {
        get
        {
            return string.Format(
                gameSettingEntity.Project_Folder + "{0}{1}{2}",
                new System.Object[] { "\\DebugMessages\\", System.DateTime.Now.ToShortDateString().Replace("/", "-"), "_DebugMessage.txt" });
        }
    }

    private string logCSVFilePath
    {
        get
        {
            return string.Format(gameSettingEntity.Project_Folder + "{0}{1}{2}",
            new System.Object[] { "\\DebugMessages\\", System.DateTime.Now.ToShortDateString().Replace("/", "-"), "_DebugMessage.csv" });
        }
    }

    private void OnEnable()
    {
        gameSettingEntity = FindObjectOfType<GameSettingEntity>();

        Directory.CreateDirectory(Path.GetDirectoryName(FilePath));

        // File not exist yet, create first column
        if (File.Exists(logCSVFilePath))
        {
            string column = "Time,Type,Message\n";
            File.WriteAllText(logCSVFilePath, column);
        }

        Application.logMessageReceived += LogHandler;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= LogHandler;
    }

    private void LogHandler(string logMsg, string stack, LogType logType)
    {
        StreamWriter stream = new StreamWriter(FilePath, true);
        StreamWriter csvStream = new StreamWriter(logCSVFilePath, true);

        if (logType == LogType.Log)
        {
            stream.WriteLine("[" + System.DateTime.Now.ToString() + "] NORMAL -- " + logMsg);

            csvStream.WriteLine(string.Format("{0},{1},{2}", System.DateTime.Now.ToString(), "Normal", logMsg));
        }
        else if (logType == LogType.Error || logType == LogType.Exception)
        {
            stream.WriteLine("[" + System.DateTime.Now.ToString() + "] ERROR -- " + logMsg);

            csvStream.WriteLine(string.Format("{0},{1},{2}", System.DateTime.Now.ToString(), "Error", logMsg));
        }

        stream.Close();
        csvStream.Close();
    }
}