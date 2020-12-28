using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

/// <summary>
/// Add options to TMP_Dropdown field from a file
/// Tips: put a text file with line by line text into assets folder, 
/// attach this component to a TMP_Dropdown gameObject. You can add the options in editor 
/// by right click on this component in inspector and select Feed
/// Notes: The options will automatically feed into dropdown field in Start()
/// </summary>
public class DropDownFieldFeeder : MonoBehaviour
{
    [Header("Add options to Dropdown")]
    public TMP_Dropdown dropDown;
    public string phoneCodeFileName;
    [FilePath(AbsolutePath=true, Extensions="$fileExtension")] public string phoneCodeFile;
    public string fileExtension = "txt";

    private void Start()
    {
        if(dropDown == null) dropDown = GetComponent<TMP_Dropdown>();
        Feed();
    }

    [ContextMenu("FeedMobileCode")]
    public void Feed()
    {
        dropDown.ClearOptions();

      //  string path = Application.streamingAssetsPath + "/" + phoneCodeFileName;

      //  string[] texts = System.IO.File.ReadAllLines(path);
        string[] texts = System.IO.File.ReadAllLines(phoneCodeFile);

        List<string> options = new List<string>();

        foreach (string line in texts)
        {
            options.Add(line);
        }

        dropDown.AddOptions(options);
    }

}
