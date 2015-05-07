using System;
using System.IO;
using AutoMapper;
using StageBeheersTool.Models.DAL.Extensions;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.Models.Identity;
using StageBeheersTool.ViewModels;
using System.Web;
using System.Web.Mvc;

namespace StageBeheersTool.Controllers
{
    public class BegeleiderController : BaseController
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
        public ActionResult List(BegeleiderListVM model)
        {
            var begeleiders = _begeleiderRepository.FindAll().WithFilter(model.Naam, model.Voornaam);
            model.Begeleiders = begeleiders;
            model.ToonActies = CurrentUser.IsAdmin();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_begeleidersList", model);
            }
            return View(model);
        }

        [Authorize(Role.Admin)]
        public ActionResult Create()
        {
            var model = new BegeleiderCreateVM { LoginAccountAanmaken = true };
            return View(model);
        }

        [Authorize(Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BegeleiderCreateVM model)
        {
            if (ModelState.IsValid)
            {
                var begeleider = Mapper.Map<Begeleider>(model);
                try
                {
                    _begeleiderRepository.Add(begeleider);
                    SetViewMessage(string.Format(Resources.SuccesBegeleiderCreate, begeleider.Naam));
                    if (model.LoginAccountAanmaken)
                    {
                        _userService.CreateLogin(begeleider.HogentEmail, Role.Begeleider);
                    }
                    return RedirectToAction("Details", new { begeleider.Id, Overzicht });
                }
                catch (ApplicationException ex)
                {
                    SetViewError(ex.Message);
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
            model.ToonEdit = CurrentUser.IsAdmin() || begeleider.Equals(_userService.GetBegeleider());
            model.ToonTerugNaarLijst = id != 0;
            model.ToonVerwijderen = CurrentUser.IsAdmin();
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
                begeleider = _userService.GetBegeleider();
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
        public ActionResult Edit(BegeleiderEditVM model)
        {
            Begeleider begeleider;
            if (CurrentUser.IsAdmin())
            {
                begeleider = FindBegeleider(model.Id);
            }
            else
            {
                begeleider = _userService.GetBegeleider();
            }
            if (begeleider == null)
            {
                return HttpNotFound();
            }
            var begeleiderModel = Mapper.Map<Begeleider>(model);
            try
            {
                var foto = _imageService.GetFoto(model.FotoFile, begeleider.Naam);
                begeleiderModel.Foto = foto;
            }
            catch (ApplicationException ex)
            {
                SetViewError(ex.Message);
            }
            _begeleiderRepository.Update(begeleiderModel);
            SetViewMessage("Wijzigingen opgeslagen.");
            return RedirectToAction("Details", new { model.Id, Overzicht });
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
            try
            {
                _begeleiderRepository.Delete(begeleider);
                SetViewMessage(string.Format(Resources.SuccesDeleteBegeleider, begeleider.Naam));
            }
            catch (ApplicationException ex)
            {
                SetViewError(ex.Message);
                return View(begeleider);
            }
            finally
            {
                _userService.DeleteLogin(begeleider.HogentEmail);
            }
            return Redirect(Overzicht);
        }

        [Authorize(Role.Admin)]
        public ActionResult BegeleiderJson(string email)
        {
            var begeleider = _begeleiderRepository.FindByEmail(email);
            if (begeleider == null)
            {
                return HttpNotFound();
            }
            var model = Mapper.Map<BegeleiderJsonVM>(begeleider);
            model.Email = begeleider.HogentEmail;
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #region Helpers

        private Begeleider FindBegeleider(int id)
        {
            if (id == 0)
            {
                return _userService.GetBegeleider();
            }
            return _begeleiderRepository.FindById(id);
        }

        #endregion
    }
}