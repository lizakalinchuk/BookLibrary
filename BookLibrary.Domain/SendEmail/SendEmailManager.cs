using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Domain.SendEmail
{
    public class SendEmailManager : ISendEmailManager
    {
        public void Send(string EmailTo, string title)
        {
            var appSettings = ConfigurationManager.AppSettings;
            int port = Convert.ToInt32(appSettings["port"]); 
            bool enableSSL = true;
            string emailFrom = appSettings["emailFrom"];
            string password = appSettings["password"]; 
            string emailTo = EmailTo; ; 
            string subject = "Library Info";
            string body = string.Format("You took the '{0}' in our library!", title);
            string smtpAddress = appSettings["smtpAddress"];

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(emailFrom);
            mail.To.Add(emailTo);
            mail.Subject = subject;
            mail.Body = body;

            using (SmtpClient smtp = new SmtpClient(smtpAddress, port))
            {
                smtp.Credentials = new NetworkCredential(emailFrom, password);
                smtp.EnableSsl = enableSSL;
                try
                {
                    smtp.Send(mail); 
                }
                catch {  
                // need some action
                }
            }
        }
    }
}
