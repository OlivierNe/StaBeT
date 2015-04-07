using AutoMapper;
using Microsoft.AspNet.Identity;
using StageBeheersTool.Helpers;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.ViewModels;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using StageBeheersTool.Models.Authentication;
using System.Threading.Tasks;

namespace StageBeheersTool.Controllers
{
    [Authorize]
    public class StageopdrachtController : Controller
    {
        private readonly IStageopdrachtRepository _stageopdrachtRepository;
        private readonly IBedrijfRepository _bedrijfRepository;
        private readonly ISpecialisatieRepository _specialisatieRepository;
        private readonly IAcademiejaarRepository _academiejaarRepository;
        private readonly IUserService _userService;

        public StageopdrachtController(IStageopdrachtRepository stageopdrachtRepository,
            IBedrijfRepository bedrijfRepository, ISpecialisatieRepository specialisatieRepository,
            IUserService userService, IAcademiejaarRepository academiejaarRepository)
        {
            _stageopdrachtRepository = stageopdrachtRepository;
            _bedrijfRepository = bedrijfRepository;
            _specialisatieRepository = specialisatieRepository;
            _academiejaarRepository = academiejaarRepository;
            _userService = userService;
        }

        #region lijst stageopdrachten

        [Authorize(Role.Student, Role.Bedrijf, Role.Begeleider, Role.Admin)]
        public ActionResult Index(StageopdrachtIndexVM model)
        {
            var stageopdrachten = _stageopdrachtRepository.FindAll().WithFilter(model.Semester,
                model.AantalStudenten, model.Specialisatie, model.Bedrijf, model.Locatie, model.Student);
            model.Title = CurrentUser.IsBedrijf() ? "Overzicht stageopdrachten"
                : "Overzicht stageopdrachten " + AcademiejaarHelper.HuidigAcademiejaar();
            model.InitializeItems(stageopdrachten);
            model.ToonSearchForm = CurrentUser.IsBedrijf() == false;
            model.ToonAantalStudenten = true;
            model.ToonStudenten = CurrentUser.IsBegeleider() || CurrentUser.IsAdmin();
            model.ToonStatus = CurrentUser.IsAdmin();
            model.ToonBedrijf = CurrentUser.IsBedrijf();

            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View(model);
        }

        [Authorize(Role.Begeleider)]
        public ActionResult GoedgekeurdeStages(StageopdrachtIndexVM model)
        {
            var stageopdrachten = _stageopdrachtRepository.FindGeldigeBegeleiderStageopdrachten().WithFilter(model.Semester,
                model.AantalStudenten, model.Specialisatie, model.Bedrijf, model.Locatie, model.Student);
            model.InitializeItems(stageopdrachten);
            model.ToonSearchForm = true;
            model.ToonZoekenOpStudent = true;
            model.ToonStudenten = true;
            model.Title = "Toegewezen stages zonder begeleider";
            model.OverzichtAction = "GoedgekeurdeStages";
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View(model);
        }

        // nog niet beoordeelde voorstellen van bedrijven
        [Authorize(Role.Admin)]
        public ActionResult Voorstellen(StageopdrachtIndexVM model)
        {
            var stageopdrachten = _stageopdrachtRepository.FindStageopdrachtVoorstellen()
               .WithFilter(model.Semester, model.AantalStudenten, model.Specialisatie, model.Bedrijf, model.Locatie, null);
            model.ToonOordelen = true;
            model.ToonSearchForm = true;
            model.ToonStatus = true;
            model.ToonBedrijf = true;
            model.ToonAantalStudenten = true;
            model.InitializeItems(stageopdrachten);
            model.Title = "Stage voorstellen bedrijven " + AcademiejaarHelper.HuidigAcademiejaar();
            model.OverzichtAction = "Voorstellen";

            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View(model);
        }

        [Authorize(Role.Begeleider)]
        public ActionResult MijnStages()
        {
            var stageopdrachten = _stageopdrachtRepository.FindStageopdrachtenVanBegeleider();
            var model = new StageopdrachtIndexVM
            {
                Stageopdrachten = stageopdrachten,
                Title = "Mijn stages",
                ToonStudenten = true,
                OverzichtAction = "MijnStages"
            };
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View(model);
        }

        [Authorize(Role.Admin, Role.Begeleider, Role.Bedrijf)]
        public ActionResult Archief()
        {
            var academiejaren = _stageopdrachtRepository.FindAllAcademiejaren();
            return View(academiejaren);
        }

        [Authorize(Role.Begeleider)]
        public ActionResult MijnArchief()
        {
            var academiejaren = _stageopdrachtRepository.FindAllAcademiejarenVanBegeleider();
            return View(academiejaren);
        }

