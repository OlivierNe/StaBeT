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
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using StageBeheersTool.Models.Authentication;
using System.Threading.Tasks;


namespace StageBeheersTool.Controllers
{
    [Authorize]
    public class StageopdrachtController : Controller
    {
        private readonly ISpecialisatieRepository _specialisatieRepository;
        private readonly IStageopdrachtRepository _stageopdrachtRepository;
        private readonly IAcademiejaarRepository _academiejaarRepository;
        private readonly IUserService _userService;

        public StageopdrachtController(IStageopdrachtRepository stageopdrachtRepository,
            ISpecialisatieRepository specialisatieRepository, IUserService userService,
            IAcademiejaarRepository academiejaarRepository)
        {
            _stageopdrachtRepository = stageopdrachtRepository;
            _specialisatieRepository = specialisatieRepository;
            _academiejaarRepository = academiejaarRepository;
            _userService = userService;
        }

        #region lijst stageopdrachten

        [Authorize(Role.student, Role.Bedrijf, Role.Begeleider, Role.Admin)]
        public ActionResult Index(StageopdrachtIndexVM model)
        {
            var stageopdrachten = _stageopdrachtRepository.FindAllByFilter(model.Semester, model.AantalStudenten,
                model.SpecialisatieId, model.Bedrijf, model.Locatie, model.Student).ToList();
            model.setItems(stageopdrachten, _specialisatieRepository.FindAll());
            model.ToonSearchForm = true;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View(model);
        }

        [Authorize(Role.Begeleider)]
        public ActionResult GoedgekeurdeStages(StageopdrachtIndexVM model)
        {
            var stageopdrachten = _stageopdrachtRepository.FindGoedgekeurdeStageopdrachtenByFilter(model.Semester, model.AantalStudenten,
                model.SpecialisatieId, model.Bedrijf, model.Locatie, model.Student).ToList();
            model.setItems(stageopdrachten, _specialisatieRepository.FindAll());
            model.ToonSearchForm = true;
            model.ToonZoekenOpStudent = true;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("Index", model);
        }

        [Authorize(Role.Admin)]
        [ActionName("Voorstellen")]
        public ActionResult VoorstellenVanBedrijven(StageopdrachtIndexVM model)
        {
            var stageopdrachten = _stageopdrachtRepository.FindStageopdrachtVoorstellen().ToList();
            model.ToonOordelen = true;
            model.Stageopdrachten = stageopdrachten;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("Index", model);
        }

        #endregion

        #region create
        [Authorize(Role.Bedrijf)]
        public ActionResult Create(bool isStagementor = false, bool isContractOndertekenaar = false)
        {
            if (Request.IsAjaxRequest())
            {
                if (isStagementor)
                {
                    return PartialView("_CreateStagementorForm");
                }
                if (isContractOndertekenaar)
                {
                    return PartialView("_CreateContractOndertekenaarForm");
                }
            }
            var bedrijf = _userService.FindBedrijf();
            var model = new StageopdrachtCreateVM(_specialisatieRepository.FindAll(),
                bedrijf.FindAllContractOndertekenaars(), bedrijf.FindAllStagementors());
            model.SetAdres(bedrijf.Gemeente, bedrijf.Postcode, bedrijf.Straat, bedrijf.Straatnummer);
            var academiejaarInstellingen = _academiejaarRepository.FindByHuidigAcademiejaar();
            model.StageperiodeSem1 = academiejaarInstellingen.StageperiodeSemester1();
            model.StageperiodeSem2 = academiejaarInstellingen.StageperiodeSemester2();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Role.Bedrijf)]
        public ActionResult Create(StageopdrachtCreateVM model)
        {
            var bedrijf = _userService.FindBedrijf();
            if (ModelState.IsValid)
            {
                var stageopdracht = Mapper.Map<Stageopdracht>(model);
                stageopdracht.Specialisatie = model.SpecialisatieId != null ? _specialisatieRepository.FindBy((int)model.SpecialisatieId) : null;

                if (stageopdracht.Stagementor == null)
                {
                    stageopdracht.Stagementor = model.StagementorId != null ? bedrijf.FindContactpersoonById((int)model.StagementorId) : null;
                }
                if (model.StagementorId == -1)
                {
                    stageopdracht.Stagementor = stageopdracht.Contractondertekenaar;
                    bedrijf.AddContactpersoon(stageopdracht.Stagementor);
                }
                else if (model.StagementorId == null)
                {
                    bedrijf.AddContactpersoon(stageopdracht.Stagementor);
                }
                if (stageopdracht.Contractondertekenaar == null)
                {
                    stageopdracht.Contractondertekenaar = model.ContractondertekenaarId != null ? bedrijf.FindContactpersoonById((int)model.ContractondertekenaarId) : null;
                }
                if (model.ContractondertekenaarId == -1)
                {
                    stageopdracht.Contractondertekenaar = stageopdracht.Stagementor;
                    bedrijf.AddContactpersoon(stageopdracht.Contractondertekenaar);
                }
                else if (model.StagementorId == null)
                {
                    bedrijf.AddContactpersoon(stageopdracht.Contractondertekenaar);
                }
                bedrijf.AddStageopdracht(stageopdracht);
                _userService.SaveChanges();
                TempData["message"] = "Stageopdracht succesvol aangemaakt.";
                return RedirectToAction("Details", new { id = stageopdracht.Id });
            }
            model.SetSelectLists(_specialisatieRepository.FindAll(), bedrijf.FindAllContractOndertekenaars(),
                bedrijf.FindAllStagementors());
            var academiejaarInstellingen = _academiejaarRepository.FindByHuidigAcademiejaar();
            model.StageperiodeSem1 = academiejaarInstellingen.StageperiodeSemester1();
            model.StageperiodeSem2 = academiejaarInstellingen.StageperiodeSemester2();
            return View(model);
        }
        #endregion

