using AutoMapper;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;


namespace StageBeheersTool.Controllers
{
    [Authorize]
    public class StageopdrachtController : Controller
    {
        private IBedrijfRepository bedrijfRepository;
        private ISpecialisatieRepository specialisatieRepository;
        private IStageopdrachtRepository stageopdrachtRepository;

        public StageopdrachtController(IBedrijfRepository bedrijfRepository,
            ISpecialisatieRepository specialisatieRepository, IStageopdrachtRepository stageopdrachtRepository)
        {
            this.bedrijfRepository = bedrijfRepository;
            this.specialisatieRepository = specialisatieRepository;
            this.stageopdrachtRepository = stageopdrachtRepository;
        }

        public ActionResult Index(int page = 1)
        {
            return View(FindBedrijf().Stageopdrachten.ToPagedList(page, 10));
        }

        [Authorize(Roles = "bedrijf")]
        public ActionResult Create()
        {
            var bedrijf = FindBedrijf();
            var constractondertekenaars = bedrijf.FindAllContractOndertekenaars();
            var stagementors = bedrijf.FindAllStagementors();
            if (constractondertekenaars.Count() == 0 || stagementors.Count() == 0)
            {
                ModelState.AddModelError("", "Geen stagementor of constractondertekenaar gevonden. Klik <a href='/Contactpersoon/Create'>hier</a> om 1 aan te maken.");
            }

            return View(new StageopdrachtCreateVM(specialisatieRepository.FindAll(),
                bedrijf.FindAllContractOndertekenaars(),
                bedrijf.FindAllStagementors()));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "bedrijf")]
        public ActionResult Create(StageopdrachtCreateVM model)
        {
            var bedrijf = FindBedrijf();
            if (ModelState.IsValid)
            {
                var stageopdracht = Mapper.Map<StageopdrachtCreateVM, Stageopdracht>(model);
                stageopdracht.Specialisatie = specialisatieRepository.FindBy(model.SpecialisatieId);
                stageopdracht.Stagementor = bedrijf.FindContactpersoonById(model.StagementorId);
                stageopdracht.ContractOndertekenaar = bedrijf.FindContactpersoonById(model.ContractOndertekenaarId);
                bedrijf.AddStageopdracht(stageopdracht);
                bedrijfRepository.SaveChanges();
                TempData["message"] = "Stageopdracht succesvol aangemaakt.";
                return RedirectToAction("Index");
            }
            return View(new StageopdrachtCreateVM(specialisatieRepository.FindAll(),
                bedrijf.FindAllContractOndertekenaars(),
                bedrijf.FindAllStagementors()));
        }

        [Authorize(Roles = "bedrijf")]
        public ActionResult Details(int id)
        {
            var bedrijf = FindBedrijf();
            var stageopdracht = bedrijf.FindStageopdrachtById(id);
            if (stageopdracht != null)
            {
                return View(stageopdracht);
            }
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "bedrijf")]
        public ActionResult Edit(int id)
        {
            var bedrijf = FindBedrijf();
            var stageopdracht = bedrijf.FindStageopdrachtById(id);
            if (stageopdracht != null)
            {
                var model = Mapper.Map<Stageopdracht, StageopdrachtEditVM>(stageopdracht);
                model.SpecialisatieId = stageopdracht.Specialisatie.Id;
                model.ContractOndertekenaarId = stageopdracht.ContractOndertekenaar.Id;
                model.StagementorId = stageopdracht.Stagementor.Id;
                model.SetSelectLists(specialisatieRepository.FindAll(),
                    bedrijf.FindAllContractOndertekenaars(),
                    bedrijf.FindAllStagementors());
                return View(model);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "bedrijf")]
        public ActionResult Edit(StageopdrachtEditVM model)
        {
            var bedrijf = FindBedrijf();
            if (ModelState.IsValid)
            {
                var stageopdracht = Mapper.Map<StageopdrachtEditVM, Stageopdracht>(model);
                stageopdracht.Specialisatie = specialisatieRepository.FindBy(model.SpecialisatieId);
                stageopdracht.Stagementor = bedrijf.FindContactpersoonById(model.StagementorId);
                stageopdracht.ContractOndertekenaar = bedrijf.FindContactpersoonById(model.ContractOndertekenaarId);
                bedrijf.UpdateStageopdracht(stageopdracht);
                bedrijfRepository.SaveChanges();
                return RedirectToAction("Details", new { id = model.Id });
            }
            model.SetSelectLists(specialisatieRepository.FindAll(),
                bedrijf.FindAllContractOndertekenaars(),
                bedrijf.FindAllStagementors());
            return View(model);
        }

        [Authorize(Roles = "bedrijf")]
        public ActionResult Delete(int id)
        {
            var bedrijf = FindBedrijf();
            var stageopdracht = bedrijf.FindStageopdrachtById(id);
            if (stageopdracht != null)
            {
                return View(stageopdracht);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "bedrijf")]
        public ActionResult DeleteConfirmed(int id)
        {
            var bedrijf = FindBedrijf();
            var stageopdracht = bedrijf.FindStageopdrachtById(id);
            if (stageopdracht != null)
            {
                if (stageopdracht.IsGoedgekeurd())
                {
                    TempData["message"] = "Goegekeurde stages kunnen niet meer verwijderd worden.";
                    return RedirectToAction("Index");
                }
                stageopdrachtRepository.Delete(stageopdracht);
                bedrijfRepository.SaveChanges();
                TempData["message"] = "Stageopdracht '" + stageopdracht.Titel + "' succesvol verwijderd.";
                return RedirectToAction("Index");
            }
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