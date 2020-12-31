using UnityEngine;
using System.Diagnostics;
using Sirenix.OdinInspector;

public class ExecutableLauncher : MonoBehaviour
{
    [FilePath(AbsolutePath = true, Extensions = "exe")]
    public string executablePath;

    private bool launched = false;

    private Process proc;

    public string arguments;

    public void LaunchExecutable()
    {
        Process[] process = Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(executablePath));
        if (process.Length > 0) launched = true;
        if (launched) return;

        proc = new Process();
        proc.StartInfo.FileName = executablePath;
        proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        proc.StartInfo.CreateNoWindow = true;

        
        if (!string.IsNullOrEmpty(arguments)) proc.StartInfo.Arguments = arguments;

        proc.Start();
        UnityEngine.Debug.Log($"{name} - launch {executablePath} at {System.DateTime.Now}");
        //  System.Diagnostics.Process.Start(executablePath);
        launched = true;
    }

    private void OnApplicationQuit()
    {
        if (launched)
        {
            Process[] process = Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(executablePath));
            if (process.Length > 0) proc = process[0];

            if (!proc.WaitForExit(1000))
                proc.Kill();
        }
    }
}