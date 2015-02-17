using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using StageBeheersTool.Models;
using StageBeheersTool.ViewModels;
using StageBeheersTool.Models.DAL;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using System.Diagnostics;



namespace StageBeheersTool
{
    public class EmailService : IIdentityMessageService
    {

        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            //testemail587123@gmail.com
            //Mijnwachtwoord123
            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["smtpFrom"]);
                mailMessage.Subject = message.Subject;
                mailMessage.Body = message.Body;
                mailMessage.IsBodyHtml = true;
                mailMessage.To.Add(new MailAddress(message.Destination));

                NetworkCredential NetworkCredential = new NetworkCredential();
                NetworkCredential.UserName = mailMessage.From.Address;
                NetworkCredential.Password = ConfigurationManager.AppSettings["smtpFromPw"];

                SmtpClient smtp = new SmtpClient();
                smtp.Host = ConfigurationManager.AppSettings["smtpServer"];
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCredential;
                smtp.Port = int.Parse(ConfigurationManager.AppSettings["smtpPort"]);
                try
                {
                    smtp.Send(mailMessage);
                }
                catch (Exception e)
                {
                    Task.FromResult(1);
                }
            }
            return Task.FromResult(0);
        }

    }

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
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new StageToolDbContext()));
            if (!roleManager.RoleExists(role))
                roleManager.Create(new IdentityRole(role));
            return await base.AddToRoleAsync(userId, role);
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
        public async Task<SignInStatus> PasswordSignInEmailAsync(string email, string password, bool isPersistent, bool shouldLockout)
        {
            var user = await UserManager.FindByEmailAsync(email);
            return await base.PasswordSignInAsync(user.UserName, password, isPersistent, shouldLockout);
        }
    }

}
