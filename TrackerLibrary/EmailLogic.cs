using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace TrackerLibrary;

public static class EmailLogic
{
    public static void SendEmail(string to, string subject, string body)
    {
        MailAddress fromMailAddress = new(GlobalConfig.Settings.SenderEmail, GlobalConfig.Settings.SenderDisplayName);

        MailMessage mail = new();
        mail.To.Add(to);
        mail.From = fromMailAddress;
        mail.Subject = subject;
        mail.Body = body;
        mail.IsBodyHtml = true;

        SmtpClient client = GlobalConfig.Settings.smtp;
        client.Send(mail);
    }
}
