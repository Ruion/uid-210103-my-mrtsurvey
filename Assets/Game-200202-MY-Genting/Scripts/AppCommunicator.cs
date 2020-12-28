using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Sirenix.OdinInspector;

public class AppCommunicator : SerializedMonoBehaviour
{
    public string vendingMachinePlayerPrefsName;

    [Button(ButtonSizes.Medium)]
    public void WriteTextToFile([FilePath(AbsolutePath = true)] string filePath, string textToWrite)
    {
        File.WriteAllText(filePath, textToWrite);
    }

    public void CommunicateVendingMachine()
    {
        WriteTextToFile(@"C:\UID-APP\App-VendingMachineDropper\QueryCondition.txt",
            string.Format(" AND {0} = '{1}'",
            new System.Object[] { vendingMachinePlayerPrefsName, PlayerPrefs.GetString(vendingMachinePlayerPrefsName, "") }));
    }
}