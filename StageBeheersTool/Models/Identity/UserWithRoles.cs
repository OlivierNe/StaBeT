using System.Collections.Generic;

namespace StageBeheersTool.Models.Identity
{
    public class UserMetRoles
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public List<string> Roles { get; set; }

        public bool IsAdmin()
        {
            return Roles.Contains(Role.Admin);
        }

        public bool IsBedrijf()
        {
            return Roles.Contains(Role.Bedrijf);
        }

        public bool IsBegeleider()
        {
            return Roles.Contains(Role.Begeleider);
        }

        public bool IsStudent()
        {
            return Roles.Contains(Role.Student);
        }

        public bool HeeftGeenRol()
        {
            return Roles.Count == 0;
        }

    }
}