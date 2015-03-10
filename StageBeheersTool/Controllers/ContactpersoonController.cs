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

namespace StageBeheersTool.Controllers
{
    [Authorize(Roles = "bedrijf")]
    public class ContactpersoonController : Controller
    {
        private IUserService userService;
        private IContactpersoonRepository contactpersoonRepository;

        public ContactpersoonController(IUserService userService, IContactpersoonRepository contactpersoonRepository)
        {
            this.userService = userService;
            this.contactpersoonRepository = contactpersoonRepository;
        }

        [Authorize(Roles = "bedrijf")]
        public ActionResult Index(int page = 1)
        {
            var contactPersonen = userService.FindBedrijf().Contactpersonen;
            return View(contactPersonen.ToPagedList(page, 10));
        }

        [Authorize(Roles = "bedrijf")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "bedrijf")]
        public ActionResult Create(ContactpersoonCreateVM model)
        {
            if (ModelState.IsValid)
            {
                var contactpersoon = Mapper.Map<ContactpersoonCreateVM, Contactpersoon>(model);
                var bedrijf = userService.FindBedrijf();
                bedrijf.AddContactpersoon(contactpersoon);
                userService.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [Authorize(Roles = "bedrijf")]
        public ActionResult Details(int id)
        {
            var contactpersoon = userService.FindBedrijf().FindContactpersoonById(id);
            if (contactpersoon == null)
            {
                return HttpNotFound();
            }
            return View(contactpersoon);
        }

        [Authorize(Roles = "bedrijf")]
        public ActionResult Edit(int id)
        {
            var contactpersoon = userService.FindBedrijf().FindContactpersoonById(id);
            if (contactpersoon == null)
            {
                return HttpNotFound();
            }
            var model = Mapper.Map<Contactpersoon, ContactpersoonEditVM>(contactpersoon);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "bedrijf")]
        public ActionResult Edit(ContactpersoonEditVM model)
        {
            if (ModelState.IsValid)
            {
                var contactpersoon = Mapper.Map<ContactpersoonEditVM, Contactpersoon>(model);
                var bedrijf = userService.FindBedrijf();
                bedrijf.UpdateContactpersoon(contactpersoon);
                userService.SaveChanges();
                return RedirectToAction("Details", new { id = model.Id });
            }
            return View(model);
        }

        [Authorize(Roles = "bedrijf")]
        public ActionResult Delete(int id)
        {
            var bedrijf = userService.FindBedrijf();
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
        [Authorize(Roles = "bedrijf")]
        public ActionResult DeleteConfirmed(int id)
        {
            var bedrijf = userService.FindBedrijf();
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
            contactpersoonRepository.Delete(contactpersoon);
            userService.SaveChanges();
            TempData["message"] = "Contactpersoon " + contactpersoon.Naam + " verwijderd.";
            return RedirectToAction("Index");
        }

    }
}