﻿using AutoMapper;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.ViewModels;
using System.Web;
using System.Web.Mvc;
using StageBeheersTool.Models.Authentication;

namespace StageBeheersTool.Controllers
{
    public class BegeleiderController : Controller
    {
        private readonly IBegeleiderRepository _begeleiderRepository;
        private readonly IImageService _imageService;
        private readonly IUserService _userService;

        public BegeleiderController(IBegeleiderRepository begeleiderRepository, IImageService imageService,
            IUserService userservice)
        {
            _begeleiderRepository = begeleiderRepository;
            _imageService = imageService;
            _userService = userservice;
        }

        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult Index()
        {
            var begeleiders = _begeleiderRepository.FindAll();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_begeleidersList", begeleiders);
            }
            return View(begeleiders);
        }

        [Authorize(Role.Begeleider, Role.Admin)]
        public ActionResult Details(int id)
        {
            var begeleider = FindBegeleider(id);
            if (begeleider == null)
            {
                return HttpNotFound();
            }
            var model = Mapper.Map<BegeleiderDetailsVM>(begeleider);
            model.ToonEdit = begeleider.Equals(_userService.FindBegeleider());
            model.ToonTerugNaarLijst = id != 0;
            return View(model);
        }

        [Authorize(Role.Begeleider, Role.Admin)]
        public ActionResult Edit(int id)
        {
            Begeleider begeleider;
            if (CurrentUser.IsAdmin())
            {
                begeleider = FindBegeleider(id);
                if (begeleider == null)
                {
                    return HttpNotFound();
                }
            }
            else
            {
                begeleider = _userService.FindBegeleider();
            }
            var model = Mapper.Map<BegeleiderEditVM>(begeleider);
            return View(model);
        }

        [Authorize(Role.Begeleider, Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BegeleiderEditVM model, HttpPostedFileBase fotoFile)
        {
            Begeleider begeleider;
            if (CurrentUser.IsAdmin())
            {
                begeleider = FindBegeleider(model.Id);
                if (begeleider == null)
                {
                    return HttpNotFound();
                }
            }
            else
            {
                begeleider = _userService.FindBegeleider();
            }

            if (_imageService.IsValidImage(fotoFile))
            {
                if (_imageService.HasValidSize(fotoFile))
                {
                    model.FotoUrl = _imageService.SaveImage(fotoFile, begeleider.FotoUrl, "~/Images/Begeleider");
                }
                else
                {
                    ModelState.AddModelError("", string.Format(Resources.ErrorOngeldigeAfbeeldingGrootte, (_imageService.MaxSize() / 1024)));
                    return View(model);
                }
            }
            var begeleiderModel = Mapper.Map<Begeleider>(model);
            _begeleiderRepository.Update(begeleiderModel);
            TempData["message"] = "Gegevens gewijzigd.";
            return RedirectToAction("Details", new { id = model.Id });
        }


        #region helpers

        private Begeleider FindBegeleider(int id)
        {
            if (id == 0)
            {
                return _userService.FindBegeleider();
            }
            else
            {
                return _begeleiderRepository.FindById(id);
            }
        }
        #endregion
    }
}