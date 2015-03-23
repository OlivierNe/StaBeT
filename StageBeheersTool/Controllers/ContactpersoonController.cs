using AutoMapper;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity.Infrastructure;
using StageBeheersTool.Models.Authentication;

namespace StageBeheersTool.Controllers
{
    [Authorize(Role.Bedrijf)]
    public class ContactpersoonController : Controller
    {
        private readonly IUserService _userService;
        private readonly IContactpersoonRepository _contactpersoonRepository;

        public ContactpersoonController(IUserService userService, IContactpersoonRepository contactpersoonRepository)
        {
            _userService = userService;
            _contactpersoonRepository = contactpersoonRepository;
        }

        [Authorize(Role.Bedrijf)]
        public ActionResult Index(int page = 1)
        {
            var contactPersonen = _userService.FindBedrijf().Contactpersonen;
            return View(contactPersonen.ToPagedList(page, 10));
        }

        [Authorize(Role.Bedrijf)]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Role.Bedrijf)]
        public ActionResult Create(ContactpersoonCreateVM model)
        {
            if (ModelState.IsValid)
            {
                var contactpersoon = Mapper.Map<ContactpersoonCreateVM, Contactpersoon>(model);
                var bedrijf = _userService.FindBedrijf();
                bedrijf.AddContactpersoon(contactpersoon);
                _userService.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [Authorize(Role.Bedrijf)]
        public ActionResult Details(int id)
        {
            var contactpersoon = _userService.FindBedrijf().FindContactpersoonById(id);
            if (contactpersoon == null)
            {
                return HttpNotFound();
            }
            return View(contactpersoon);
        }

        [Authorize(Role.Bedrijf)]
        public ActionResult Edit(int id)
        {
            var contactpersoon = _userService.FindBedrijf().FindContactpersoonById(id);
            if (contactpersoon == null)
            {
                return HttpNotFound();
            }
            var model = Mapper.Map<Contactpersoon, ContactpersoonEditVM>(contactpersoon);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Role.Bedrijf)]
        public ActionResult Edit(ContactpersoonEditVM model)
        {
            if (ModelState.IsValid)
            {
                var contactpersoon = Mapper.Map<ContactpersoonEditVM, Contactpersoon>(model);
                var bedrijf = _userService.FindBedrijf();
                bedrijf.UpdateContactpersoon(contactpersoon);
                _userService.SaveChanges();
                return RedirectToAction("Details", new { id = model.Id });
            }
            return View(model);
        }

        [Authorize(Role.Bedrijf)]
        public ActionResult Delete(int id)
        {
            var bedrijf = _userService.FindBedrijf();
            var contactpersoon = bedrijf.FindContactpersoonById(id);
            if (contactpersoon == null)
            {
                return HttpNotFound();
            }
            return View(contactpersoon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        [Authorize(Role.Bedrijf)]
        public ActionResult DeleteConfirmed(int id)
        {
            var bedrijf = _userService.FindBedrijf();
            var contactpersoon = bedrijf.FindContactpersoonById(id);
            if (contactpersoon == null)
            {
                return HttpNotFound();
            }
            if (bedrijf.ContactpersoonHeeftStageopdrachten(contactpersoon))
            {
                TempData["message"] = "Verwijderen mislukt: Contactpersoon is aan 1 of meerdere stageopdrachten gekoppeld.";
                return View(contactpersoon);
            }
            _contactpersoonRepository.Delete(contactpersoon);
            _userService.SaveChanges();
            TempData["message"] = "Contactpersoon " + contactpersoon.Naam + " verwijderd.";
            return RedirectToAction("Index");
        }

    }
}