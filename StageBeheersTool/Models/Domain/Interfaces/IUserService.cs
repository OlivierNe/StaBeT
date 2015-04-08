using System.Collections.Generic;
using StageBeheersTool.Models.Authentication;
using StageBeheersTool.ViewModels;

namespace StageBeheersTool.Models.Domain
{
    public interface IUserService
    {
        Bedrijf FindBedrijf();
        Student FindStudent();
        Begeleider FindBegeleider();
        IEnumerable<UserMetRoles> GetUsersWithRoles();
        bool CreateUser(Bedrijf bedrijf);
        bool CreateUser(Begeleider begeleider);
        bool CreateUser(Student student);
        void SaveChanges();
    }
}