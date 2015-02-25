using AutoMapper;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Microsoft.AspNet.Identity;

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

        [Authorize(Roles = "bedrijf,student")]
        public ActionResult Index(int page = 1)
        {
            if (User.IsInRole("bedrijf"))
            {
                return View(FindBedrijf().Stageopdrachten.ToPagedList(page, 10));
            }
            else if (User.IsInRole("student"))
            {
                return View(stageopdrachtRepository.FindAll()
                    .Where(so => so.Status == StageopdrachtStatus.Goedgekeurd && so.AantalToegewezenStudenten < so.AantalStudenten)
                    .ToPagedList(page, 10));
            }

            return View(stageopdrachtRepository.FindAll().ToPagedList(page, 10));
        }

        [Authorize(Roles = "bedrijf")]
        public ActionResult Create()
        {
            var bedrijf = FindBedrijf();
            var constractondertekenaars = bedrijf.FindAllContractOndertekenaars();
            var stagementors = bedrijf.FindAllStagementors();
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
                stageopdracht.Specialisatie = model.SpecialisatieId != null ? specialisatieRepository.FindBy((int)model.SpecialisatieId) : null;
                stageopdracht.Stagementor = model.StagementorId != null ? bedrijf.FindContactpersoonById((int)model.StagementorId) : null;
                stageopdracht.ContractOndertekenaar = model.ContractOndertekenaarId != null ? bedrijf.FindContactpersoonById((int)model.ContractOndertekenaarId) : null;
                bedrijf.AddStageopdracht(stageopdracht);
                bedrijfRepository.SaveChanges();
                TempData["message"] = "Stageopdracht succesvol aangemaakt.";
                return RedirectToAction("Index");
            }
            return View(new StageopdrachtCreateVM(specialisatieRepository.FindAll(),
                bedrijf.FindAllContractOndertekenaars(),
                bedrijf.FindAllStagementors()));
        }

        [Authorize(Roles = "bedrijf,student,admin,begeleider")]
        public ActionResult Details(int id)
        {
            Stageopdracht stageopdracht = null;

            if (User.IsInRole("bedrijf"))
            {
                var bedrijf = FindBedrijf();
                stageopdracht = bedrijf.FindStageopdrachtById(id);
            }
            else
            {
                stageopdracht = stageopdrachtRepository.FindById(id);
            }

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
                model.SpecialisatieId = stageopdracht.Specialisatie == null ? null : (int?)stageopdracht.Specialisatie.Id;
                model.ContractOndertekenaarId = stageopdracht.ContractOndertekenaar == null ? null : (int?)stageopdracht.ContractOndertekenaar.Id;
                model.StagementorId = stageopdracht.Stagementor == null ? null : (int?)stageopdracht.Stagementor.Id;
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
                stageopdracht.Specialisatie = model.SpecialisatieId != null ? specialisatieRepository.FindBy((int)model.SpecialisatieId) : null;
                stageopdracht.Stagementor = model.StagementorId != null ? bedrijf.FindContactpersoonById((int)model.StagementorId) : null;
                stageopdracht.ContractOndertekenaar = model.ContractOndertekenaarId != null ? bedrijf.FindContactpersoonById((int)model.ContractOndertekenaarId) : null;
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