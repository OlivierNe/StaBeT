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
        private IStudentRepository studentRepository;

        public StageopdrachtController(IBedrijfRepository bedrijfRepository,
            ISpecialisatieRepository specialisatieRepository, IStageopdrachtRepository stageopdrachtRepository,
            IStudentRepository studentRepository)
        {
            this.bedrijfRepository = bedrijfRepository;
            this.specialisatieRepository = specialisatieRepository;
            this.stageopdrachtRepository = stageopdrachtRepository;
            this.studentRepository = studentRepository;
        }

        [Authorize(Roles = "bedrijf,student,begeleider,admin")]
        public ActionResult Index(int? semester, string soort, string bedrijf, string locatie)
        {
            IEnumerable<Stageopdracht> stageopdrachten = null;
            if (User.IsInRole("bedrijf"))
            {
                stageopdrachten = new List<Stageopdracht>(FindBedrijf().Stageopdrachten);
            }
            else if (User.IsInRole("student"))
            {
                stageopdrachten = stageopdrachtRepository.FindGeldigeStageopdrachten(semester, soort, bedrijf, locatie);
            }
            else
            {
                stageopdrachten = stageopdrachtRepository.FindByFilter(semester, soort, bedrijf, locatie);
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
                bedrijfRepository.SaveChanges();
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
            Stageopdracht stageopdracht = null;
            var model = new StageopdrachtDetailsVM();
            if (User.IsInRole("bedrijf"))
            {
                var bedrijf = FindBedrijf();
                model.Stageopdracht = bedrijf.FindStageopdrachtById(id);
            }
            else if (User.IsInRole("student"))
            {
                model.Stageopdracht = stageopdrachtRepository.FindGeldigeStageopdrachtById(id);
                var student = FindStudent();
                model.ToonToevoegen = !student.HeeftStageopdracht(id);
                model.ToonVerwijderenBtn = !model.ToonToevoegen;
            }
            else
            {
                stageopdracht = stageopdrachtRepository.FindById(id);
            }

            if (model.Stageopdracht == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }


        [Authorize(Roles = "bedrijf")]
        public ActionResult Edit(int id)
        {
            var bedrijf = FindBedrijf();
            var stageopdracht = bedrijf.FindStageopdrachtById(id);
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
            var bedrijf = FindBedrijf();
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
            bedrijfRepository.SaveChanges();
            TempData["message"] = "Stageopdracht '" + stageopdracht.Titel + "' succesvol verwijderd.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "student,begeleider")]
        public ActionResult ToevoegenAanVoorkeur(int id)
        {
            var stageopdracht = stageopdrachtRepository.FindGeldigeStageopdrachtById(id);
            if (stageopdrachtRepository == null)
            {
                return HttpNotFound();
            }
            FindStudent().AddStageopdracht(stageopdracht);
            studentRepository.SaveChanges();
            TempData["message"] = "Stageopdracht toegevoegd aan mijn voorkeurstages.";
            return RedirectToAction("Details", new { id = id });
        }

        [Authorize(Roles = "student,begeleider")]
        public ActionResult VerwijderenUitVoorkeur(int id)
        {
            var stageopdracht = stageopdrachtRepository.FindGeldigeStageopdrachtById(id);
            if (stageopdrachtRepository == null)
            {
                return HttpNotFound();
            }
            FindStudent().RemoveStageopdracht(stageopdracht);
            studentRepository.SaveChanges();
            TempData["message"] = "Stageopdracht verwijderd uit mijn voorkeurstages.";
            return RedirectToAction("Details", new { id = id });
        }

        [Authorize(Roles = "student, begeleider")]
        public ActionResult MijnStageopdrachten()
        {
            var student = FindStudent();
            if (student.MijnStageopdrachten.Count == 1)
            {
                return RedirectToAction("Details", new { id = student.MijnStageopdrachten.First().Id });
            }

            var model = new StageopdrachtIndexVM() { Stageopdrachten = student.MijnStageopdrachten, DisplaySearchForm = false };
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("Index", model);
        }

        #region Helpers
        private Bedrijf FindBedrijf()
        {
            return bedrijfRepository.FindByEmail(User.Identity.Name);
        }
        private Student FindStudent()
        {
            return studentRepository.FindByEmail(User.Identity.Name);
        }
        #endregion

    }
}