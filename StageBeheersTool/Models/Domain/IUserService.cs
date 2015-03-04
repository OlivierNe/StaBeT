using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{
    public interface IUserService
    {
        Bedrijf FindBedrijf();
        Student FindStudent();
        Begeleider FindBegeleider();
        Admin FindAdmin();
        bool IsAdmin();
        bool IsBedrijf();
        bool IsStudent();
        bool IsBegeleider();
        void CreateUser<T>(T userObject) where T : class;
        void SaveChanges();
    }
}