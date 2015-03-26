using AutoMapper;
using Microsoft.AspNet.Identity;
using StageBeheersTool.Models.Authentication;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using StageBeheersTool.ViewModels;


namespace StageBeheersTool.Controllers
{
    [Authorize(Role.Admin)]
    public class AdminController : Controller
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
                new AdminVm() { Id = admin.Id, Email = admin.Email, IsAdmin = UserManager.IsInRole(admin.Id, "admin") }).OrderBy(admin => admin.Email);
            return View(model.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(IList<AdminVm> model)
        {

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
                        UserManager.Create(newUser);
                        adminVm.Id = newUser.Id;
                    }
                    else
                    {
                        adminVm.Id = user.Id;
                    }
                }
                if (adminVm.IsAdmin)
                {
                    UserManager.AddToRole(adminVm.Id, "admin");
                    UserManager.RemoveFromRole(adminVm.Id, "adminDisabled");
                }
                else
                {
                    UserManager.AddToRole(adminVm.Id, "adminDisabled");
                    UserManager.RemoveFromRole(adminVm.Id, "admin");
                }
                UserManager.UpdateSecurityStamp(adminVm.Id); 
            }

            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(string id)
        {

            return View();
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
            return RedirectToAction("Index");
        }

    }
}