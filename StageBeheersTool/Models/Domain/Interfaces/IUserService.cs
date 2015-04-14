using System.Collections.Generic;
using StageBeheersTool.Models.Authentication;

namespace StageBeheersTool.Models.Domain
{
    public interface IUserService
    {
        Bedrijf FindBedrijf();
        Student FindStudent();
        Begeleider FindBegeleider();
        void AddRolesToUser(ApplicationUser user, params string[] roles);
        bool CreateUserObject(Bedrijf bedrijf);
        bool CreateUserObject(Begeleider begeleider);
        bool CreateUserObject(Student student);
        IEnumerable<UserMetRoles> GetUsersWithRoles();
        ApplicationUser CreateLogin(string email, string wachtwoord, params string[] roles);
        void DeleteLogin(string email);
        void SaveChanges();
    }
}