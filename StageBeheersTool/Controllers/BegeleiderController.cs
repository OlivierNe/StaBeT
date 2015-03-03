using AutoMapper;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.IO;

namespace StageBeheersTool.Controllers
{
    public class BegeleiderController : Controller
    {
        private IBegeleiderRepository begeleiderRepository;

        public BegeleiderController(IBegeleiderRepository begeleiderRepository)
        {
            this.begeleiderRepository = begeleiderRepository;
        }

        public ActionResult Details()
        {
            var begeleider = CurrentBegeleider();
            return View(begeleider);
        }

        [Authorize(Roles = "begeleider")]
        public ActionResult Edit()
        {
            var begeleider = CurrentBegeleider();
            var model = Mapper.Map<BegeleiderEditVM>(begeleider);
            return View(model);
        }

        [Authorize(Roles = "begeleider")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BegeleiderEditVM model, HttpPostedFileBase fotoFile)
        {
            var begeleider = CurrentBegeleider();
            if (fotoFile != null && fotoFile.ContentLength > 0 && fotoFile.ContentType.StartsWith("image/"))
            {
                if (fotoFile.ContentLength > 512000)
                {
                    ModelState.AddModelError(string.Empty, "Ongeldige afbeelding grootte, max. 500kb.");
                    return View(model);
                }
                else
                {
                    string oldFotoUrl = begeleider.FotoUrl;
                    if (System.IO.File.Exists(oldFotoUrl))
                    {
                        System.IO.File.Delete(oldFotoUrl);
                    }
                    string filename = User.Identity.GetUserId() + Path.GetExtension(fotoFile.FileName);
                    string relativePath = "~/Images/Begeleider/" + filename;
                    model.FotoUrl = relativePath;
                    string absolutePath = Path.Combine(Server.MapPath("~/Images/Begeleider"), Path.GetFileName(filename));
                    fotoFile.SaveAs(absolutePath);
                }
            }
            var newBegeleider = Mapper.Map<Begeleider>(model);
            begeleiderRepository.Update(begeleider, newBegeleider);
            begeleiderRepository.SaveChanges();
            TempData["message"] = "Gegevens gewijzigd.";
            return RedirectToAction("Details");
        }

        #region Helpers
        private Begeleider CurrentBegeleider()
        {
            return begeleiderRepository.FindByEmail(User.Identity.Name);
        }
        #endregion

    }
}