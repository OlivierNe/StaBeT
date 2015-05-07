using System;
using AutoMapper;
using StageBeheersTool.Models.DAL.Extensions;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.Models.Identity;
using StageBeheersTool.ViewModels;
using System.Web.Mvc;

namespace StageBeheersTool.Controllers
{
    public class ContactpersoonController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IBedrijfRepository _bedrijfRepository;
        private readonly IContactpersoonRepository _contactpersoonRepository;

        public ContactpersoonController(IUserService userService,
            IContactpersoonRepository contactpersoonRepository, IBedrijfRepository bedrijfRepository)
        {
            _userService = userService;
            _contactpersoonRepository = contactpersoonRepository;
            _bedrijfRepository = bedrijfRepository;
        }

        [Authorize(Role.Bedrijf, Role.Admin)]
        public ActionResult List(ContactpersoonListVM model)
        {
            if (CurrentUser.IsBedrijf())
            {
                var bedrijf = _userService.GetBedrijf();
                model.Contactpersonen = bedrijf.Contactpersonen;
            }
            else //admin
            {
                model.ToonZoeken = true;
                model.Contactpersonen = _contactpersoonRepository.FindAll()
                    .WithFilter(model.Bedrijf, model.Naam);
            }
            model.Title = "Overzicht contactpersonen";
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ContactpersoonList", model);
            }
            return View(model);
        }

        [Authorize(Role.Admin)]
        public ActionResult VanBedrijf(int bedrijfId)
        {
            var bedrijf = _bedrijfRepository.FindById(bedrijfId);
            var model = new ContactpersoonListVM
            {
                Contactpersonen = bedrijf.Contactpersonen
            };
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ContactpersoonList", model);
            }
            model.Title = "Overzicht contactpersonen van " + bedrijf.Naam;
            return View("List", model);
        }

        [Authorize(Role.Bedrijf, Role.Admin)]
        public ActionResult Create()
        {
            var model = new ContactpersoonCreateVM();
            if (CurrentUser.IsAdmin())
            {
                model.SetBedrijven(_bedrijfRepository.FindAll());
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Role.Bedrijf, Role.Admin)]
        public ActionResult Create(ContactpersoonCreateVM model)
        {
            if (CurrentUser.IsAdmin() && model.BedrijfId == 0)
            {
                ModelState.AddModelError("", Resources.ErrorBedrijfVerplicht);
            }
            if (ModelState.IsValid)
            {
                var contactpersoon = Mapper.Map<ContactpersoonCreateVM, Contactpersoon>(model);
                var bedrijf = FindBedrijf(model.BedrijfId);
                if (bedrijf == null)
                {
                    SetViewError("Bedrijf niet gevonden.");
                    return View(model);
                }
                bedrijf.AddContactpersoon(contactpersoon);
                _bedrijfRepository.SaveChanges();
                SetViewMessage(String.Format(Resources.SuccesCreateContactpersoon, contactpersoon.Naam));
                if (CurrentUser.IsAdmin())
                {
                    return RedirectToAction("VanBedrijf", new { bedrijfId = bedrijf.Id });
                }
                return RedirectToAction("List");
            }
            return View(model);
        }

        [Authorize(Role.Bedrijf, Role.Admin)]
        public ActionResult Details(int id)
        {
            var contactpersoon = FindContactpersoon(id);
            if (contactpersoon == null)
            {
                return HttpNotFound();
            }
            return View(contactpersoon);
        }

        [Authorize(Role.Bedrijf, Role.Admin)]
        public ActionResult Edit(int id)
        {
            var contactpersoon = FindContactpersoon(id);
            if (contactpersoon == null)
            {
                return HttpNotFound();
            }
            var model = Mapper.Map<Contactpersoon, ContactpersoonEditVM>(contactpersoon);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Role.Bedrijf, Role.Admin)]
        public ActionResult Edit(ContactpersoonEditVM model)
        {
            if (ModelState.IsValid)
            {
                var contactpersoon = Mapper.Map<ContactpersoonEditVM, Contactpersoon>(model);
                if (CurrentUser.IsBedrijf())
                {
                    var bedrijf = _userService.GetBedrijf();
                    if (bedrijf.FindContactpersoonById(contactpersoon.Id) != null)
                    {
                        _contactpersoonRepository.Update(contactpersoon);
                    }
                }
                else //admin
                {
                    _contactpersoonRepository.Update(contactpersoon);
                }
                return RedirectToAction("Details", new { id = model.Id });
            }
            return View(model);
        }

        [Authorize(Role.Bedrijf, Role.Admin)]
        public ActionResult Delete(int id)
        {
            var contactpersoon = FindContactpersoon(id);
            if (contactpersoon == null)
            {
                return HttpNotFound();
            }
            return View(contactpersoon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        [Authorize(Role.Bedrijf, Role.Admin)]
        public ActionResult DeleteConfirmed(int id)
        {
            var contactpersoon = FindContactpersoon(id);
            if (contactpersoon == null)
            {
                return HttpNotFound();
            }
            var bedrijf = contactpersoon.Bedrijf;
            bedrijf.KoppelContactpersoonLosVanOpdrachten(contactpersoon);
            _contactpersoonRepository.Delete(contactpersoon);
            SetViewMessage(String.Format(Resources.SuccesDeleteContactpersoon, contactpersoon.Naam));
            return RedirectToAction("List");
        }

        #region Helpers

        private Contactpersoon FindContactpersoon(int id)
        {
            if (CurrentUser.IsBedrijf())
            {
                var bedrijf = _userService.GetBedrijf();
                return bedrijf.FindContactpersoonById(id); //bedrijf mag enkel zijn eigen contactpersonen beheren
            }
            return _contactpersoonRepository.FindById(id);//ingelogd als admin
        }

        private Bedrijf FindBedrijf(int id)
        {
            if (CurrentUser.IsBedrijf())
            {
                return _userService.GetBedrijf();
            }
            return _bedrijfRepository.FindById(id);
        }

        #endregion

    }
}