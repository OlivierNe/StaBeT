using System;
using System.Collections;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using StageBeheersTool.Models;
using StageBeheersTool.ViewModels;
using StageBeheersTool.Models.DAL;
using StageBeheersTool.Models.Domain;
using AutoMapper;
using System.Web.Security;
using Newtonsoft.Json;
using StageBeheersTool.Models.Authentication;

namespace StageBeheersTool.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private IUserService userService;

        public ApplicationSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
            private set { _signInManager = value; }
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        public AccountController(IUserService userService)
        {
            this.userService = userService;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.Email.EndsWith("@student.hogent.be")) //student
            {
                //TODO: service aanspreken:
                //https://webservice.hogent.be/ldap/ldap.wsdl
                //TODO: if( geldig hogent account + wachtwoord)...
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null) //eerste login
                {
                    user = new ApplicationUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
                    await UserManager.CreateAsync(user);
                    await UserManager.AddToRoleAsync(user.Id, "student");
                    Student student = new Student() { HogentEmail = model.Email };
                    userService.CreateUser<Student>(student);
                    userService.SaveChanges();
                }
                await SignInManager.SignInAsync(user, model.RememberMe, false);
                return RedirectToAction("Index", "Stageopdracht");
            }
            else if (model.Email.EndsWith("@hogent.be")) //begeleider of admin
            {
                //TODO: service aanspreken:
                //https://webservice.hogent.be/ldap/ldap.wsdl
                //TODO: if( geldig hogent account + wachtwoord)...
                
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    user = new ApplicationUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
                    await UserManager.CreateAsync(user);
                    await UserManager.AddToRoleAsync(user.Id, "begeleider");//of admin
                    Begeleider begeleider = new Begeleider() { HogentEmail = model.Email };
                    userService.CreateUser<Begeleider>(begeleider);
                    userService.SaveChanges();
                }
                await SignInManager.SignInAsync(user, model.RememberMe, false);
                return RedirectToAction("Index", "Stageopdracht");
            }
            else if (model.Email.EndsWith("@admin.be"))
            {//tijdelijk




            }
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    var user = await UserManager.FindByEmailAsync(model.Email);
                    if (user.EmailConfirmed)
                    {
                        return RedirectToAction("Index", "Stageopdracht");
                    }
                    else
                    {
                        return RedirectToAction("ChangePassword");
                    }
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterBedrijfViewModel model)
        {
            if (ModelState.IsValid)
            {
                Random r = new Random();
                string generatedPassword = Membership.GeneratePassword(r.Next(10, 12), 0);
#if DEBUG
                generatedPassword = "wachtwoord";
#endif
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, generatedPassword);
                if (result.Succeeded)
                {
                    Bedrijf bedrijf = Mapper.Map<RegisterBedrijfViewModel, Bedrijf>(model);
                    userService.CreateUser<Bedrijf>(bedrijf);
                    userService.SaveChanges();
                    IdentityMessage message = new IdentityMessage()
                    {
                        Subject = "Registratie",
                        Destination = bedrijf.Email,
                        Body = string.Format("<strong>Account aangemaakt: </strong><ul><li>Login: {0}</li><li>Wachtwoord: {1}</li></ul>",
                        bedrijf.Email, generatedPassword)
                    };

                    await UserManager.EmailService.SendAsync(message);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    TempData["message"] = "Registratie e-mail verzonden.";
                    return RedirectToAction("Login", "Account");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            return View(new ChangePasswordViewModel() { FirstLogin = !user.EmailConfirmed });
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            IdentityResult result = null;
            if (!user.EmailConfirmed)
            {
                result = await UserManager.ChangePasswordWithoutOldAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, "bedrijf");
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToAction("Index", "Stageopdracht");
                    }
                }
            }
            result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                TempData["message"] = "Wachtwoord gewijzigd.";
                return RedirectToAction("Details", "Bedrijf");
            }
            AddErrors(result);
            return View(model);
        }





        ////
        //// GET: /Account/ConfirmEmail
        //[AllowAnonymous]
        //public async Task<ActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return View("Error");
        //    }
        //    var result = await UserManager.ConfirmEmailAsync(userId, code);
        //    return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //}

        ////
        //// GET: /Account/ForgotPassword
        //[AllowAnonymous]
        //public ActionResult ForgotPassword()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/ForgotPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await UserManager.FindByNameAsync(model.Email);
        //        if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
        //        {
        //            // Don't reveal that the user does not exist or is not confirmed
        //            return View("ForgotPasswordConfirmation");
        //        }

        //        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //        // Send an email with this link
        //        // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        //        // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
        //        // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
        //        // return RedirectToAction("ForgotPasswordConfirmation", "Account");
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        ////
        //// GET: /Account/ForgotPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ForgotPasswordConfirmation()
        //{
        //    return View();
        //}

        ////
        //// GET: /Account/ResetPassword
        //[AllowAnonymous]
        //public ActionResult ResetPassword(string code)
        //{
        //    return code == null ? View("Error") : View();
        //}

        ////
        //// POST: /Account/ResetPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var user = await UserManager.FindByNameAsync(model.Email);
        //    if (user == null)
        //    {
        //        // Don't reveal that the user does not exist
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    AddErrors(result);
        //    return View();
        //}

        ////
        //// GET: /Account/ResetPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ResetPasswordConfirmation()
        //{
        //    return View();
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}