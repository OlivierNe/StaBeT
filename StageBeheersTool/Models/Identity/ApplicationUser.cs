using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using StageBeheersTool.Helpers;
using StageBeheersTool.Models.DAL;

namespace StageBeheersTool.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            var name = userIdentity.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();
            var display = name;
            if (userIdentity.HasRole(Role.Bedrijf))
            {
                var ctx = HttpContext.Current.GetOwinContext().Get<StageToolDbContext>();
                var bedrijf = ctx.Bedrijven.FirstOrDefault(b => b.Email == name);
                if (bedrijf != null)
                    display = bedrijf.Naam;
            }
            else if (userIdentity.HasRole(Role.Student))
            {
                var ctx = HttpContext.Current.GetOwinContext().Get<StageToolDbContext>();
                var student = ctx.Studenten.FirstOrDefault(s => s.HogentEmail == name);
                if (student != null)
                {
                    if (string.IsNullOrEmpty(student.Naam) == false)
                    {
                        display = student.Naam;
                    }
                    userIdentity.AddClaim(new Claim(MyClaimTypes.StudentHeeftStage, student.HeeftToegewezenStage().ToString()));
                }
            }
            else if (userIdentity.HasRole(Role.Begeleider))
            {
                var ctx = HttpContext.Current.GetOwinContext().Get<StageToolDbContext>();
                var begeleider = ctx.Begeleiders.FirstOrDefault(b => b.HogentEmail == name);
                if (begeleider != null && string.IsNullOrEmpty(begeleider.Naam) == false)
                    display = begeleider.Naam;
            }
            if (userIdentity.HasRole(Role.Admin) && userIdentity.HasRole(Role.Begeleider))
            {
                if (userIdentity.HasClaim(c => c.Type == MyClaimTypes.LoginMode) == false)
                {
                    manager.AddClaim(Id, new Claim(MyClaimTypes.LoginMode, Role.Begeleider));
                }
            }
            userIdentity.AddClaim(new Claim(MyClaimTypes.Display, display));
            return userIdentity;
        }

    }
}