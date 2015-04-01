using AutoMapper;
using StageBeheersTool.Helpers;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.ViewModels;
using System.Web.Mvc;
using StageBeheersTool.Models.Authentication;

namespace StageBeheersTool.Controllers
{
    public class ContactpersoonController : Controller
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
        public ActionResult Index(ContactpersoonIndexVM model)
        {
            if (CurrentUser.IsBedrijf())
            {
                var bedrijf = _userService.FindBedrijf();
                model.Contactpersonen = bedrijf.Contactpersonen;
            }
            else //admin
            {
                model.ToonBedrijf = true;
                model.Contactpersonen = _contactpersoonRepository.FindAll()
                    .WithFilter(model.Bedrijf, model.Naam);
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ContactpersoonList", model);
            }
            return View(model);
        }

        [Authorize(Role.Admin)]
        public ActionResult VanBedrijf(int bedrijfId)
        {
            var model = new ContactpersoonIndexVM
            {
                Contactpersonen = _contactpersoonRepository.FindAllVanBedrijf(bedrijfId)
            };
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ContactpersoonList");
            }
            return View("Index", model);
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
                ModelState.AddModelError("", "Het veld bedrijf is verplicht.");
            }
            if (ModelState.IsValid)
            {
                var contactpersoon = Mapper.Map<ContactpersoonCreateVM, Contactpersoon>(model);
                var bedrijf = _bedrijfRepository.FindById(model.BedrijfId);
                bedrijf.AddContactpersoon(contactpersoon);
                _bedrijfRepository.SaveChanges();
                return RedirectToAction("Index");
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
                    var bedrijf = _userService.FindBedrijf();
                    bedrijf.UpdateContactpersoon(contactpersoon);
                    _bedrijfRepository.SaveChanges();
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
            //TODO:wat moet er gebeuren als een contactpersoon verwijderd wordt?
            /*if (bedrijf.ContactpersoonHeeftStageopdrachten(contactpersoon))
            {
                //TODO: contactpersoon loskoppelen mag maar er moet kunnen gezien worden wie aan welke stageopdracht gekoppeld was(voor archief)
                TempData["message"] = "Verwijderen mislukt: Contactpersoon is aan 1 of meerdere stageopdrachten gekoppeld.";
                return View(contactpersoon);
            }*/
            _contactpersoonRepository.Delete(contactpersoon);
            _contactpersoonRepository.SaveChanges();
            TempData["message"] = "Contactpersoon " + contactpersoon.Naam + " verwijderd.";
            return RedirectToAction("Index");
        }

        #region private helpers

        private Contactpersoon FindContactpersoon(int id)
        {
            if (CurrentUser.IsBedrijf())
            {
                var bedrijf = _userService.FindBedrijf();
                return bedrijf.FindContactpersoonById(id); //bedrijf mag enkel zijn eigen contactpersonen beheren
            }
            return _contactpersoonRepository.FindById(id);//ingelogd als admin
        }
        #endregion

    }
}