        [Authorize(Role.Admin, Role.Begeleider, Role.Bedrijf)]
        public ActionResult VanAcademiejaar(string academiejaar, string student, string bedrijf)
        {
            var stageopdrachten = _stageopdrachtRepository.FindAllVanAcademiejaar(academiejaar)
                .WithFilter(student: student, bedrijf: bedrijf);
            var academiejaren = _stageopdrachtRepository.FindAllAcademiejaren();
            var model = new StageopdrachtIndexVM
            {
                Stageopdrachten = stageopdrachten,
                OverzichtAction = "Archief",
                Title = "Stageopdrachten - " + academiejaar,
                Academiejaar = academiejaar,
                ToonStudenten = true,
                ToonBedrijf = true,
                Academiejaren = academiejaren
            };
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View(model);
        }

        [Authorize(Role.Begeleider)]
        public ActionResult MijnStagesVanAcademiejaar(string academiejaar, string student, string bedrijf)
        {
            var stageopdrachten = _stageopdrachtRepository.FindMijnStagesVanAcademiejaar(academiejaar)
                .WithFilter(student: student, bedrijf: bedrijf);
            var academiejaren = _stageopdrachtRepository.FindAllAcademiejarenVanBegeleider();
            var model = new StageopdrachtIndexVM
            {
                Stageopdrachten = stageopdrachten,
                OverzichtAction = "MijnStagesVanAcademiejaar",
                Title = academiejaar == null ? "Al mijn stages" : "Mijn Stages " + academiejaar,
                Academiejaar = academiejaar,
                ToonStudenten = true,
                ToonBedrijf = true,
                Academiejaren = academiejaren
            };
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("MijnStages", model);
        }
        #endregion

