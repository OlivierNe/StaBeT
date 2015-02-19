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

        public StageopdrachtController(IBedrijfRepository bedrijfRepository,
            ISpecialisatieRepository specialisatieRepository)
        {
            this.bedrijfRepository = bedrijfRepository;
            this.specialisatieRepository = specialisatieRepository;
        }

        public ActionResult Index(string message, int page = 1)
        {
            ViewBag.Message = message;
            return View(FindBedrijf().Stageopdrachten.ToPagedList(page, 10));
        }

        [Authorize(Roles = "bedrijf")]
        public ActionResult Create()
        {
            var bedrijf = FindBedrijf();
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
                return RedirectToAction("Index", new { message = "Stageopdracht succesvol aangemaakt." });
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
            return View(stageopdracht);
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
                bedrijf.UpdateStageopdracht(stageopdracht);
                bedrijfRepository.SaveChanges();
                return RedirectToAction("Index");
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
            return View(stageopdracht);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "bedrijf")]
        public ActionResult DeleteConfirmed(int id)
        {
            var bedrijf = FindBedrijf();
            var stageopdracht = bedrijf.FindStageopdrachtById(id);
            if (stageopdracht.IsGoedgekeurd)
            {
                return RedirectToAction("Index", new { message = "Goegekeurde stages kunnen niet verwijderd worden." });
            }
            bedrijf.DeleteStageopdracht(id);
            bedrijfRepository.SaveChanges();
            return RedirectToAction("Index", new { message = "Stageopdracht '" + stageopdracht.Titel + "' succesvol verwijderd." });
        }

        #region Helpers
        private Bedrijf FindBedrijf()
        {
            return bedrijfRepository.FindByEmail(User.Identity.Name);
        }
        #endregion

    }
}