using UnityEngine;
using System.Net.Mail;
using Sirenix.OdinInspector;
using System.Threading.Tasks;
using System.IO;

public class SendErrorEmail : MonoBehaviour
{
    public string emailFrom;
    public string emailTarget;
    public string emailPassword;
    public string emailSubject;
    public string emailBody;

    [HideInInspector]
    public string attachmentPath;

    // Start is called before the first frame update
    private void Start()
    {
    }

    [Button]
    public async Task SendFileToEmail(string _path = "")
    {
        string fileName = Path.Combine(Path.GetDirectoryName(_path), "Email", $"{Path.GetFileNameWithoutExtension(_path)}-{System.DateTime.Now.ToString("HH-mm-ss-ff").Replace(":", "-")}{Path.GetExtension(_path)}");
        //if (!File.Exists(fileName))
        //File.Create(fileName);
        File.Copy(_path, fileName);

        using (MailMessage mail = new MailMessage())
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress(emailFrom);
            mail.To.Add(emailTarget);
            mail.Subject = JSONExtension.LoadEnv("SOURCE_IDENTIFIER_CODE");
            mail.Body = emailBody;
            mail.Attachments.Add(new Attachment(fileName));
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(emailFrom, emailPassword);
            SmtpServer.EnableSsl = true;
            SmtpServer.SendCompleted += SmtpServer_SendCompleted;
            await SmtpServer.SendMailAsync(mail);
        }
    }

    private void SmtpServer_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
        Debug.Log("email sent");
    }
}