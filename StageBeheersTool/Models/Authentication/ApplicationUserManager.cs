using System.Runtime.CompilerServices;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using StageBeheersTool.Models.DAL;
using StageBeheersTool.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace StageBeheersTool.Models.Authentication
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<StageToolDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                //RequiredLength = 6,
                //RequireNonLetterOrDigit = true,
                //RequireDigit = true,
                //RequireLowercase = true,
                //RequireUppercase = true,
                RequiredLength = 1
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            manager.EmailService = new EmailService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        public async Task<IdentityResult> ChangePasswordWithoutOldAsync(ApplicationUser user, string password)
        {
            await this.RemovePasswordAsync(user.Id);
            var result = await this.AddPasswordAsync(user.Id, password);
            if (result.Succeeded)
            {
                string token = await GenerateEmailConfirmationTokenAsync(user.Id);
                result = await ConfirmEmailAsync(user.Id, token);
            }
            return result;
        }

        public async override Task<IdentityResult> AddToRoleAsync(string userId, string role)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(DependencyResolver.Current.GetService(typeof(StageToolDbContext)) as StageToolDbContext));
            if (!roleManager.RoleExists(role))
                roleManager.Create(new IdentityRole(role));
            return await base.AddToRoleAsync(userId, role);
        }

        public void AddToRole(string userId, string role)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(DependencyResolver.Current.GetService(typeof(StageToolDbContext)) as StageToolDbContext));
            if (!roleManager.RoleExists(role))
                roleManager.Create(new IdentityRole(role));
            base.AddToRoleAsync(userId, role);
        }

        public IList<ApplicationUser> GetAdmins()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(DependencyResolver.Current.GetService(typeof(StageToolDbContext)) as StageToolDbContext));

            var role = roleManager.FindByName(Role.Admin);
            var admins = this.Users.Where(user => user.Roles.Select(r => r.RoleId).Contains(role.Id)).ToList();

            role = roleManager.FindByName(Role.AdminDisabled);
            var disabledadmins = this.Users.Where(user => user.Roles.Select(r => r.RoleId).Contains(role.Id));

            var users = admins.Union(disabledadmins);
            return users.ToList();
        }

        public IList<ApplicationUser> GetStudenten()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(DependencyResolver.Current.GetService(typeof(StageToolDbContext)) as StageToolDbContext));
            var role = roleManager.FindByName(Role.Student);
            return this.Users.Where(user => user.Roles.Select(r => r.RoleId).Contains(role.Id)).ToList();
        }

        public IList<ApplicationUser> GetBegeleiders()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(DependencyResolver.Current.GetService(typeof(StageToolDbContext)) as StageToolDbContext));
            var role = roleManager.FindByName(Role.Begeleider);
            return this.Users.Where(user => user.Roles.Select(r => r.RoleId).Contains(role.Id)).ToList();
        }

        public IList<ApplicationUser> GetBedrijven()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(DependencyResolver.Current.GetService(typeof(StageToolDbContext)) as StageToolDbContext));
            var role = roleManager.FindByName(Role.Bedrijf);
            return this.Users.Where(user => user.Roles.Select(r => r.RoleId).Contains(role.Id)).ToList();
        }


        public bool IsInBedrijfRoleOfGeenRole(string userId)
        {
            IList<string> roles = this.GetRoles(userId);
            return roles.Contains("bedrijf") || roles.Count == 0;
        }
    }

}