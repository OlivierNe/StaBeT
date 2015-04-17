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
using StageBeheersTool.Helpers;
using StageBeheersTool.Models.Identity;
using StageBeheersTool.ViewModels;
using StageBeheersTool.Models.Domain;
using AutoMapper;
using System.Web.Security;
using StageBeheersTool.be.hogent.webservice;

namespace StageBeheersTool.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

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

        public AccountController(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        #region Accounts beheren
        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult List(AccountListVM model)
        {
            var users = _userService.GetUsersWithRoles()
                .Where(user => (string.IsNullOrWhiteSpace(model.LoginZoeken) ||
                    user.Login.ToLower().Contains(model.LoginZoeken.ToLower())) && ((model.IsAdmin && user.IsAdmin())
                    || (model.IsBegeleider && user.IsBegeleider()) || (model.IsStudent && user.IsStudent())
                    || (model.IsBedrijf && user.IsBedrijf())));
            model.SetUsers(users);
            model.ToonActies = CurrentUser.IsAdmin();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_UserList", model);
            }
            return View(model);
        }

        //TODO:Import studenten stage uit bamaflex.
        [Authorize(Role.Admin)]
        public ActionResult Create()
        {
            return View(new AccountCreateVM());
        }

        [Authorize(Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountCreateVM model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }
            if (string.IsNullOrWhiteSpace(model.Wachtwoord))//TODO:tijdelijk "wachtwoord" om te testen
            {
                model.Wachtwoord = "wachtwoord";
            }
            var user = _userService.CreateLogin(model.Email, model.Wachtwoord);
            model.Title = "Nieuw account";
            if (user != null)
            {
                if (model.Admin)
                {
                    await UserManager.AddToRoleAsync(user.Id, Role.Admin);
                }
                if (model.Begeleider)
                {
                    await UserManager.AddToRoleAsync(user.Id, Role.Begeleider);
                    _userService.CreateUserObject(new Begeleider { HogentEmail = model.Email });
                }
                if (model.Student)
                {
                    await UserManager.AddToRoleAsync(user.Id, Role.Student);
                    _userService.CreateUserObject(new Student { HogentEmail = model.Email });
                }
                if (model.Bedrijf)
                {
                    await UserManager.AddToRoleAsync(user.Id, Role.Bedrijf);
                    _userService.CreateUserObject(new Bedrijf { Email = model.Email, Naam = model.Email });
                }
                SetViewMessage(string.Format(Resources.SuccesAanmakenAccount, user.UserName));
            }
            else
            {
                SetViewError(string.Format(Resources.ErrorAanmakenAccount, user.Email));
                return View(model);
            }
            return RedirectToAction("List");
        }

        [Authorize(Role.Admin)]
        public ActionResult Edit(string id)
        {
            var user = UserManager.FindById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var model = new AccountCreateVM
            {
                Admin = UserManager.IsInRole(user.Id, Role.Admin),
                Begeleider = UserManager.IsInRole(user.Id, Role.Begeleider),
                Student = UserManager.IsInRole(user.Id, Role.Student),
                Bedrijf = UserManager.IsInRole(user.Id, Role.Bedrijf),
                Email = user.UserName,
                Id = user.Id,
                Title = "Account wijzigen"
            };
            return View("Create", model);
        }

        [Authorize(Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AccountCreateVM model)
        {
            if (ModelState.IsValid == false)
            {
                return View("Create", model);
            }
            var user = await UserManager.FindByIdAsync(model.Id);

            var result = await EditAccount(model, user);

            if (result.Succeeded)
            {
                SetViewMessage(string.Format(Resources.SuccesEditAccount, user.UserName));
            }
            else
            {
                SetViewError(string.Format(Resources.ErrorCreateAccount, user.Email));
            }
            return RedirectToAction("List");
        }

        [Authorize(Role.Admin)]
        [HttpPost]
        public async Task<ActionResult> AjaxEdit(AccountCreateVM model)
        {
            if (ModelState.IsValid == false)
            {
                var errorMessage = "";
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        errorMessage += error.ErrorMessage;
                    }
                }
                return Json(new { type = "error", message = errorMessage });
            }
            var user = await UserManager.FindByIdAsync(model.Id);

            var result = await EditAccount(model, user);

            var message = new { type = "", message = "" };
            if (result.Succeeded)
            {
                return Json(new
                {
                    type = "success",
                    message = "Account '" + user.UserName + "' succesvol gewijzigd.",
                    model.Admin,
                    model.Begeleider,
                    model.Student,
                    model.Bedrijf,
                    model.Email
                });
            }
            message = new { type = "error", message = string.Format(Resources.ErrorCreateAccount, user.Email) };
            return Json(message);
        }

        private async Task<IdentityResult> EditAccount(AccountCreateVM model, ApplicationUser user)
        {
            if (model.Admin)
            {
                await UserManager.AddToRoleAsync(user.Id, Role.Admin);
            }
            else
            {
                await UserManager.RemoveFromRoleAsync(user.Id, Role.Admin);
            }
            if (model.Begeleider)
            {
                await UserManager.AddToRoleAsync(user.Id, Role.Begeleider);
                _userService.CreateUserObject(new Begeleider { HogentEmail = model.Email });
            }
            else
            {
                await UserManager.RemoveFromRoleAsync(user.Id, Role.Begeleider);
            }
            if (model.Student)
            {
                await UserManager.AddToRoleAsync(user.Id, Role.Student);
                _userService.CreateUserObject(new Student { HogentEmail = model.Email });
            }
            else
            {
                await UserManager.RemoveFromRoleAsync(user.Id, Role.Student);
            }
            if (model.Bedrijf)
            {
                await UserManager.AddToRoleAsync(user.Id, Role.Bedrijf);
                await UserManager.ChangePasswordWithoutOldAsync(user, model.Wachtwoord);
                _userService.CreateUserObject(new Bedrijf { Email = model.Email, Naam = model.Email });
            }
            else
            {
                await UserManager.RemoveFromRoleAsync(user.Id, Role.Bedrijf);
            }
            user.Email = model.Email;
            user.UserName = model.Email;
            return await UserManager.UpdateAsync(user);
        }


        [Authorize(Role.Admin)]
        public ActionResult Delete(string id)
        {
            var user = UserManager.FindById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [Authorize(Role.Admin)]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var user = UserManager.FindById(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            await UserManager.RemoveAllClaims(user.Id);
            var result = await UserManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                SetViewMessage(string.Format(Resources.SuccesDeleteAccount, user.UserName));
                return RedirectToAction("List");
            }
            SetViewError(string.Format(Resources.ErrorDeleteAccount, user.UserName));
            return View(user);
        }
        #endregion

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
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
                    return RedirectToAction("List", "Stageopdracht");
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
                        return RedirectToAction("List", "Stageopdracht");
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
                    _userService.CreateUserObject(bedrijf);

                    await _emailService.SendAsync(EmailMessages.BedrijfGeregistreerd(user.Email, generatedPassword));
                    SetViewMessage(Resources.SuccesEmailRegistratieBedrijfVerzonden);
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
                        return RedirectToAction("List", "Stageopdracht");
                    }
                }
            }
            result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                SetViewMessage(Resources.SuccesChangePassword);
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