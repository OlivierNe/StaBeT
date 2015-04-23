using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using StageBeheersTool.Models.Identity;

namespace StageBeheersTool.Helpers
{
    public static class IdentityHelpers
    {
        public static string GetDisplayName()
        {
            var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            var display = claims.Where(c => c.Type == MyClaimTypes.Display).Select(c => c.Value).FirstOrDefault();
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
            var mode = claims.Where(c => c.Type == MyClaimTypes.LoginMode).Select(c => c.Value).FirstOrDefault();
            return mode;
        }

        public static bool StudentHeeftStage()
        {
            var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            var heeftStageClaim = claims.Where(c => c.Type == MyClaimTypes.StudentHeeftStage).Select(c => c.Value).FirstOrDefault();
            if (heeftStageClaim == null)
            {
                return false;
            }
            bool heeftStage;
            return Boolean.TryParse(heeftStageClaim, out heeftStage) && heeftStage;
        }

        public static bool HasRole(this ClaimsIdentity identity, string role)
        {
            return identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role && c.Value == role) != null;
        }
    }
}