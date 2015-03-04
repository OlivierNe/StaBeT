﻿using AutoMapper;
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
        private ISpecialisatieRepository specialisatieRepository;
        private IStageopdrachtRepository stageopdrachtRepository;
        private IUserService userService;

        public StageopdrachtController(IStageopdrachtRepository stageopdrachtRepository,
            ISpecialisatieRepository specialisatieRepository, IUserService userService)
        {
            this.stageopdrachtRepository = stageopdrachtRepository;
            this.specialisatieRepository = specialisatieRepository;
            this.userService = userService;
        }

        [Authorize(Roles = "bedrijf,student,begeleider,admin")]
        public ActionResult Index(int? semester, int? aantalStudenten, string soort, string bedrijf, string locatie, string student = null)
        {
            IEnumerable<Stageopdracht> stageopdrachten = null;
            if (userService.IsBedrijf())
            {
                stageopdrachten = userService.FindBedrijf().Stageopdrachten;
            }
            else if (userService.IsStudent())
            {
                stageopdrachten = stageopdrachtRepository.FindGeldigeStageopdrachten(semester, aantalStudenten, soort, bedrijf, locatie);
            }
            else
            {
                stageopdrachten = stageopdrachtRepository.FindByFilter(semester, aantalStudenten, soort, bedrijf, locatie).ToList();
            }
            var model = new StageopdrachtIndexVM()
            {
                Stageopdrachten = stageopdrachten,
                Semester = semester,
                Soort = soort,
                Locatie = locatie,
                Bedrijf = bedrijf
            };
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View(model);
        }

        [Authorize(Roles = "bedrijf")]
        public ActionResult Create(bool isStagementor = false, bool isContractOndertekenaar = false)
        {
            if (Request.IsAjaxRequest())
            {
                if (isStagementor)
                {
                    return PartialView("_CreateStagementorForm");
                }
                else if (isContractOndertekenaar)
                {
                    return PartialView("_CreateContractOndertekenaarForm");
                }
            }
            var bedrijf = userService.FindBedrijf();
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
            var bedrijf = userService.FindBedrijf();
            if (ModelState.IsValid)
            {
                var stageopdracht = Mapper.Map<Stageopdracht>(model);
                stageopdracht.Specialisatie = model.SpecialisatieId != null ? specialisatieRepository.FindBy((int)model.SpecialisatieId) : null;

                if (stageopdracht.Stagementor == null)
                {
                    stageopdracht.Stagementor = model.StagementorId != null ? bedrijf.FindContactpersoonById((int)model.StagementorId) : null;
                }

                if (model.StagementorId == -1)
                {
                    stageopdracht.Stagementor = stageopdracht.ContractOndertekenaar;
                    bedrijf.AddContactpersoon(stageopdracht.Stagementor);
                }
                else if (model.StagementorId == null)
                {
                    bedrijf.AddContactpersoon(stageopdracht.Stagementor);
                }
                if (stageopdracht.ContractOndertekenaar == null)
                {
                    stageopdracht.ContractOndertekenaar = model.ContractOndertekenaarId != null ? bedrijf.FindContactpersoonById((int)model.ContractOndertekenaarId) : null;
                }
                if (model.ContractOndertekenaarId == -1)
                {
                    stageopdracht.ContractOndertekenaar = stageopdracht.Stagementor;
                    bedrijf.AddContactpersoon(stageopdracht.ContractOndertekenaar);
                }
                else if (model.StagementorId == null)
                {
                    bedrijf.AddContactpersoon(stageopdracht.ContractOndertekenaar);
                }

                bedrijf.AddStageopdracht(stageopdracht);
                userService.SaveChanges();
                TempData["message"] = "Stageopdracht succesvol aangemaakt.";
                return RedirectToAction("Details", new { id = stageopdracht.Id });
            }
            model.SetSelectLists(specialisatieRepository.FindAll(), bedrijf.FindAllContractOndertekenaars(),
                bedrijf.FindAllStagementors());
            return View(model);
        }

        [Authorize(Roles = "bedrijf,student,admin,begeleider")]
        public ActionResult Details(int id)
        {
            var model = new StageopdrachtDetailsVM();
            if (userService.IsBedrijf())
            {
                var bedrijf = userService.FindBedrijf();
                model.Stageopdracht = bedrijf.FindStageopdrachtById(id);
            }
            else if (userService.IsStudent())
            {
                model.Stageopdracht = stageopdrachtRepository.FindGeldigeStageopdrachtById(id);
                var student = userService.FindStudent();
                model.ToonToevoegen = !student.HeeftStageopdracht(id);
                model.ToonVerwijderenBtn = !model.ToonToevoegen;
            }
            else if (userService.IsBegeleider())
            {
                var begeleider = userService.FindBegeleider();
                model.Stageopdracht = stageopdrachtRepository.FindById(id);
                model.ToonToevoegen = !begeleider.HeeftStageopdracht(id);
                model.ToonVerwijderenBtn = !model.ToonToevoegen;
            }
            else
            {
                model.Stageopdracht = stageopdrachtRepository.FindById(id);
            }

            if (model.Stageopdracht == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }


        [Authorize(Roles = "bedrijf,begeleider")]
        public ActionResult Edit(int id)
        {
            Stageopdracht stageopdracht = null;
            Bedrijf bedrijf = null;
            if (userService.IsBegeleider())
            {
                var begeleider = userService.FindBegeleider();
                stageopdracht = begeleider.FindStageopdracht(id);
                bedrijf = stageopdracht.Bedrijf;
                if (stageopdracht == null)
                {
                    return HttpNotFound();
                }
            }
            else if (userService.IsBedrijf())
            {
                bedrijf = userService.FindBedrijf();
                stageopdracht = bedrijf.FindStageopdrachtById(id);
            }
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            var model = Mapper.Map<Stageopdracht, StageopdrachtEditVM>(stageopdracht);
            model.SpecialisatieId = stageopdracht.Specialisatie == null ? null : (int?)stageopdracht.Specialisatie.Id;
            model.ContractOndertekenaarId = stageopdracht.ContractOndertekenaar == null ? null : (int?)stageopdracht.ContractOndertekenaar.Id;
            model.StagementorId = stageopdracht.Stagementor == null ? null : (int?)stageopdracht.Stagementor.Id;
            model.SetSelectLists(specialisatieRepository.FindAll(),
                bedrijf.FindAllContractOndertekenaars(),
                bedrijf.FindAllStagementors());
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "bedrijf, begeleider")]
        public ActionResult Edit(StageopdrachtEditVM model)
        {
            Bedrijf bedrijf = null;
            Stageopdracht stageopdracht = null;

            if (ModelState.IsValid)
            {
                if (userService.IsBedrijf())
                {
                    stageopdracht = stageopdrachtRepository.FindById(model.Id);
                    bedrijf = userService.FindBedrijf();
                }
                else if (userService.IsBegeleider())
                {
                    stageopdracht = stageopdrachtRepository.FindById(model.Id);
                    bedrijf = stageopdracht.Bedrijf;
                }

                var newStageopdracht = Mapper.Map<StageopdrachtEditVM, Stageopdracht>(model);
                newStageopdracht.Specialisatie = model.SpecialisatieId != null ? specialisatieRepository.FindBy((int)model.SpecialisatieId) : null;
                newStageopdracht.Stagementor = model.StagementorId != null ? bedrijf.FindContactpersoonById((int)model.StagementorId) : null;
                newStageopdracht.ContractOndertekenaar = model.ContractOndertekenaarId
                    != null ? bedrijf.FindContactpersoonById((int)model.ContractOndertekenaarId) : null;
                stageopdrachtRepository.Update(newStageopdracht, stageopdracht);
                userService.SaveChanges();
                return RedirectToAction("Details", new { id = model.Id });
            }

            stageopdracht = Mapper.Map<StageopdrachtEditVM, Stageopdracht>(model);
            bedrijf = stageopdracht.Bedrijf;
            model.SetSelectLists(specialisatieRepository.FindAll(),
                bedrijf.FindAllContractOndertekenaars(),
                bedrijf.FindAllStagementors());
            return View(model);
        }

        [Authorize(Roles = "bedrijf")]
        public ActionResult Delete(int id)
        {
            var bedrijf = userService.FindBedrijf();
            var stageopdracht = bedrijf.FindStageopdrachtById(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            return View(stageopdracht);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "bedrijf")]
        public ActionResult DeleteConfirmed(int id)
        {
            var bedrijf = userService.FindBedrijf();
            var stageopdracht = bedrijf.FindStageopdrachtById(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            if (stageopdracht.IsGoedgekeurd())
            {
                TempData["message"] = "Goegekeurde stages kunnen niet meer verwijderd worden.";
                return RedirectToAction("Index");
            }
            stageopdrachtRepository.Delete(stageopdracht);
            userService.SaveChanges();
            TempData["message"] = "Stageopdracht '" + stageopdracht.Titel + "' succesvol verwijderd.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "student,begeleider")]
        public ActionResult ToevoegenAanVoorkeur(int id)
        {
            var stageopdracht = stageopdrachtRepository.FindGeldigeStageopdrachtById(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            if (userService.IsStudent())
            {
                userService.FindStudent().AddStageopdracht(stageopdracht);
                userService.SaveChanges();
            }
            else if (userService.IsBegeleider())
            {
                userService.FindBegeleider().AddStageopdracht(stageopdracht);
                userService.SaveChanges();
            }
            // TempData["message"] = "Stageopdracht toegevoegd aan mijn voorkeurstages.";
            return RedirectToAction("MijnVoorkeurStages");
        }


        [Authorize(Roles = "student,begeleider")]
        public ActionResult VerwijderenUitVoorkeur(int id)
        {
            var stageopdracht = stageopdrachtRepository.FindGeldigeStageopdrachtById(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            if (userService.IsStudent())
            {
                userService.FindStudent().RemoveStageopdracht(stageopdracht);
                userService.SaveChanges();
            }
            else if (userService.IsBegeleider())
            {
                userService.FindBegeleider().RemoveStageopdracht(stageopdracht);
                userService.SaveChanges();
            }
            // TempData["message"] = "Stageopdracht verwijderd uit mijn voorkeurstages.";
            return RedirectToAction("MijnVoorkeurStages");
        }

        [Authorize(Roles = "student, begeleider")]
        public ActionResult MijnVoorkeurStages()
        {
            IEnumerable<Stageopdracht> stageopdrachten = null;
            if (userService.IsStudent())
            {
                var student = userService.FindStudent();
                stageopdrachten = student.MijnStageopdrachten;
            }
            else if (userService.IsBegeleider())
            {
                var begeleider = userService.FindBegeleider();
                stageopdrachten = begeleider.MijnStageopdrachten;
            }
            /*if (student.MijnStageopdrachten.Count == 1)
            {
                return RedirectToAction("Details", new { id = student.MijnStageopdrachten.First().Id });
            }*/

            var model = new StageopdrachtIndexVM() { Stageopdrachten = stageopdrachten, DisplaySearchForm = false };
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("Index", model);
        }

    }

}