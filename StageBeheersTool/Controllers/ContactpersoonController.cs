using AutoMapper;
using StageBeheersTool.Helpers;
using StageBeheersTool.Models.DAL.Extensions;
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
                ModelState.AddModelError("", Resources.ErrorBedrijfVerplicht);
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