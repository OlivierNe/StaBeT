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

        public Admin FindAdmin()
        {
            return ctx.Admins.FirstOrDefault(admin => admin.HoGentEmail == HttpContext.Current.User.Identity.Name);
        }

        public void CreateUser<T>(T userObject) where T : class
        {
            ctx.Set<T>().Add(userObject);
        }

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
    }
}