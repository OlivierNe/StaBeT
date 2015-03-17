using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace StageBeheersTool.Models.Services
{
    public class EmailService : IIdentityMessageService
    {

        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            //testemail587123@gmail.com
            //Mijnwachtwoord123
            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["smtpFrom"]);
                mailMessage.Subject = message.Subject;
                mailMessage.Body = message.Body;
                mailMessage.IsBodyHtml = true;
                mailMessage.To.Add(new MailAddress(message.Destination));

                //NetworkCredential NetworkCredential = new NetworkCredential();
                //NetworkCredential.UserName = mailMessage.From.Address;
                //NetworkCredential.Password = ConfigurationManager.AppSettings["smtpFromPw"];

                SmtpClient smtp = new SmtpClient();
                smtp.Host = ConfigurationManager.AppSettings["smtpServer"];
                smtp.EnableSsl = false;
                //smtp.UseDefaultCredentials = true;
                //smtp.Credentials = NetworkCredential;
                smtp.Port = int.Parse(ConfigurationManager.AppSettings["smtpPort"]);
                try
                {
                    //smtp.Send(mailMessage);
                }
                catch (Exception)
                {
                    Task.FromResult(1);
                }
            }
            return Task.FromResult(0);
        }

    }
}