        #region create
        [Authorize(Role.Bedrijf, Role.Admin)]
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
            StageopdrachtCreateVM model;
            if (CurrentUser.IsBedrijf())
            {
                var bedrijf = _userService.FindBedrijf();
                model = new StageopdrachtCreateVM(_specialisatieRepository.FindAll(),
                    bedrijf.FindAllContractOndertekenaars(), bedrijf.FindAllStagementors());
                model.SetAdres(bedrijf.Gemeente, bedrijf.Postcode, bedrijf.Straat);
            }
            else //admin
            {
                model = new StageopdrachtCreateVM(_bedrijfRepository.FindAll(), _specialisatieRepository.FindAll());
            }
            model.AantalStudenten = 2;
            var academiejaarInstellingen = _academiejaarRepository.FindByHuidigAcademiejaar();
            if (academiejaarInstellingen != null)
            {
                model.StageperiodeSem1 = academiejaarInstellingen.StageperiodeSemester1();
                model.StageperiodeSem2 = academiejaarInstellingen.StageperiodeSemester2();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Role.Bedrijf, Role.Admin)]
        public ActionResult Create(StageopdrachtCreateVM model)
        {
            if (CurrentUser.IsAdmin() && model.BedrijfId == null)
            {
                ModelState.AddModelError("BedrijfId", Resources.ErrorBedrijfVerplicht);
            }
            var bedrijf = _bedrijfRepository.FindById(model.BedrijfId);
            if (ModelState.IsValid)
            {
                var stageopdracht = Mapper.Map<Stageopdracht>(model);
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
            if (CurrentUser.IsBedrijf())
            {
                model.SetSelectLists(_specialisatieRepository.FindAll(), bedrijf.FindAllContractOndertekenaars(),
                    bedrijf.FindAllStagementors());
            }
            else //admin
            {
                model.SetBedrijfSelectList(_bedrijfRepository.FindAll(), _specialisatieRepository.FindAll());
            }

            var academiejaarInstellingen = _academiejaarRepository.FindByHuidigAcademiejaar();
            if (academiejaarInstellingen != null)
            {
                model.StageperiodeSem1 = academiejaarInstellingen.StageperiodeSemester1();
                model.StageperiodeSem2 = academiejaarInstellingen.StageperiodeSemester2();
            }
            return View(model);
        }
        #endregion

        #region details

        [Authorize(Role.Student, Role.Admin, Role.Begeleider, Role.Bedrijf)]
        public ActionResult Details(int id, string overzicht = "Index")
        {
            var model = new StageopdrachtDetailsVM();
            var stageopdracht = _stageopdrachtRepository.FindById(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            var academiejaarInstellingen = _academiejaarRepository.FindByAcademiejaar(stageopdracht.Academiejaar);
            if (academiejaarInstellingen != null)
            {
                model.StageperiodeSem1 = academiejaarInstellingen.StageperiodeSemester1();
                model.StageperiodeSem2 = academiejaarInstellingen.StageperiodeSemester2();
            }
            if (CurrentUser.IsBedrijf())
            {
                var bedrijf = _userService.FindBedrijf();
                model.ToonEdit = academiejaarInstellingen == null
                    || bedrijf.MagStageopdracgtWijzigen(stageopdracht, academiejaarInstellingen.DeadlineBedrijfStageEdit);
                model.EditDeadline = academiejaarInstellingen == null ? null : academiejaarInstellingen.DeadlineBedrijfStageEditToString();
            }
            else if (CurrentUser.IsStudent())
            {
                var student = _userService.FindStudent();
                model.ToonVoorkeurVerwijderen = student.HeeftStageopdrachtAlsVoorkeur(id);
                model.ToonVoorkeurToevoegen = !model.ToonVoorkeurVerwijderen;
            }
            else if (CurrentUser.IsBegeleider())
            {
                var begeleider = _userService.FindBegeleider();
                model.ToonAanvraagIndienen = begeleider.MagAanvraagIndienen(stageopdracht);
                model.ToonAanvraagAnnuleren = begeleider.MagAanvraagAnnuleren(stageopdracht);
                model.ToonEdit = begeleider.MagStageWijzigen(stageopdracht);
            }
            if (AcademiejaarHelper.VroegerDanHuidig(stageopdracht.Academiejaar))
            {
                model.ToonEdit = false;
            }
            model.OverzichtAction = overzicht;
            model.Stageopdracht = stageopdracht;

            return View(model);
        }
        #endregion

        #region edit
        [Authorize(Role.Bedrijf, Role.Begeleider, Role.Admin)]
        public ActionResult Edit(int id)
        {
            var stageopdracht = _stageopdrachtRepository.FindById(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            var academiejaarInstellingen = _academiejaarRepository.FindByAcademiejaar(stageopdracht.Academiejaar);
            if (academiejaarInstellingen != null)
            {
                var deadline = academiejaarInstellingen.DeadlineBedrijfStageEdit;
                if (CurrentUser.IsBedrijf() && stageopdracht.Bedrijf.MagStageopdracgtWijzigen(stageopdracht, deadline) == false)
                {
                    return new HttpStatusCodeResult(403);
                }
            }
            var model = Mapper.Map<Stageopdracht, StageopdrachtEditVM>(stageopdracht);
            model.ContractondertekenaarId = stageopdracht.Contractondertekenaar == null ? null : (int?)stageopdracht.Contractondertekenaar.Id;
            model.StagementorId = stageopdracht.Stagementor == null ? null : (int?)stageopdracht.Stagementor.Id;
            model.SetSelectLists(_specialisatieRepository.FindAll(), stageopdracht.Bedrijf.FindAllContractOndertekenaars(),
                stageopdracht.Bedrijf.FindAllStagementors());
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Role.Bedrijf, Role.Begeleider, Role.Admin)]
        public ActionResult Edit(StageopdrachtEditVM model)
        {
            Stageopdracht stageopdracht;
            if (ModelState.IsValid)
            {
                var opdracht = _stageopdrachtRepository.FindById(model.Id);
                if (opdracht == null)
                {
                    return HttpNotFound();
                }
                var bedrijf = opdracht.Bedrijf;
                stageopdracht = Mapper.Map<StageopdrachtEditVM, Stageopdracht>(model);
                stageopdracht.Stagementor = model.StagementorId != null ? bedrijf.FindContactpersoonById((int)model.StagementorId) : null;
                stageopdracht.Contractondertekenaar = model.ContractondertekenaarId
                    != null ? bedrijf.FindContactpersoonById((int)model.ContractondertekenaarId) : null;
                _stageopdrachtRepository.Update(stageopdracht);
                _userService.SaveChanges();
                return RedirectToAction("Details", new { id = model.Id });
            }
            stageopdracht = _stageopdrachtRepository.FindById(model.Id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            model.SetSelectLists(_specialisatieRepository.FindAll(),
                stageopdracht.Bedrijf.FindAllContractOndertekenaars(),
                stageopdracht.Bedrijf.FindAllStagementors());
            return View(model);
        }

        #endregion

        #region stageopdracht verwijderen
        [Authorize(Role.Bedrijf, Role.Admin)]
        public ActionResult Delete(int id)
        {
            var stageopdracht = _stageopdrachtRepository.FindById(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            return View(stageopdracht);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Role.Bedrijf, Role.Admin)]
        public ActionResult DeleteConfirmed(int id)
        {
            var stageopdracht = _stageopdrachtRepository.FindById(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            if (CurrentUser.IsAdmin() == false && stageopdracht.IsGoedgekeurd())
            {
                TempData["message"] = "Goegekeurde stages kunnen niet meer verwijderd worden.";
                return RedirectToAction("Index");
            }
            _stageopdrachtRepository.Delete(stageopdracht);
            TempData["message"] = "Stageopdracht '" + stageopdracht.Titel + "' succesvol verwijderd.";
            return RedirectToAction("Index");
        }
        #endregion

        #region student voorkeurstages
        [Authorize(Role.Student)]
        public ActionResult ToevoegenAanVoorkeur(int id)
        {
            var stageopdracht = _stageopdrachtRepository.FindById(id);

            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            var student = _userService.FindStudent();
            student.AddVoorkeurStage(stageopdracht);
            _userService.SaveChanges();
            return RedirectToAction("MijnVoorkeurStages");
        }

        [Authorize(Role.Student)]
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

        [Authorize(Role.Student)]
        public ActionResult MijnVoorkeurStages()
        {
            var student = _userService.FindStudent();
            var stageopdrachten = student.VoorkeurStages;
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

        #region stagebegeleiding aanvragen
        [Authorize(Role.Begeleider)]
        public ActionResult AanvraagIndienen(int id, string overzicht)
        {
            var stageopdracht = _stageopdrachtRepository.FindById(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            var begeleider = _userService.FindBegeleider();
            begeleider.AddAanvraag(stageopdracht);
            _stageopdrachtRepository.SaveChanges();
            return RedirectToAction("Details", new { id, overzicht });
        }

        [Authorize(Role.Begeleider)]
        public ActionResult AanvraagAnnuleren(int id, string overzicht)
        {
            var stageopdracht = _stageopdrachtRepository.FindById(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            var begeleider = _userService.FindBegeleider();
            if (begeleider.HeeftStageBegeleidingAangevraagd(stageopdracht) == false)
            {
                return new HttpStatusCodeResult(403);
            }
            _stageopdrachtRepository.DeleteAanvraag(begeleider.FindAanvraag(stageopdracht));
            _stageopdrachtRepository.SaveChanges();
            return RedirectToAction("Details", new { id, overzicht });
        }

        [Authorize(Role.Admin)]
        public ActionResult AanvragenStagebegeleiding()
        {
            var aanvragen = _stageopdrachtRepository.FindAllAanvragen();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StagebegeleidingAanvragen", aanvragen);
            }
            return View(aanvragen);
        }

        [Authorize(Role.Begeleider)]
        public ActionResult MijnAanvragenStagebegeleiding()
        {
            var begeleider = _userService.FindBegeleider();
            var aanvragen = begeleider.StageAanvragen;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StagebegeleidingAanvragen", aanvragen);
            }
            return View("AanvragenStagebegeleiding", aanvragen);
        }

        [Authorize(Role.Admin)]
        public ActionResult AanvraagGoedkeuren(int id)
        {
            var aanvraag = _stageopdrachtRepository.FindAanvraagById(id);
            if (aanvraag == null)
            {
                return HttpNotFound();
            }
            var result = Admin.KeurStageBegeleidingAanvraagGoed(aanvraag);
            if (result == true)
            {
                TempData["message"] = string.Format(Resources.SuccesStagebegeleidingAanvraagGoedgekeurd,
                    aanvraag.Begeleider.Naam, aanvraag.Stage.Titel);
                _stageopdrachtRepository.SaveChanges();
            }
            else
            {
                TempData["error"] = string.Format(Resources.ErrorStagebegeleidingAanvraagGoedgekeurd,
                    aanvraag.Stage.Titel, aanvraag.Stage.Stagebegeleider.Naam);
            }
            return RedirectToAction("AanvragenStagebegeleiding");
        }

        [Authorize(Role.Admin)]
        public ActionResult AanvraagAfkeuren(int id)
        {
            var aanvraag = _stageopdrachtRepository.FindAanvraagById(id);
            if (aanvraag == null)
            {
                return HttpNotFound();
            }
            var result = Admin.KeurStageBegeleidAanvraagAf(aanvraag);
            if (result == true)
            {
                TempData["message"] = Resources.SuccesStagebegeleidingAfgekeurd;
            }
            else
            {
                TempData["message"] = string.Format(Resources.SuccesStagebegeleidingAfgekeurdBegeleiderLosgekoppeld,
                    aanvraag.Begeleider.Naam, aanvraag.Stage.Titel);
            }
            _stageopdrachtRepository.SaveChanges();
            return RedirectToAction("AanvragenStagebegeleiding");
        }
        #endregion

    }


}