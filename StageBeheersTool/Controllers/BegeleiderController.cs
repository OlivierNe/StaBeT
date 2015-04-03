using AutoMapper;
using StageBeheersTool.Helpers;
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
        public ActionResult Index(string naam = null, string voornaam = null)
        {
            var begeleiders = _begeleiderRepository.FindAll().WithFilter(naam, voornaam);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_begeleidersList", begeleiders);
            }
            return View(begeleiders);
        }

        [Authorize(Role.Admin)]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BegeleiderCreateVM model)
        {
            if (ModelState.IsValid)
            {
                var begeleider = Mapper.Map<Begeleider>(model);
                var result = _begeleiderRepository.Add(begeleider);
                if (result == true)
                {
                    TempData["message"] = string.Format(Resources.SuccesBegeleiderCreate, begeleider.HogentEmail);
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = string.Format(Resources.ErrorBegeleiderCreateHogentEmailBestaatAl, begeleider.HogentEmail);
                }
            }
            return View(model);
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
            model.ToonEdit = CurrentUser.IsAdmin() || begeleider.Equals(_userService.FindBegeleider());
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
            }
            else
            {
                begeleider = _userService.FindBegeleider();
            }
            if (begeleider == null)
            {
                return HttpNotFound();
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
            }
            else
            {
                begeleider = _userService.FindBegeleider();
            }
            if (begeleider == null)
            {
                return HttpNotFound();
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

        [Authorize(Role.Admin)]
        public ActionResult Delete(int id)
        {
            var begeleider = FindBegeleider(id);
            if (begeleider == null)
            {
                return HttpNotFound();
            }
            return View(begeleider);
        }

        [Authorize(Role.Admin)]
        [ActionName("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var begeleider = FindBegeleider(id);
            if (begeleider == null)
            {
                return HttpNotFound();
            }
            var result = _begeleiderRepository.Delete(begeleider);
            if (result == true)
            {
                TempData["message"] = "Begeleider " + begeleider.HogentEmail + " succesvol verwijderd.";
                return RedirectToAction("Index");
            }
            TempData["error"] = Resources.ErrorBegeleiderDeleteFailed;
            return View(begeleider);
        }

        [Authorize(Role.Admin)]
        public ActionResult BegeleiderJson(string hoGentEmail)
        {
            var begeleider = _begeleiderRepository.FindByEmail(hoGentEmail);
            var model = Mapper.Map<BegeleiderJsonVM>(begeleider);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #region helpers

        private Begeleider FindBegeleider(int id)
        {
            if (id == 0)
            {
                return _userService.FindBegeleider();
            }
            return _begeleiderRepository.FindById(id);
        }
        #endregion
    }
}