using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Models.Services
{
    public class EmailService : IEmailService
    {
        private readonly IInstellingenRepository _instellingenRepository;

        public EmailService(IInstellingenRepository instellingenRepository)
        {
            _instellingenRepository = instellingenRepository;
        }

        public Task SendAsync(IdentityMessage message)
        {
            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["smtpFrom"]);
                mailMessage.Subject = message.Subject;
                mailMessage.Body = message.Body;
                mailMessage.IsBodyHtml = true;
                mailMessage.To.Add(new MailAddress(message.Destination));
                var mailboxStages = _instellingenRepository.Find(Instelling.MailboxStages);
                if (mailboxStages != null)
                {
                    mailMessage.CC.Add(mailboxStages.Value);
                }
                var smtp = new SmtpClient
                {
                    Host = ConfigurationManager.AppSettings["smtpServer"],
                    EnableSsl = false,
                    Port = int.Parse(ConfigurationManager.AppSettings["smtpPort"])
                };
                // smtp.Send(mailMessage);//TODO
            }
            return Task.FromResult(0);
        }

    }
}