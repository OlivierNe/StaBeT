using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using StageBeheersTool.Models.DAL;
using StageBeheersTool.Models.Domain;

namespace StageBeheersTool.Models.Services
{
    public class EmailService : IEmailService
    {
        private readonly IInstellingenRepository _instellingenRepository;
        private readonly StageToolDbContext _dbContext;
        private readonly DbSet<StandaardEmail> _standaardEmails;

        public EmailService(IInstellingenRepository instellingenRepository,
            StageToolDbContext dbContext)
        {
            _instellingenRepository = instellingenRepository;
            _dbContext = dbContext;
            _standaardEmails = dbContext.StandaardEmails;
        }

        public Task SendAsync(string onderwerp, string inhoud, params string[] geadresseerden)
        {
            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["smtpFrom"]);
                mailMessage.Subject = onderwerp;
                mailMessage.Body = inhoud;
                mailMessage.IsBodyHtml = true;
                foreach (var geadresseerde in geadresseerden)
                {
                    mailMessage.To.Add(geadresseerde);
                }
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
                //TODO:send uit commentaar zetten
                // smtp.Send(mailMessage);
            }
            return Task.FromResult(0);
        }

        public async Task<bool> SendStandaardEmail(EmailType emailType, string reden = null, params string[] geadresseerden)
        {
            var standaardEmail = FindStandaardEmailByType(emailType);
            if (standaardEmail != null && standaardEmail.Gedeactiveerd == false)
            {
                await SendAsync(standaardEmail.Onderwerp, standaardEmail.Inhoud + "<br>" + reden, geadresseerden);
                return true;
            }
            return false;
        }

        public void AddStandaardEmail(StandaardEmail standaardEmail)
        {
            _standaardEmails.Add(standaardEmail);
            SaveChanges();
        }

        public StandaardEmail FindStandaardEmailById(int id)
        {
            return _standaardEmails.SingleOrDefault(s => s.Id == id);
        }

        public StandaardEmail FindStandaardEmailByType(EmailType emailType)
        {
            return _standaardEmails.FirstOrDefault(s => s.EmailType == emailType);
        }

        public IQueryable<StandaardEmail> FindStandaardEmails()
        {
            return _standaardEmails;
        }

        public void UpdateStandaardEmail(StandaardEmail standaardEmail)
        {
            _standaardEmails.AddOrUpdate(standaardEmail);
            SaveChanges();
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}