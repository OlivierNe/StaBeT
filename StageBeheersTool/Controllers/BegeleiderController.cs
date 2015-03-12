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
        private IImageService imageService;
        private IUserService userService;

        public BegeleiderController(IBegeleiderRepository begeleiderRepository, IImageService imageService,
            IUserService userservice)
        {
            this.begeleiderRepository = begeleiderRepository;
            this.imageService = imageService;
            this.userService = userservice;
        }

        [Authorize(Roles = "begeleider, admin")]
        public ActionResult Details(int? id)
        {
            Begeleider begeleider = null;
            if (id == null)
            {
                begeleider = userService.FindBegeleider();
                return View(new BegeleiderDetailsVM() { Begeleider = begeleider, ToonEdit = true });
            }
            begeleider = begeleiderRepository.FindById((int)id);
            if (begeleider == null)
            {
                return HttpNotFound();
            }
            return View(new BegeleiderDetailsVM() { Begeleider = begeleider });
        }

        [Authorize(Roles = "begeleider")]
        public ActionResult Edit()
        {
            var begeleider = userService.FindBegeleider();
            var model = Mapper.Map<BegeleiderEditVM>(begeleider);
            return View(model);
        }

        [Authorize(Roles = "begeleider")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BegeleiderEditVM model, HttpPostedFileBase fotoFile)
        {
            var begeleider = userService.FindBegeleider();
            if (imageService.IsValidImage(fotoFile))
            {
                if (imageService.HasValidSize(fotoFile))
                {
                    model.FotoUrl = imageService.SaveImage(fotoFile, begeleider.FotoUrl, "~/Images/Begeleider");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Ongeldige afbeelding grootte, max. " + (imageService.MaxSize() / 1024) + " Kb.");
                    return View(model);
                }
            }
            var newBegeleider = Mapper.Map<Begeleider>(model);
            begeleiderRepository.Update(begeleider, newBegeleider);
            begeleiderRepository.SaveChanges();
            TempData["message"] = "Gegevens gewijzigd.";
            return RedirectToAction("Details");
        }

    }
}