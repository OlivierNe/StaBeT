using System.Web;

namespace StageBeheersTool.Models.Authentication
{
    public class CurrentUser
    {
        public static bool IsAdmin()
        {
            return HttpContext.Current.User.IsInRole("admin");
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
            return HttpContext.Current.User.IsInRole("begeleider");
        }



    }
}