using System;
using System.Collections.Generic;
using AutoMapper;
using StageBeheersTool.Models.DAL.Extensions;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.Models.Identity;
using StageBeheersTool.ViewModels;
using System.Web.Mvc;

namespace StageBeheersTool.Controllers
{
    public class BedrijfController : BaseController
    {
        private readonly IBedrijfRepository _bedrijfRepository;
        private readonly IUserService _userService;

        public BedrijfController(IBedrijfRepository bedrijfRepository,
            IUserService userService)
        {
            _bedrijfRepository = bedrijfRepository;
            _userService = userService;
        }

        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult List(string bedrijfsnaam = null)
        {
            var bedrijven = _bedrijfRepository.FindAll().WithFilter(bedrijfsnaam);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_BedrijfList", bedrijven);
            }
            return View(bedrijven);
        }

        [Authorize(Role.Admin)]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Role.Admin)]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(RegisterBedrijfViewModel model)
        {
            if (ModelState.IsValid)
            {
                var bedrijf = Mapper.Map<Bedrijf>(model);
                try
                {
                    _bedrijfRepository.Add(bedrijf);
                    SetViewMessage(string.Format(Resources.SuccesCreateBedrijf, bedrijf.Naam));
                    _userService.CreateLogin(bedrijf.Email, Role.Bedrijf);
                    return RedirectToAction("Details", new { id = bedrijf.Id });
                }
                catch (ApplicationException ex)
                {
                    SetViewError(ex.Message);
                }
            }
            return View(model);
        }

        [Authorize(Role.Bedrijf, Role.Admin, Role.Begeleider, Role.Student)]
        public ActionResult Details(int? id)
        {
            var bedrijf = FindBedrijf(id);
            if (bedrijf == null)
            {
                return HttpNotFound();
            }
            var isAdmin = CurrentUser.IsAdmin();
            var isBegeleider = CurrentUser.IsBegeleider();
            var isBedrijf = CurrentUser.IsBedrijf();
            var model = new BedrijfDetailsVM
            {
                Bedrijf = bedrijf,
                ToonEdit = isAdmin || isBedrijf,
                ToonChangePassword = isBedrijf,
                ToonExtra = isAdmin || isBegeleider,
                ToonTerug = isAdmin || isBegeleider,
                ToonDelete = isAdmin
            };
            return View(model);
        }

        [Authorize(Role.Bedrijf, Role.Admin)]
        public ActionResult Edit(int id)
        {
            var bedrijf = FindBedrijf(id);
            if (bedrijf == null)
            {
                return HttpNotFound();
            }
            var model = Mapper.Map<EditBedrijfVM>(bedrijf);
            model.ToonTerug = CurrentUser.IsAdmin();
            return View(model);
        }

        [Authorize(Role.Bedrijf, Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditBedrijfVM model)
        {
            var bedrijf = Mapper.Map<Bedrijf>(model);
            if (CurrentUser.IsBedrijf())
            {
                bedrijf.Id = _userService.GetBedrijf().Id;
            }
            _bedrijfRepository.Update(bedrijf);
            SetViewMessage(Resources.SuccesEditBedrijf);
            return RedirectToAction("Details", new { id = bedrijf.Id, Overzicht });
        }

        [Authorize(Role.Admin)]
        public ActionResult Delete(int id)
        {
            var bedrijf = _bedrijfRepository.FindById(id);
            if (bedrijf == null)
            {
                return HttpNotFound();
            }
            return View(bedrijf);
        }

        [Authorize(Role.Admin)]
        [ActionName("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string overzicht = "/Bedrijf/List")
        {
            var bedrijf = _bedrijfRepository.FindById(id);
            if (bedrijf == null)
            {
                return HttpNotFound();
            }
            try
            {
                _bedrijfRepository.Delete(bedrijf);
                SetViewMessage(string.Format(Resources.SuccesDeleteBedrijf, bedrijf.Naam));
            }
            catch (ApplicationException ex)
            {
                SetViewError(ex.Message);
                return View(bedrijf);
            }
            finally
            {
                _userService.DeleteLogin(bedrijf.Email);
            }
            return Redirect(overzicht);
        }

        [Authorize(Role.Admin)]
        public ActionResult BedrijfJson(int? id = null, string email = null)
        {
            Bedrijf bedrijf;
            if (id != null)
            {
                bedrijf = _bedrijfRepository.FindById((int)id);
            }
            else
            {
                bedrijf = _bedrijfRepository.FindByEmail(email);
            }
            if (bedrijf == null)
            {
                return HttpNotFound();
            }
            var model = Mapper.Map<BedrijfJsonVM>(bedrijf);
            model.Stagementors = Mapper.Map<IEnumerable<Contactpersoon>, IEnumerable<ContactpersoonJsonVM>>(bedrijf.FindAllStagementors());
            model.Contractondertekenaars =
                Mapper.Map<IEnumerable<Contactpersoon>, IEnumerable<ContactpersoonJsonVM>>(bedrijf.FindAllContractOndertekenaars());
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #region Helpers

        private Bedrijf FindBedrijf(int? id)
        {
            if (CurrentUser.IsBedrijf())
            {
                return _userService.GetBedrijf();
            }
            if (id == null)
            {
                return null;
            }
            var bedrijf = _bedrijfRepository.FindById((int)id);
            return bedrijf;
        }

        #endregion
    }
}