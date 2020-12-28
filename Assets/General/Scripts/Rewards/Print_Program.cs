using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Print_Program : RewardMaster { 
    public string printerPath;

    public UnityEvent OnPrinted;

    public void Print(string name)
    {
        var proc = new System.Diagnostics.Process();
        proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        proc.StartInfo.Verb = "print";

        // Using PDFtoPrinter
        proc.StartInfo.FileName = printerPath + "PDFtoPrinter.exe";
        
        proc.StartInfo.Arguments = printerPath + name + ".pdf";
        Debug.Log(proc.StartInfo.Arguments);
        /*
        proc.StartInfo.FileName = final_path;
        proc.StartInfo.Arguments = final_path2;
        */

        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.CreateNoWindow = true;

        OnPrinted.Invoke();

        proc.Start();

        if (proc.HasExited == false)
        {
            proc.WaitForExit(3000);
        }

        proc.EnableRaisingEvents = true;

        Debug.Log("Printed");
        
        proc.Close();
    }
}