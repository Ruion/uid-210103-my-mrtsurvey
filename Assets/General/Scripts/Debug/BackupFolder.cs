using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using Sirenix.OdinInspector;
using System.Net.Mail;
using System.Threading.Tasks;
using Ionic.Zip;
using UnityEngine.Networking;

public class BackupFolder : MonoBehaviour
{
    // backup database folder as zip and send to email

    // copy files and zip it
    private string databasePath;

    [SerializeField]
    private string[] directoryPathToBackup;

    private string backupDatabasePath;
    private string backupDatabaseZipPath;
    private string source_identifier_code;
    private string FILE_UPLOAD_URL;
    private string PROJECT_FOLDER;

    public string uploadAPI = "upload-device-debug-file";

    public bool isFileEnable { get; set; }
    public UnityEvent OnBackUpStart;
    public UnityEvent OnBackUpFinish;

    // Start is called before the first frame update
    private void Start()
    {
    }

    private void OnEnable()
    {
        bool debugMode = JSONExtension.LoadEnvBool("DEBUG_MODE");
        FILE_UPLOAD_URL = (debugMode ? JSONExtension.LoadEnv("SERVER_URL_TESTING") : JSONExtension.LoadEnv("SERVER_URL")) + (debugMode ? JSONExtension.LoadEnv("API_ROUTE_DEBUG") : JSONExtension.LoadEnv("API_ROUTE")) + uploadAPI;
        source_identifier_code = JSONExtension.LoadEnv("SOURCE_IDENTIFIER_CODE");
        PROJECT_FOLDER = FindObjectOfType<GameSettingEntity>().Project_Folder;
    }

    [Button]
    public void BackUpDatabaseFolder()
    {
        if (!isFileEnable) return;

        OnBackUpStart.Invoke();

        databasePath = Path.Combine(FindObjectOfType<GameSettingEntity>().Project_Folder, "Databases");
        backupDatabasePath = Path.Combine(FindObjectOfType<GameSettingEntity>().Project_Folder, "TimeBackup", "Databases");
        backupDatabaseZipPath = Path.Combine(FindObjectOfType<GameSettingEntity>().Project_Folder, "TimeBackup", "Databases.zip");
        DirectoryCopy(databasePath, backupDatabasePath, true);

        using (ZipFile zip = new ZipFile())
        {
            for (int p = 0; p < directoryPathToBackup.Length; p++)
            {
                string path = Path.Combine(PROJECT_FOLDER, directoryPathToBackup[p]);
                string backupPath = Path.Combine(PROJECT_FOLDER, "TimeBackup", directoryPathToBackup[p]);
                DirectoryCopy(path, backupPath, true);
                zip.AddDirectory(backupPath, directoryPathToBackup[p]);
            }

            zip.Save(Path.Combine(PROJECT_FOLDER, source_identifier_code + ".zip"));
        }

        // upload zip file to server
        StartCoroutine(UploadZipFile(Path.Combine(PROJECT_FOLDER, source_identifier_code + ".zip")));
    }

    private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        DirectoryInfo[] dirs = dir.GetDirectories();

        // If the destination directory doesn't exist, create it.
        Directory.CreateDirectory(destDirName);

        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string tempPath = Path.Combine(destDirName, file.Name);
            file.CopyTo(tempPath, true);
        }

        // If copying subdirectories, copy them and their contents to new location.
        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
            }
        }
    }

    private IEnumerator UploadZipFile(string _path)
    {
        // Send POST request.
        WWWForm form = new WWWForm();
        // Add the file.
        form.AddField("source_identifier_code", source_identifier_code);
        form.AddBinaryData("debug", File.ReadAllBytes(_path), source_identifier_code + ".zip", "application/zip");
        //form.AddBinaryData("debug", File.ReadAllBytes(_path));

        UnityWebRequest req = UnityWebRequest.Post(FILE_UPLOAD_URL, form);
        yield return req.SendWebRequest();

        if (req.isHttpError || req.isNetworkError)
            Debug.Log(req.error + $"\n {FILE_UPLOAD_URL}");
        else
            Debug.Log("Backup uploaded");

        // delete folder copied for zip
        Directory.Delete(Path.Combine(PROJECT_FOLDER, "TimeBackup"), true);

        OnBackUpFinish.Invoke();
    }
}