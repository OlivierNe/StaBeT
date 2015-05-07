using System.Linq;

namespace StageBeheersTool.Models.Domain
{
    public interface IInstellingenRepository
    {
        IQueryable<Instelling> FindAll();
        Instelling Find(string key);
        void AddOrUpdate(Instelling instelling);

        void AddStandaardEmail(StandaardEmail standaardEmail);
        StandaardEmail FindStandaardEmailById(int id);
        StandaardEmail FindStandaardEmailByType(EmailType emailType);
        IQueryable<StandaardEmail> FindStandaardEmails();
        void UpdateStandaardEmail(StandaardEmail standaardEmail);

        void AddAcademiejaarInstelling(AcademiejaarInstellingen academiejaar);
        AcademiejaarInstellingen FindAcademiejaarInstellingVanHuidig();
        AcademiejaarInstellingen FindAcademiejaarInstellingByAcademiejaar(string academiejaar);
        IQueryable<AcademiejaarInstellingen> FindAllAcademiejaarInstellingen();
        void UpdateAcademiejaarInstelling(AcademiejaarInstellingen academiejaar);
        void DeleteAcademiejaarInstelling(AcademiejaarInstellingen academiejaar);

        void SaveChanges();
    }
}
