using System.Collections.Generic;
using Microsoft.AspNet.Identity.Owin;
using StageBeheersTool.Models.Authentication;
using StageBeheersTool.Models.DAL;
using StageBeheersTool.Models.Domain;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Services
{
    public class UserService : IUserService
    {
        private readonly StageToolDbContext _dbContext;

        public UserService(StageToolDbContext ctx)
        {
            _dbContext = ctx;
        }

        public Bedrijf FindBedrijf()
        {
            return _dbContext.Bedrijven.FirstOrDefault(bedrijf => bedrijf.Email == HttpContext.Current.User.Identity.Name);
        }

        public Student FindStudent()
        {
            return _dbContext.Studenten.FirstOrDefault(student => student.HogentEmail == HttpContext.Current.User.Identity.Name);
        }

        public Begeleider FindBegeleider()
        {
            return _dbContext.Begeleiders.FirstOrDefault(begeleider => begeleider.HogentEmail == HttpContext.Current.User.Identity.Name);
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public bool CreateUser(Bedrijf bedrijf)
        {
            if (UserExists(bedrijf))
                return false;
            _dbContext.Bedrijven.Add(bedrijf);
            SaveChanges();
            return true;
        }

        public bool CreateUser(Begeleider begeleider)
        {
            if (UserExists(begeleider))
                return false;
            _dbContext.Begeleiders.Add(begeleider);
            SaveChanges();
            return true;
        }

        public bool CreateUser(Student student)
        {
            if (UserExists(student))
                return false;
            _dbContext.Studenten.Add(student);
            SaveChanges();
            return true;
        }

        public IEnumerable<UserMetRoles> GetUsersWithRoles()
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var users = (from u in userManager.Users
                           from ur in u.Roles
                           join r in _dbContext.Roles on ur.RoleId equals r.Id
                           group r.Name by new { u.UserName, u.Id } into user
                           select new UserMetRoles() { Id = user.Key.Id, Login = user.Key.UserName, Roles = user.ToList() }).OrderBy(s => s.Login);

            //group by u.Id);//DistinctBy(u => u.Id);
            //foreach (var user in users)
            //{
            //    users.

            //}

            return users;
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