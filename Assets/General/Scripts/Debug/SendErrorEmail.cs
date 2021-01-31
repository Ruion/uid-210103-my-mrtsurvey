using UnityEngine;
using System.Net.Mail;
using Sirenix.OdinInspector;
using System.Threading.Tasks;
using System.IO;

public class SendErrorEmail : MonoBehaviour
{
    private string EMAIL_USERNAME;
    private string EMAIL_PASSOWRD;
    private string EMAIL_RECIPIENTS;
    private string EMAIL_BODY = "Error";
    private string PROJECT_CODE;

    [HideInInspector]
    public string attachmentPath;

    // Start is called before the first frame update
    private void Start()
    { }

    private void OnEnable()
    {
        EMAIL_USERNAME = JSONExtension.LoadEnv("EMAIL_USERNAME");
        EMAIL_PASSOWRD = JSONExtension.LoadEnv("EMAIL_PASSOWRD");
        EMAIL_RECIPIENTS = JSONExtension.LoadEnv("EMAIL_RECIPIENTS");
        PROJECT_CODE = JSONExtension.LoadEnv("PROJECT_CODE");
    }

    [Button]
    public async Task SendFileToEmail(string _path = "")
    {
        string fileName = Path.Combine(Path.GetDirectoryName(_path), "Email", $"{Path.GetFileNameWithoutExtension(_path)}-{System.DateTime.Now.ToString("HH-mm-ss-ff").Replace(":", "-")}{Path.GetExtension(_path)}");

        File.Copy(_path, fileName);

        using (MailMessage mail = new MailMessage())
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress(EMAIL_USERNAME);
            foreach (var address in EMAIL_RECIPIENTS.Split(new[] { ";" }, System.StringSplitOptions.RemoveEmptyEntries))
            {
                mail.To.Add(address);
            }
            mail.Subject = PROJECT_CODE + " - " + JSONExtension.LoadEnv("SOURCE_IDENTIFIER_CODE");
            mail.Body = EMAIL_BODY;
            mail.Attachments.Add(new Attachment(fileName));
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(EMAIL_USERNAME, EMAIL_PASSOWRD);
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