        #region details
        [Authorize(Role.student, Role.Admin, Role.Begeleider, Role.Bedrijf)]
        public ActionResult Details(int id)
        {
            var model = new StageopdrachtDetailsVM();
            AcademiejaarInstellingen academiejaarInstellingen;
            if (_userService.IsBedrijf())
            {
                var bedrijf = _userService.FindBedrijf();
                var stageopdracht = bedrijf.FindStageopdrachtById(id);
                if (stageopdracht == null)
                {
                    return HttpNotFound();
                }
                academiejaarInstellingen = _academiejaarRepository.FindByAcademiejaar(stageopdracht.Academiejaar);
                model.ToonEdit = bedrijf.MagWijzigen(model.Stageopdracht, academiejaarInstellingen.DeadlineBedrijfStageEdit);
                model.EditDeadline = academiejaarInstellingen.DeadlineBedrijfStageEditToString();
                model.Stageopdracht = stageopdracht;
            }
            else if (_userService.IsStudent())
            {
                model.Stageopdracht = _stageopdrachtRepository.FindGeldigeStageopdrachtById(id);
                var student = _userService.FindStudent();
                model.ToonToevoegen = !student.HeeftStageopdrachtAlsVoorkeur(id);
                model.ToonVerwijderenBtn = !model.ToonToevoegen;
            }
            else if (_userService.IsBegeleider())
            {
                var begeleider = _userService.FindBegeleider();
                var stageopdracht = _stageopdrachtRepository.FindById(id);
                if (stageopdracht == null)
                {
                    return HttpNotFound();
                }
                model.ToonAanvraagIndienen = stageopdracht.IsGoedgekeurd()
                    && !model.Stageopdracht.HeeftStageBegeleider() && !begeleider.HeeftStageBegeleidingAangevraagd(id);
                model.ToonAanvraagAnnuleren = stageopdracht.IsGoedgekeurd() && !stageopdracht.HeeftStageBegeleider() &&
                    begeleider.HeeftStageBegeleidingAangevraagd(stageopdracht.Id);
                if (stageopdracht.Stagebegeleider != null && begeleider.Id == stageopdracht.Stagebegeleider.Id)
                {
                    model.ToonEdit = true;
                }
                model.Stageopdracht = stageopdracht;
            }
            else
            {
                model.Stageopdracht = _stageopdrachtRepository.FindById(id);
            }
            if (model.Stageopdracht == null)
            {
                return HttpNotFound();
            }
            academiejaarInstellingen = _academiejaarRepository.FindByAcademiejaar(model.Stageopdracht.Academiejaar);
            model.StageperiodeSem1 = academiejaarInstellingen.StageperiodeSemester1();
            model.StageperiodeSem2 = academiejaarInstellingen.StageperiodeSemester2();
            return View(model);
        }
        #endregion

