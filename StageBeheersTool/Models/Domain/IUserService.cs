
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
        void CreateUser<T>(T userObject) where T : class;
        bool UserExists(Student student);
        bool UserExists(Begeleider begeleider);
        bool UserExists(Bedrijf bedrijf);
        void SaveChanges();
    }
}