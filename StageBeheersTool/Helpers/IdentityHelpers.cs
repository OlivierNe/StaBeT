using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace StageBeheersTool.Helpers
{
    public static class IdentityHelpers
    {
        public static string GetDisplayName()
        {
            var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            var display = claims.Where(c => c.Type == "Display").Select(c => c.Value).SingleOrDefault();
            if (string.IsNullOrWhiteSpace(display))
            {
                return identity.Name;
            }
            return display;
        }

        public static string GetMode()
        {
            var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            var mode = claims.Where(c => c.Type == "Mode").Select(c => c.Value).FirstOrDefault();
            return mode;
        }

    }
}