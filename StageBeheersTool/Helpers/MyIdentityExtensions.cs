using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;
using Microsoft.AspNet.Identity;

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
                return identity.GetUserName();
            }
            return display;
        }

    }
}