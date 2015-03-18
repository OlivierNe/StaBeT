using StageBeheersTool.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;

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
            return View();
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

        public ActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(string id)
        {
            return View();
        }

        public ActionResult Delete()
        {
            return View();
        }
        
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost()
        {
            return View();
        }



    }
}