using System.Web;
using StageBeheersTool.Helpers;

namespace StageBeheersTool.Models.Authentication
{
    public class CurrentUser
    {
        public static bool IsAdmin()
        {
            var user = HttpContext.Current.User;
            if (user.IsInRole(Role.Admin) && user.IsInRole(Role.Begeleider))
            {
                return IdentityHelpers.GetMode() == Role.Admin;
            }
            return user.IsInRole(Role.Admin);
        }

        public static bool IsBedrijf()
        {
            return HttpContext.Current.User.IsInRole("bedrijf");
        }

        public static bool IsStudent()
        {
            return HttpContext.Current.User.IsInRole("student");
        }

        public static bool IsBegeleider()
        {
            var user = HttpContext.Current.User;
            if (user.IsInRole(Role.Admin) && user.IsInRole(Role.Begeleider))
            {
                return IdentityHelpers.GetMode() == Role.Begeleider;
            }

            return user.IsInRole(Role.Begeleider);
        }

        public static bool IsBegeleiderEnAdmin()
        {
            var user = HttpContext.Current.User;
            return user.IsInRole(Role.Admin) && user.IsInRole(Role.Begeleider);
        }

    }
}