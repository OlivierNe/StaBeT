﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using DocumentFormat.OpenXml.Math;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using StageBeheersTool.Models.DAL;
using StageBeheersTool.Models.Domain;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using StageBeheersTool.Models.Identity;

namespace StageBeheersTool.Models.Services
{
    public class UserService : IUserService
    {
        private readonly StageToolDbContext _dbContext;
        private readonly IDbSet<ApplicationUser> _users;

        private ApplicationUserManager _userManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        public UserService(StageToolDbContext ctx)
        {
            _dbContext = ctx;
            _users = ctx.Users;
        }

        public Bedrijf GetBedrijf()
        {
            return _dbContext.Bedrijven.FirstOrDefault(bedrijf => bedrijf.Email == HttpContext.Current.User.Identity.Name);
        }

        public Student GetStudent()
        {
            return _dbContext.Studenten.FirstOrDefault(student => student.HogentEmail == HttpContext.Current.User.Identity.Name);
        }

        public Begeleider GetBegeleider()
        {
            return _dbContext.Begeleiders.FirstOrDefault(begeleider => begeleider.HogentEmail == HttpContext.Current.User.Identity.Name);
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public bool CreateUserObject(Bedrijf bedrijf)
        {
            if (UserExists(bedrijf))
                return false;
            _dbContext.Bedrijven.Add(bedrijf);
            SaveChanges();
            return true;
        }

        public bool CreateUserObject(Begeleider begeleider)
        {
            if (UserExists(begeleider))
                return false;
            _dbContext.Begeleiders.Add(begeleider);
            SaveChanges();
            return true;
        }

        public bool CreateUserObject(Student student)
        {
            if (UserExists(student))
                return false;
            _dbContext.Studenten.Add(student);
            SaveChanges();
            return true;
        }

        public IEnumerable<UserMetRoles> GetUsersWithRoles()
        {
            var users = (from u in _userManager.Users
                         from ur in u.Roles
                         join r in _dbContext.Roles on ur.RoleId equals r.Id
                         group r.Name by new { u.UserName, u.Id } into user
                         select new UserMetRoles() { Id = user.Key.Id, Login = user.Key.UserName, Roles = user.ToList() }).OrderBy(s => s.Login);
            return users;
        }


        public void AddRolesToUser(ApplicationUser user, params string[] roles)
        {
            foreach (var role in roles)
            {
                _userManager.AddToRole(user.Id, role);
            }
        }

        public ApplicationUser CreateLogin(string email, string wachtwoord = null, params string[] roles)
        {
            var user = _users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                AddRolesToUser(user, roles);
                return user;
            }
            user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
            IdentityResult result;
            if (string.IsNullOrWhiteSpace(wachtwoord))
            {
                result = _userManager.Create(user);
            }
            else
            {
                result = _userManager.Create(user, wachtwoord);
            }
            if (result.Succeeded)
            {
                AddRolesToUser(user, roles);
            }
            return user;
        }


        public void CreateLogins(List<string> emailList, params string[] roles)
        {
            foreach (var email in emailList)
            {
                var user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
                var result = _userManager.Create(user);
                if (result.Succeeded)
                {
                    AddRolesToUser(user, roles);
                }
            }
        }

        public void DeleteAlleStudentAccounts()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_dbContext));
            IdentityRole role = roleManager.FindByName(Role.Student);
            var studentAccounts = _users.Where(user => user.Roles.Any(identRole => identRole.RoleId == role.Id)).ToList();
            foreach (var user in studentAccounts)
            {
                _users.Remove(user);
            }
            _dbContext.SaveChanges();
        }

        public void DeleteLogin(string email)
        {
            var user = _userManager.FindByEmail(email);
            if (user != null)
            {
                var claims = user.Claims.ToArray();
                foreach (var claim in claims)
                {
                    _userManager.RemoveClaim(claim.UserId, new Claim(claim.ClaimType, claim.ClaimValue));
                }
                _userManager.Delete(user);
            }
        }

        public void UpdateSecurityStamp(string email)
        {
            var user = _userManager.FindByEmail(email);
            if (user != null)
            {
                _userManager.UpdateSecurityStamp(user.Id);
            }
        }

        #region helpers
        private bool UserExists(Student student)
        {
            return _dbContext.Studenten.Any(s => s.HogentEmail == student.HogentEmail);
        }

        private bool UserExists(Begeleider begeleider)
        {
            return _dbContext.Begeleiders.Any(s => s.HogentEmail == begeleider.HogentEmail);
        }

        private bool UserExists(Bedrijf bedrijf)
        {
            return _dbContext.Bedrijven.Any(s => s.Email == bedrijf.Email);
        }
        #endregion


    }

}