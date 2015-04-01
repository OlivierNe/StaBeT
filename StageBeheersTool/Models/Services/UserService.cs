using StageBeheersTool.Models.DAL;
using StageBeheersTool.Models.Domain;
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

        public void SaveChanges()
        {
            ctx.SaveChanges();
        }

        public bool CreateUser(Bedrijf bedrijf)
        {
            if (UserExists(bedrijf))
                return false;
            ctx.Bedrijven.Add(bedrijf);
            SaveChanges();
            return true;
        }

        public bool CreateUser(Begeleider begeleider)
        {
            if (UserExists(begeleider))
                return false;
            ctx.Begeleiders.Add(begeleider);
            SaveChanges();
            return true;
        }

        public bool CreateUser(Student student)
        {
            if (UserExists(student))
                return false;
            ctx.Studenten.Add(student);
            SaveChanges();
            return true;
        }

        #region helpers
        private bool UserExists(Student student)
        {
            return ctx.Studenten.Any(s => s.HogentEmail == student.HogentEmail);
        }

        private bool UserExists(Begeleider begeleider)
        {
            return ctx.Begeleiders.Any(s => s.HogentEmail == begeleider.HogentEmail);
        }

        private bool UserExists(Bedrijf bedrijf)
        {
            return ctx.Bedrijven.Any(s => s.Email == bedrijf.Email);
        }
        #endregion
    }
}