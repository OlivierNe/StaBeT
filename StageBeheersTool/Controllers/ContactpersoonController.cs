using AutoMapper;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StageBeheersTool.Controllers
{
    /*
     * Aanmaken, wijzigen en verwijderen van contactpersonen (mentor-contractondertekenaar). 
     * De informatie over een contactpersoon bevat minimaal: naam - voornaam - e-mail - gsm 
     * - functie binnen het bedrijf - aanspreektitel - functie stageopdracht (mentor-contractondertekenaar.
     * */
    [Authorize(Roles = "bedrijf")]
    public class ContactpersoonController : Controller
    {
        private IBedrijfRepository bedrijfRepository;

        public ContactpersoonController(IBedrijfRepository bedrijfRepository)
        {
            this.bedrijfRepository = bedrijfRepository;
        }

        public ActionResult Index()
        {
            var contactPersonen = FindBedrijf().Contactpersonen;
            return View(contactPersonen);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ContactpersoonCreateVM model)
        {
            if (ModelState.IsValid)
            {
                var contactpersoon = Mapper.Map<ContactpersoonCreateVM, Contactpersoon>(model);
                var bedrijf = FindBedrijf();
                bedrijf.AddContactpersoon(contactpersoon);
                bedrijfRepository.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var contactpersoon = FindBedrijf().FindContactpersoonById(id);
            if (contactpersoon != null)
            {
                return View(contactpersoon);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var contactpersoon = FindBedrijf().FindContactpersoonById(id);
            if (contactpersoon != null)
            {
                var vm = Mapper.Map<Contactpersoon, ContactpersoonEditVM>(contactpersoon);
                return View(vm);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ContactpersoonEditVM model)
        {
            if (ModelState.IsValid)
            {
                var contactpersoon = Mapper.Map<ContactpersoonEditVM, Contactpersoon>(model);
                var bedrijf = FindBedrijf();
                bedrijf.UpdateContactpersoon(contactpersoon);
                bedrijfRepository.SaveChanges();
                return RedirectToAction("Details", new { id = model.Id });
            }
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var contactpersoon = FindBedrijf().FindContactpersoonById(id);
            return View(contactpersoon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var bedrijf = FindBedrijf();
            var contactpersoon = bedrijf.FindContactpersoonById(id);
            bedrijf.DeleteContactpersoon(contactpersoon);
            bedrijfRepository.SaveChanges();
            return RedirectToAction("Index");
        }

        #region Helpers
        private Bedrijf FindBedrijf()
        {
            return bedrijfRepository.FindByEmail(User.Identity.Name);
        }
        #endregion

    }
}