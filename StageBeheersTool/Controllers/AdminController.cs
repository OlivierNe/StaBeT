using AutoMapper;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using StageBeheersTool.Helpers;
using StageBeheersTool.Models.Identity;
using StageBeheersTool.ViewModels;

namespace StageBeheersTool.Controllers
{
    [Authorize(Role.Admin)]
    public class AdminController : BaseController
    {
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        // GET: Admin
        public ActionResult Index()
        {
            var admins = UserManager.GetAdmins();
            var model = admins.Select(admin =>
                new AdminVm
                {
                    Id = admin.Id,
                    Email = admin.Email,
                    IsAdmin = UserManager.IsInRole(admin.Id, Role.Admin)
                }).OrderBy(admin => admin.Email);
            return View(model.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(IList<AdminVm> model)
        {
            var validator = new HoGentPersoneelEmailAttribute();
            foreach (var adminVm in model.Where(m => m.HasChanged))
            {
                if (string.IsNullOrEmpty(adminVm.Email) && string.IsNullOrEmpty(adminVm.Id))
                {
                    break;
                }

                if (string.IsNullOrEmpty(adminVm.Id))
                {
                    var newUser = new ApplicationUser()
                    {
                        Email = adminVm.Email,
                        UserName = adminVm.Email,
                        EmailConfirmed = true
                    };
                    var user = UserManager.FindByEmail(newUser.Email);
                    if (user == null)
                    {
                        if (validator.IsValid(newUser.Email))
                        {
                            UserManager.Create(newUser);
                            adminVm.Id = newUser.Id;
                        }
                        else
                        {
                            SetViewError(newUser.Email + " is geen geldig HoGent e-mailadres.");
                            continue;
                        }
                    }
                    else
                    {
                        adminVm.Id = user.Id;
                    }
                }
                if (adminVm.IsAdmin)
                {
                    UserManager.AddToRole(adminVm.Id, Role.Admin);
                    UserManager.RemoveFromRole(adminVm.Id, Role.AdminDisabled);
                }
                else
                {
                    UserManager.AddToRole(adminVm.Id, Role.AdminDisabled);
                    UserManager.RemoveFromRole(adminVm.Id, Role.Admin);
                }
                UserManager.UpdateSecurityStamp(adminVm.Id);
            }
            SetViewMessage(Resources.SuccesAdminsGewijzigd);
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string email)
        {
            var user = UserManager.FindByEmail(email);
            if (user == null)
            {
                return HttpNotFound();
            }
            var model = Mapper.Map<AdminVm>(user);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string email)
        {
            var user = UserManager.FindByEmail(email);
            if (user == null)
            {
                return HttpNotFound();
            }
            UserManager.Delete(user);
            SetViewMessage(string.Format(Resources.SuccesAdminVerwijderd, user.Email));
            return RedirectToAction("Index");
        }

        public ActionResult DeleteFromList(string email)
        {
            var user = UserManager.FindByEmail(email);
            UserManager.RemoveFromRole(user.Id, Role.AdminDisabled);
            return RedirectToAction("Index");
        }
    }
}