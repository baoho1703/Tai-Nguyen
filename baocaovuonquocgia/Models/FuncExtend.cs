using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace baocaovuonquocgia.Models
{
    public class FuncExtend
    {
        public static bool SendMail(string toAddress, string body, string subject)
        {
            bool success = true;
            var fromAddress = "thongtinhosoyteduphong@gmail.com";
            string fromPassword = "cucquanlymoitruong";
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress(fromAddress);
                mail.To.Add(toAddress);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                SmtpServer.Port = 25;
                SmtpServer.Credentials = new System.Net.NetworkCredential(fromAddress, fromPassword);
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }
    }
}