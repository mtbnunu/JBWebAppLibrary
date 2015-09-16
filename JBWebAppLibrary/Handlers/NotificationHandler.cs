using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace JBWebappLibrary.Helpers
{
    public static class NotificationHandler
    {
        public static void SendEmail(string to, string from, string smtpHost, string subject, string body, bool isHtml = true,
            bool useDefaultCredentials = true, string username = "", string password = "")
        {
            var smtpClient = setUpSmtpClient(smtpHost, useDefaultCredentials, username, password);
            var message = setUpMessage(from, subject, body, isHtml);
            message.To.Add(to);
            smtpClient.Send(message);

        }

        public static void SendEmail(IEnumerable<string> to, string from, string smtpHost, string subject, string body,
            bool isHtml = true, bool useDefaultCredentials = true, string username = "", string password = "")
        {
            var smtpClient = setUpSmtpClient(smtpHost, useDefaultCredentials, username, password);
            var message = setUpMessage(from, subject, body, isHtml);

            foreach (var t in to)
            {

                message.To.Add(t);
            }
            smtpClient.Send(message);
        }



        private static SmtpClient setUpSmtpClient(string smtpHost, bool useDefaultCredentials, string username, string password)
        {
            SmtpClient smtpClient = new SmtpClient();

            smtpClient.Host = smtpHost;
            smtpClient.UseDefaultCredentials = useDefaultCredentials;
            if (!useDefaultCredentials)
            {
                NetworkCredential basicCredential = new NetworkCredential(username, password);
                smtpClient.Credentials = basicCredential;
            }
            return smtpClient;
        }

        private static MailMessage setUpMessage(string from, string subject, string body, bool isHtml)
        {
            MailMessage message = new MailMessage();
            MailAddress fromAddress = new MailAddress(from);

            message.From = fromAddress;
            message.Subject = subject;
            message.IsBodyHtml = isHtml;
            message.Body = body;
            return message;
        }
    }
}
