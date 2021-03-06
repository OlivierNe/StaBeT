﻿using System.Collections.Generic;
using StageBeheersTool.Models.Identity;

namespace StageBeheersTool.Models.Domain
{
    public interface IUserService
    {
        Bedrijf GetBedrijf();
        Student GetStudent();
        Begeleider GetBegeleider();
        void AddRolesToUser(ApplicationUser user, params string[] roles);
        bool CreateUserObject(Bedrijf bedrijf);
        bool CreateUserObject(Begeleider begeleider);
        bool CreateUserObject(Student student);
        IEnumerable<UserMetRoles> GetUsersWithRoles();
        ApplicationUser CreateLogin(string email, string wachtwoord = null, params string[] roles);
        void CreateLogins(List<string> emailList, params string[] roles);
        void DeleteLogin(string email);
        void DeleteAlleStudentAccounts();
        void UpdateSecurityStamp(string email);
        void SaveChanges();
    }
}