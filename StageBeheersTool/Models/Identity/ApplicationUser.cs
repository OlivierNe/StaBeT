using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
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
            if (manager.IsInRole(Id, "bedrijf"))
            {
                var ctx = HttpContext.Current.GetOwinContext().Get<StageToolDbContext>();
                var bedrijf = ctx.Bedrijven.FirstOrDefault(b => b.Email == name);
                if (bedrijf != null)
                    display = bedrijf.Naam;
            }
            else if (manager.IsInRole(Id, "student"))
            {
                var ctx = HttpContext.Current.GetOwinContext().Get<StageToolDbContext>();
                var student = ctx.Studenten.FirstOrDefault(s => s.HogentEmail == name);
                if (student != null && string.IsNullOrEmpty(student.Naam) == false)
                    display = student.Naam;
            }
            else if (manager.IsInRole(Id, "begeleider"))
            {
                var ctx = HttpContext.Current.GetOwinContext().Get<StageToolDbContext>();
                var begeleider = ctx.Begeleiders.FirstOrDefault(b => b.HogentEmail == name);
                if (begeleider != null && string.IsNullOrEmpty(begeleider.Naam) == false)
                    display = begeleider.Naam;
            }
            if (manager.IsInRole(Id, Role.Begeleider) && manager.IsInRole(Id, Role.Admin))
            {
                userIdentity.AddClaim(new Claim("Mode", Role.Begeleider)); //om te switchen tussen admin en begeleider
            }
            userIdentity.AddClaim(new Claim("Display", display));
            return userIdentity;
        }

    }
}