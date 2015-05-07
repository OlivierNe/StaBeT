using System.Linq;
using System.Threading.Tasks;

namespace StageBeheersTool.Models.Domain
{
    public interface IEmailService
    {
        Task SendAsync(string onderwerp, string inhoud, params string[] destinations);
        Task<bool> SendStandaardEmail(EmailType emailType, string reden = null, params string[] geadresseerden);

        void AddStandaardEmail(StandaardEmail standaardEmail);
        StandaardEmail FindStandaardEmailById(int id);
        StandaardEmail FindStandaardEmailByType(EmailType emailType);
        IQueryable<StandaardEmail> FindStandaardEmails();
        void UpdateStandaardEmail(StandaardEmail standaardEmail);

        void SaveChanges();
    }
}
