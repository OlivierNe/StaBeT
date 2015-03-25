
namespace StageBeheersTool.Models.Domain
{
    public interface IUserService
    {
        Bedrijf FindBedrijf();
        Student FindStudent();
        Begeleider FindBegeleider();
        bool IsAdmin();
        bool IsBedrijf();
        bool IsStudent();
        bool IsBegeleider();
        bool CreateUser(Bedrijf bedrijf);
        bool CreateUser(Begeleider begeleider);
        bool CreateUser(Student student);
        void SaveChanges();
    }
}