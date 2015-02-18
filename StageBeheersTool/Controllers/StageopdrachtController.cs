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
    [Authorize]
    public class StageopdrachtController : Controller
    {
        private IBedrijfRepository bedrijfRepository;
        private ISpecialisatieRepository specialisatieRepository;

        public StageopdrachtController(IBedrijfRepository bedrijfRepository, ISpecialisatieRepository specialisatieRepository)
        {
            this.bedrijfRepository = bedrijfRepository;
            this.specialisatieRepository = specialisatieRepository;
        }

        public ActionResult Index()
        {
            return View(bedrijfRepository.FindByEmail(User.Identity.Name).Stageopdrachten);
        }

        [Authorize(Roles = "bedrijf")]
        public ActionResult Create()
        {
            return View(new StageopdrachtCreateVM(specialisatieRepository.FindAll()));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "bedrijf")]
        public ActionResult Create(StageopdrachtCreateVM model)
        {
            if (ModelState.IsValid)
            {
                var stageopdracht = Mapper.Map<StageopdrachtCreateVM, Stageopdracht>(model);
                stageopdracht.Specialisatie = specialisatieRepository.FindBy(model.SpecialisatieId);
                bedrijfRepository.FindByEmail(User.Identity.Name).AddStageopdracht(stageopdracht);
                bedrijfRepository.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(new StageopdrachtCreateVM(specialisatieRepository.FindAll()));
        }

        [Authorize(Roles = "bedrijf")]
        public ActionResult Details(int id)
        {
            var bedrijf = bedrijfRepository.FindByEmail(User.Identity.Name);
            Stageopdracht stageopdracht = bedrijf.FindStageopdrachtById(id);
            return View(stageopdracht);
        }



        [Authorize(Roles = "bedrijf")]
        public ActionResult Edit(int id)
        {
            var stageopdracht = bedrijfRepository.FindByEmail(User.Identity.Name).FindStageopdrachtById(id);
            var vm = Mapper.Map<Stageopdracht, StageopdrachtEditVM>(stageopdracht);
            vm.SpecialisatieId = stageopdracht.Specialisatie.Id;
            vm.SetSelectList(specialisatieRepository.FindAll());
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "bedrijf")]
        [ActionName("Edit")]
        public ActionResult EditConfirmed(StageopdrachtEditVM model)
        {
            if (ModelState.IsValid)
            {
                var stageopdracht = Mapper.Map<StageopdrachtEditVM, Stageopdracht>(model);
                stageopdracht.Specialisatie = specialisatieRepository.FindBy(model.SpecialisatieId);
                var bedrijf = bedrijfRepository.FindByEmail(User.Identity.Name);
                bedrijf.UpdateStageopdracht(stageopdracht);
                bedrijfRepository.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(new StageopdrachtEditVM(specialisatieRepository.FindAll()));
        }

        [Authorize(Roles = "bedrijf")]
        public ActionResult Delete(int id)
        {
            var bedrijf = bedrijfRepository.FindByEmail(User.Identity.Name);
            Stageopdracht stageopdracht = bedrijf.FindStageopdrachtById(id);
            return View(stageopdracht);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "bedrijf")]
        public ActionResult DeleteConfirmed(int id)
        {
            var bedrijf = bedrijfRepository.FindByEmail(User.Identity.Name);
            bedrijf.DeleteStageopdracht(id);
            bedrijfRepository.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}