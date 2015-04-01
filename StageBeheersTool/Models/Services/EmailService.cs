using Microsoft.AspNet.Identity;
using System;
using System.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;

namespace StageBeheersTool.Models.Services
{
    public class EmailService : IIdentityMessageService
    {

        public Task SendAsync(IdentityMessage message)
        {
            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["smtpFrom"]);
                mailMessage.Subject = message.Subject;
                mailMessage.Body = message.Body;
                mailMessage.IsBodyHtml = true;
                mailMessage.To.Add(new MailAddress(message.Destination));

                var smtp = new SmtpClient
                {
                    Host = ConfigurationManager.AppSettings["smtpServer"],
                    EnableSsl = false,
                    Port = int.Parse(ConfigurationManager.AppSettings["smtpPort"])
                };
                try
                {
                   // smtp.Send(mailMessage);
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