        #region edit
        [Authorize(Role.Bedrijf, Role.Begeleider)]
        public ActionResult Edit(int id)
        {
            Stageopdracht stageopdracht = null;
            Bedrijf bedrijf = null;
            if (_userService.IsBegeleider())
            {
                var begeleider = _userService.FindBegeleider();
                stageopdracht = begeleider.FindStage(id);
                if (stageopdracht == null)
                {
                    return HttpNotFound();
                }
                bedrijf = stageopdracht.Bedrijf;
            }
            else if (_userService.IsBedrijf())
            {
                bedrijf = _userService.FindBedrijf();
                stageopdracht = bedrijf.FindStageopdrachtById(id);
                if (stageopdracht == null)
                {
                    return HttpNotFound();
                }
                var deadline = _academiejaarRepository.FindByAcademiejaar(stageopdracht.Academiejaar).DeadlineBedrijfStageEdit;
                if (bedrijf.MagWijzigen(stageopdracht, deadline) == false)
                {
                    return new HttpStatusCodeResult(403);
                }
            }
            var model = Mapper.Map<Stageopdracht, StageopdrachtEditVM>(stageopdracht);
            model.SpecialisatieId = stageopdracht.Specialisatie == null ? null : (int?)stageopdracht.Specialisatie.Id;
            model.ContractondertekenaarId = stageopdracht.Contractondertekenaar == null ? null : (int?)stageopdracht.Contractondertekenaar.Id;
            model.StagementorId = stageopdracht.Stagementor == null ? null : (int?)stageopdracht.Stagementor.Id;
            model.SetSelectLists(_specialisatieRepository.FindAll(),
                bedrijf.FindAllContractOndertekenaars(),
                bedrijf.FindAllStagementors());
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Role.Bedrijf, Role.Begeleider)]
        public ActionResult Edit(StageopdrachtEditVM model)
        {
            Bedrijf bedrijf = null;
            Stageopdracht stageopdracht = null;
            if (_userService.IsBedrijf())
            {
                stageopdracht = _stageopdrachtRepository.FindById(model.Id);
                bedrijf = _userService.FindBedrijf();
            }
            else if (_userService.IsBegeleider())
            {
                stageopdracht = _stageopdrachtRepository.FindById(model.Id);
                bedrijf = stageopdracht.Bedrijf;
            }
            if (ModelState.IsValid)
            {
                var stageopdrachtModel = Mapper.Map<StageopdrachtEditVM, Stageopdracht>(model);
                stageopdrachtModel.Specialisatie = model.SpecialisatieId != null ? _specialisatieRepository.FindBy((int)model.SpecialisatieId) : null;
                stageopdrachtModel.Stagementor = model.StagementorId != null ? bedrijf.FindContactpersoonById((int)model.StagementorId) : null;
                stageopdrachtModel.Contractondertekenaar = model.ContractondertekenaarId
                    != null ? bedrijf.FindContactpersoonById((int)model.ContractondertekenaarId) : null;
                _stageopdrachtRepository.Update(stageopdrachtModel);
                _userService.SaveChanges();
                return RedirectToAction("Details", new { id = model.Id });
            }
            stageopdracht = Mapper.Map<StageopdrachtEditVM, Stageopdracht>(model);
            model.SetSelectLists(_specialisatieRepository.FindAll(),
                bedrijf.FindAllContractOndertekenaars(),
                bedrijf.FindAllStagementors());
            return View(model);
        }
        #endregion

        #region stageopdracht verwijderen
        [Authorize(Role.Bedrijf)]
        public ActionResult Delete(int id)
        {
            var bedrijf = _userService.FindBedrijf();
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
        [Authorize(Role.Bedrijf)]
        public ActionResult DeleteConfirmed(int id)
        {
            var bedrijf = _userService.FindBedrijf();
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
            _stageopdrachtRepository.Delete(stageopdracht);
            _userService.SaveChanges();
            TempData["message"] = "Stageopdracht '" + stageopdracht.Titel + "' succesvol verwijderd.";
            return RedirectToAction("Index");
        }
        #endregion

        #region student voorkeurstages
        [Authorize(Role.student)]
        public ActionResult ToevoegenAanVoorkeur(int id)
        {
            var stageopdracht = _stageopdrachtRepository.FindGeldigeStageopdrachtById(id);

            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            var student = _userService.FindStudent();
            student.AddVoorkeurStage(stageopdracht);
            _userService.SaveChanges();
            return RedirectToAction("MijnVoorkeurStages");
        }

        [Authorize(Role.student)]
        public ActionResult VerwijderenUitVoorkeur(int[] ids)
        {
            var stageopdracht = _stageopdrachtRepository.FindById(ids[0]);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            var student = _userService.FindStudent();
            student.RemoveVoorkeurStages(ids);
            _userService.SaveChanges();
            return RedirectToAction("MijnVoorkeurStages");
        }

        [Authorize(Role.student)]
        public ActionResult MijnVoorkeurStages()
        {
            IEnumerable<Stageopdracht> stageopdrachten = null;
            var student = _userService.FindStudent();
            stageopdrachten = student.VoorkeurStages;
            var model = new StageopdrachtIndexVM() { Stageopdrachten = stageopdrachten };
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("Index", model);
        }
        #endregion

        #region stageopdracht beoordelen
        [Authorize(Role.Admin)]
        [ActionName("Goedkeuren")]
        public async Task<ActionResult> StageopdrachtGoedkeuren(int id)
        {
            var stageopdracht = _stageopdrachtRepository.FindById(id);
            Admin.KeurStageopdrachtGoed(stageopdracht);
            var emailService = Request.GetOwinContext().GetUserManager<ApplicationUserManager>().EmailService;
            await emailService.SendAsync(new IdentityMessage()
            {
                Destination = stageopdracht.Bedrijf.Email,
                Subject = "Stageopdracht goedgekeurd.",
                Body = "Stageopdracht " + stageopdracht.Titel + " goedgekeurd."
            });
            _stageopdrachtRepository.SaveChanges();
            return RedirectToAction("Voorstellen");
        }

        [Authorize(Role.Admin)]
        [ActionName("Afkeuren")]
        public ActionResult StageopdrachtAfkeuren(int id)
        {
            var stageopdracht = _stageopdrachtRepository.FindById(id);
            var model = Mapper.Map<StageopdrachtAfkeurenVM>(stageopdracht);
            model.Aan = stageopdracht.Bedrijf.Email;
            model.Onderwerp = "Stageopdracht afgekeurd";
            return View(model);
        }

        [HttpPost]
        [ActionName("Afkeuren")]
        [Authorize(Role.Admin)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StageopdrachtAfkeurenConfirmed(StageopdrachtAfkeurenVM model)
        {
            if (ModelState.IsValid)
            {
                var stageopdracht = _stageopdrachtRepository.FindById(model.Id);
                Admin.KeurStageopdrachtAf(stageopdracht);
                var emailService = Request.GetOwinContext().GetUserManager<ApplicationUserManager>().EmailService;
                await emailService.SendAsync(new IdentityMessage()
                {
                    Destination = stageopdracht.Bedrijf.Email,
                    Subject = model.Onderwerp,
                    Body = model.Reden
                });
                _stageopdrachtRepository.SaveChanges();
                return RedirectToAction("Voorstellen");
            }
            return View("Afkeuren", model);
        }
        #endregion

        #region begeleider voorkeuren
        [Authorize(Role.Begeleider)]
        public ActionResult AanvraagIndienen(int id)
        {
            var stageopdracht = _stageopdrachtRepository.FindById(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            var begeleider = _userService.FindBegeleider();
            begeleider.AddAanvraag(stageopdracht);
            _stageopdrachtRepository.SaveChanges();
            return RedirectToAction("Details", new { id = id });
        }

        [Authorize(Role.Begeleider)]
        public ActionResult AanvraagAnnuleren(int id)
        {
            var stageopdracht = _stageopdrachtRepository.FindById(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            var begeleider = _userService.FindBegeleider();
            if (begeleider.HeeftStageBegeleidingAangevraagd(id) == false)
            {
                return new HttpStatusCodeResult(403);
            }
            _stageopdrachtRepository.DeleteAanvraag(begeleider.FindAanvraag(stageopdracht));
            _stageopdrachtRepository.SaveChanges();
            return RedirectToAction("Details", new { id = id });
        }

        [Authorize(Role.Admin)]
        [ActionName("Aanvragen")]
        public ActionResult StageBegeleiderAanvragen()
        {
            var aanvragen = _stageopdrachtRepository.FindAllAanvragen();
            return View(aanvragen);
        }

        [Authorize(Role.Begeleider)]
        public ActionResult AanvragenBegeleider()
        {
            var begeleider = _userService.FindBegeleider();
            var aanvragen = _stageopdrachtRepository.FindAllAanvragenFrom(begeleider);
            return View("Aanvragen", aanvragen);
        }

        [Authorize(Role.Admin)]
        public ActionResult AanvraagGoedkeuren(int id)
        {
            var aanvraag = _stageopdrachtRepository.FindAanvraagById(id);
            if (aanvraag == null)
            {
                return HttpNotFound();
            }
            Admin.KeurStageBegeleidAanvraagGoed(aanvraag);
            _stageopdrachtRepository.SaveChanges();
            return RedirectToAction("Aanvragen");
        }

        [Authorize(Role.Admin)]
        public ActionResult AanvraagAfkeuren(int id)
        {
            var aanvraag = _stageopdrachtRepository.FindAanvraagById(id);
            if (aanvraag == null)
            {
                return HttpNotFound();
            }
            Admin.KeurStageBegeleidAanvraagAf(aanvraag);
            _stageopdrachtRepository.SaveChanges();
            return RedirectToAction("Aanvragen");
        }
        #endregion

    }


}