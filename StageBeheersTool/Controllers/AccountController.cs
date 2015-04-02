using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using StageBeheersTool.ViewModels;
using StageBeheersTool.Models.Domain;
using AutoMapper;
using System.Web.Security;
using StageBeheersTool.Models.Authentication;
using StageBeheersTool.be.hogent.webservice;

namespace StageBeheersTool.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private readonly IUserService _userService;

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
            _userService = userService;
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
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null) //geen login gevonden
            {
                ModelState.AddModelError("", Resources.ErrorOngeldigeLoginPoging);
                return View(model);
            }
            if (user.PasswordHash == null) //gebruiker heeft geen wachtwoord. Inloggen met hogent webservice
            {
                var webservice = new ldap_wrapService(); //https://webservice.hogent.be/ldap/ldap.wsdl
                var response = webservice.authenticate(model.Email, model.Password);
                var serializer = new JavaScriptSerializer();
                dynamic dataStudent = serializer.Deserialize<object>(response);
                if (dataStudent["ACTIVE"] != 0) //inloggen gelukt
                {
                    await SignInManager.SignInAsync(user, model.RememberMe, false);
                    return RedirectToAction("Index", "Stageopdracht");
                }
                else //inloggen mislukt
                {
                    ModelState.AddModelError("", Resources.ErrorOngeldigeLoginPoging);
                    return View(model);
                }
            }
            //wel een wachtwoord aanwezig(bedrijven) - niet inloggen met hogent webservice
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password,
                 model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    user = await UserManager.FindByEmailAsync(model.Email);
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
                    ModelState.AddModelError("", Resources.ErrorOngeldigeLoginPoging);
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
                    var bedrijf = Mapper.Map<RegisterBedrijfViewModel, Bedrijf>(model);

                    _userService.CreateUser(bedrijf);
                    IdentityMessage message = new IdentityMessage()
                    {
                        Subject = "Registratie",
                        Destination = bedrijf.Email,
                        Body = string.Format(Resources.EmailRegistratieBedrijf, bedrijf.Email, generatedPassword)
                    };
                    await UserManager.EmailService.SendAsync(message);
                    TempData["message"] = Resources.SuccesEmailRegistratieBedrijfVerzonden;
                    return RedirectToAction("Login", "Account");
                }
                AddErrors(result);
            }
            return View(model);
        }

        // GET: /Manage/ChangePassword 
        [Authorize]
        public ActionResult ChangePassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (UserManager.IsInBedrijfRoleOfGeenRole(user.Id) == false)
            {
                return new HttpStatusCodeResult(403);
            }
            return View(new ChangePasswordViewModel() { FirstLogin = !user.EmailConfirmed });
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (UserManager.IsInBedrijfRoleOfGeenRole(user.Id) == false)
            {
                return new HttpStatusCodeResult(403);
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
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
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                TempData["message"] = "Wachtwoord gewijzigd.";
                return RedirectToAction("Details", "Bedrijf");
            }
            AddErrors(result);
            return View(model);
        }
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Activate()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Activate(ForgotViewModel model)
        {
            if (ModelState.IsValid)
            {
                Random r = new Random();
                string generatedPassword = Membership.GeneratePassword(r.Next(10, 12), 0);
#if DEBUG
                generatedPassword = model.Email;
#endif
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, generatedPassword);
                if (result.Succeeded)
                {
                    var resultSI = await SignInManager.PasswordSignInAsync(model.Email, generatedPassword, false, shouldLockout: false);
                    switch (resultSI)
                    {
                        case SignInStatus.Success:
                            return RedirectToAction("ChangePassword");
                        case SignInStatus.Failure:
                        default:
                            ModelState.AddModelError("", Resources.ErrorOngeldigeLoginPoging);
                            return View(model);
                    }


                    //await SignInManager.SignInAsync(user, false, false);
                    //return RedirectToAction("Index", "Stageopdracht");
                    //var bedrijfRep = new BedrijfRepository(new StageToolDbContext());
                    //Bedrijf bedrijf = bedrijfRep.FindByEmail(model.Email);
                    //return RedirectToAction("Login", "Account");
                }
                AddErrors(result);

            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //Tussen admin en begeleider view switchen
        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult SwitchLoginMode(string mode)
        {
            if (CurrentUser.IsBegeleiderEnAdmin() == false)
            {
                return new HttpStatusCodeResult(403);
            }
            var identity = (ClaimsIdentity)User.Identity;
            var user = UserManager.FindByName(User.Identity.Name);
            var claims = identity.Claims.Where(c => c.Type == "Mode").ToList();
            if (claims.Count != 0)
            {
                foreach (var claim in claims)
                {
                    UserManager.RemoveClaim(user.Id, claim);
                }
            }
            UserManager.AddClaim(user.Id, new Claim("Mode", mode));
            AuthenticationManager.SignOut();
            SignInManager.SignInAsync(user, false, false);

            if (Request.UrlReferrer != null)
            {
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id))
                    || UserManager.IsInRole(user.Id, Role.Bedrijf) == false)
                {
                    return View("ForgotPasswordConfirmation");
                }
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account",
                    new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password",
                    "klik <a href=\"" + callbackUrl + "\">hier</a> om uw wachtwoord te resetten.");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null || UserManager.IsInRole(user.Id, Role.Bedrijf) == false)
            {
                // Don't reveal that the user does not exist of geen bedrijf is
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

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

        #endregion
    }
}