using StageBeheersTool.Models.DAL;
using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Services
{
    public class UserService : IUserService
    {
        private StageToolDbContext ctx;

        public UserService(StageToolDbContext ctx)
        {
            this.ctx = ctx;
        }

        public Bedrijf FindBedrijf()
        {
            return ctx.Bedrijven.FirstOrDefault(bedrijf => bedrijf.Email == HttpContext.Current.User.Identity.Name);
        }

        public Student FindStudent()
        {
            return ctx.Studenten.FirstOrDefault(student => student.HogentEmail == HttpContext.Current.User.Identity.Name);
        }

        public Begeleider FindBegeleider()
        {
            return ctx.Begeleiders.FirstOrDefault(begeleider => begeleider.HogentEmail == HttpContext.Current.User.Identity.Name);
        }

        public void CreateUser<T>(T userObject) where T : class
        {
            ctx.Set<T>().Add(userObject);
        }

        private bool UserExists<T>(T userObject) where T : class
        {
            //return ctx.Set<T>().Local.Any<T>(obj => obj.Equals(userObject));
            if (userObject.GetType() == typeof(Student))
            {
                return ctx.Studenten.Any(s => s.HogentEmail.Equals(userObject));
            }


            return false;
        }

        /*private bool Exists<T>(params object[] keys)
        {
            return (ctx.Set<T>().Find(keys) != null);
        }*/

        public bool IsAdmin()
        {
            return HttpContext.Current.User.IsInRole("admin");
        }

        public bool IsBedrijf()
        {
            return HttpContext.Current.User.IsInRole("bedrijf");
        }

        public bool IsStudent()
        {
            return HttpContext.Current.User.IsInRole("student");
        }

        public bool IsBegeleider()
        {
            return HttpContext.Current.User.IsInRole("begeleider");
        }

        public void SaveChanges()
        {
            ctx.SaveChanges();
        }

        public bool UserExists(Student student)
        {
            return ctx.Studenten.Any(s => s.HogentEmail == student.HogentEmail);
        }

        public bool UserExists(Begeleider begeleider)
        {
            return ctx.Begeleiders.Any(s => s.HogentEmail == begeleider.HogentEmail);
        }

        public bool UserExists(Bedrijf bedrijf)
        {
            return ctx.Bedrijven.Any(s => s.Email == bedrijf.Email);
        }
    }
}