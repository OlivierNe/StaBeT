
namespace StageBeheersTool.Models.Domain
{
    public interface IUserService
    {
        Bedrijf FindBedrijf();
        Student FindStudent();
        Begeleider FindBegeleider();
        bool CreateUser(Bedrijf bedrijf);
        bool CreateUser(Begeleider begeleider);
        bool CreateUser(Student student);
        void SaveChanges();
    }
}