using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Microsoft.AspNet.Identity;
using StageBeheersTool.Helpers;
using StageBeheersTool.Models.DAL.Extensions;
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
        private readonly IBegeleiderRepository _begeleiderRepository;
        private readonly ISpreadsheetService _spreadsheetService;

        public StageopdrachtController(IStageopdrachtRepository stageopdrachtRepository,
            IBedrijfRepository bedrijfRepository, ISpecialisatieRepository specialisatieRepository,
            IUserService userService, IAcademiejaarRepository academiejaarRepository,
            IBegeleiderRepository begeleiderRepository, ISpreadsheetService spreadsheetService)
        {
            _stageopdrachtRepository = stageopdrachtRepository;
            _bedrijfRepository = bedrijfRepository;
            _specialisatieRepository = specialisatieRepository;
            _academiejaarRepository = academiejaarRepository;
            _userService = userService;
            _begeleiderRepository = begeleiderRepository;
            _spreadsheetService = spreadsheetService;
        }

        #region lijsten

        //[Authorize(Role.Student, Role.Bedrijf, Role.Begeleider, Role.Admin)]
        //public ActionResult Index(StageopdrachtIndexVM model)
        //{
        //    var stageopdrachten = _stageopdrachtRepository.FindAll().WithFilter(model.Semester,
        //        model.AantalStudenten, model.Specialisatie, model.Bedrijf, model.Locatie, model.Student);
        //    model.Title = CurrentUser.IsBedrijf() ? "Overzicht stageopdrachten"
        //        : "Overzicht stageopdrachten " + AcademiejaarHelper.HuidigAcademiejaar();
        //    model.InitializeItems(stageopdrachten);
        //    model.ToonZoeken = CurrentUser.IsBedrijf() == false;
        //    model.ToonAantalStudenten = true;
        //    model.ToonStudenten = CurrentUser.IsBegeleider() || CurrentUser.IsAdmin();
        //    model.ToonStatus = CurrentUser.IsAdmin();
        //    model.ToonBedrijf = CurrentUser.IsBedrijf();

        //    if (Request.IsAjaxRequest())
        //    {
        //        return PartialView("_StageopdrachtList", model);
        //    }
        //    return View(model);
        //}

        /// <summary>
        /// Nog niet beoordeelde stageopdracht voorstellen van bedrijven
        /// </summary>
        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult BedrijfStageVoorstellen(StageopdrachtIndexVM model)
        {
            var stageopdrachten = _stageopdrachtRepository.FindStageopdrachtVoorstellen()
               .WithFilter(model.Semester, model.AantalStudenten, model.Specialisatie, model.Bedrijf, model.Locatie, null);
            model.ToonOordelen = CurrentUser.IsAdmin();
            model.ToonAantalStudenten = true;
            model.Stageopdrachten = stageopdrachten;
            model.Title = Resources.TitelBedrijfStageVoorstellen;
            model.OverzichtAction = ControllerContext.RouteData.Values["action"].ToString();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("StageOverzicht", model);
        }

        /// <summary>
        /// Alle goedgekeurde stageopdrachten van het huidige academiejaar
        /// </summary>
        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult GoedgekeurdeStages(StageopdrachtIndexVM model)
        {
            var stageopdrachten = _stageopdrachtRepository.FindGoedgekeurdeStageopdrachten().WithFilter(model.Semester,
                model.AantalStudenten, model.Specialisatie, model.Bedrijf, model.Locatie, null);
            model.Stageopdrachten = stageopdrachten;
            model.ToonZoeken = true;
            model.ToonAantalStudenten = true;
            model.ToonSemester = true;
            model.ToonBedrijf = true;
            model.Title = Resources.TitelGoedgekeurdeStageopdrachten;
            model.OverzichtAction = ControllerContext.RouteData.Values["action"].ToString();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("StageOverzicht", model);
        }

        /// <summary>
        /// Alle afgekeurde stageopdrachten van het huidige academiejaar
        /// </summary>
        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult AfgekeurdeStages(StageopdrachtIndexVM model)
        {
            var stageopdrachten = _stageopdrachtRepository.FindAfgekeurdeStageopdrachten().WithFilter(model.Semester,
                model.AantalStudenten, model.Specialisatie, model.Bedrijf, model.Locatie, null);
            model.Stageopdrachten = stageopdrachten;
            model.ToonZoeken = true;
            model.ToonAantalStudenten = true;
            model.ToonSemester = true;
            model.ToonBedrijf = true;
            model.Title = Resources.TitelAfgekeurdeStageopdrachten;
            model.OverzichtAction = ControllerContext.RouteData.Values["action"].ToString();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("StageOverzicht", model);
        }

        /// <summary>
        /// Alle beschikbare stages waar een student nog uit kan kiezen
        /// (enige lijst die studenten te zien krijgen)
        /// </summary>
        [Authorize(Role.Admin, Role.Begeleider, Role.Student)]
        public ActionResult BeschikbareStages(StageopdrachtIndexVM model)
        {
            var stageopdrachten = _stageopdrachtRepository.FindBeschikbareStages().WithFilter(model.Semester,
                model.AantalStudenten, model.Specialisatie, model.Bedrijf, model.Locatie, model.Student);
            model.Stageopdrachten = stageopdrachten;
            model.ToonZoeken = true;
            model.ToonBedrijf = true;
            model.ToonSemester = true;
            model.ToonAantalStudenten = true;
            model.Title = Resources.TitelGoedgekeurdeStageopdrachten;
            model.OverzichtAction = ControllerContext.RouteData.Values["action"].ToString();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("StageOverzicht", model);
        }

        /// <summary>
        /// Toegewezen stages met minstens 1 student zonder begeleider van het huidige academiejaar
        /// </summary>
        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult ToegewezenStagesZonderBegeleider(StageopdrachtIndexVM model)
        {
            var stageopdrachten = _stageopdrachtRepository.FindToegewezenStageopdrachtenZonderBegeleider().WithFilter(model.Semester,
                model.AantalStudenten, model.Specialisatie, model.Bedrijf, model.Locatie, model.Student);
            model.Stageopdrachten = stageopdrachten;
            model.ToonZoeken = true;
            model.ToonZoekenOpStudent = true;
            model.ToonStudenten = true;
            model.ToonBedrijf = true;
            model.ToonSemester = true;
            model.Title = Resources.TitelToegewezenStagesZonderBegeleider;
            model.OverzichtAction = ControllerContext.RouteData.Values["action"].ToString();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("StageOverzicht", model);
        }

        /// <summary>
        /// Toegewezen stages met minstens 1 student en een stagebegeleider van het huidige academiejaar
        /// </summary>
        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult ToegewezenStages(StageopdrachtIndexVM model)
        {
            var stageopdrachten = _stageopdrachtRepository.FindToegewezenStages().WithFilter(model.Semester,
                model.AantalStudenten, model.Specialisatie, model.Bedrijf, model.Locatie, model.Student);
            model.Stageopdrachten = stageopdrachten;
            model.ToonZoeken = true;
            model.ToonZoekenOpStudent = true;
            model.ToonStudenten = true;
            model.ToonBedrijf = true;
            model.ToonSemester = true;
            model.ToonBegeleider = true;
            model.Title = Resources.TitelToegewezenStages;
            model.OverzichtAction = ControllerContext.RouteData.Values["action"].ToString();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("StageOverzicht", model);
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

        [Authorize(Role.Admin)]
        public ActionResult LijstExcel()
        {
            var begeleiders = _begeleiderRepository.FindAll();
            var academiejaren = _stageopdrachtRepository.FindAllAcademiejaren();
            var model = new StageopdrachtLijstExcelVM();
            model.InitSelectLists(begeleiders, academiejaren);
            return View(model);
        }

        [Authorize(Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LijstExcel(StageopdrachtLijstExcelVM model)
        {
            var headers = model.GetHeaders();
            if (headers.Count <= 0)
            {
                ModelState.AddModelError("", "Geen kolom geselecteerd.");
            }

            if (ModelState.IsValid == false)
            {
                var begeleiders = _begeleiderRepository.FindAll();
                var academiejaren = _stageopdrachtRepository.FindAllAcademiejaren();
                model.InitSelectLists(begeleiders, academiejaren);
                return View(model);
            }
            var stages = _stageopdrachtRepository.FindAll()
                .WithFilter(academiejaar: model.SelectedAcademiejaar, begeleiderId: model.SelectedStagebegeleiderId,
                    status: (StageopdrachtStatus?)model.SelectedStatus).ToList();

            _spreadsheetService.CreateSpreadsheet(model.TabbladNaam);
            _spreadsheetService.AddHeaders(headers);
            _spreadsheetService.CreateColumnWidth(1, (uint)headers.Count, 25);

            foreach (var stage in stages)
            {
                var row = new List<string>();
                if (model.Titel)
                {
                    row.Add(stage.Titel);
                }
                if (model.Omschrijving)
                {
                    row.Add(stage.Omschrijving);
                }
                if (model.Stageplaats)
                {
                    row.Add(stage.Stageplaats);
                }
                if (model.Bedrijfsnaam)
                {
                    row.Add(stage.Bedrijf.Naam);
                }
                if (model.Bedrijfsadres)
                {
                    row.Add(stage.Bedrijf.Adres);
                }
                if (model.Begeleider)
                {
                    row.Add(stage.Stagebegeleider == null ? "" : stage.Stagebegeleider.Naam);
                }
                if (model.Studenten)
                {
                    row.Add(stage.ToonStudenten);
                }
                if (model.Status)
                {
                    row.Add(stage.Status.ToString());
                }
                _spreadsheetService.AddRow(row);
            }

            _spreadsheetService.CloseSpreadsheet();

            MemoryStream reportStream = _spreadsheetService.GetStream();
            return new ExcelFileResult(reportStream, model.Bestandsnaam);
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
                //TODO:return overzicht
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
            var model = new StageopdrachtIndexVM { Stageopdrachten = stageopdrachten };
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("Index", model);
        }
        #endregion

        #region stageopdracht beoordelen
        [Authorize(Role.Admin)]
        public async Task<ActionResult> StageopdrachtGoedkeuren(int id)
        {
            var stageopdracht = _stageopdrachtRepository.FindById(id);
            Admin.KeurStageopdrachtGoed(stageopdracht);
            var emailService = Request.GetOwinContext().GetUserManager<ApplicationUserManager>().EmailService;
            await emailService.SendAsync(new IdentityMessage
            {
                Destination = stageopdracht.Bedrijf.Email,
                Subject = "Stageopdracht goedgekeurd.",
                Body = "Stageopdracht " + stageopdracht.Titel + " werd goedgekeurd."
            });
            _stageopdrachtRepository.SaveChanges();
            TempData["message"] = string.Format(Resources.SuccesStageGoedgekeurd, stageopdracht.Titel);
            return RedirectToAction("BedrijfStageVoorstellen");
        }

        [Authorize(Role.Admin)]
        public ActionResult StageopdrachtAfkeuren(int id)
        {
            var stageopdracht = _stageopdrachtRepository.FindById(id);
            var model = Mapper.Map<StageopdrachtAfkeurenVM>(stageopdracht);
            model.Aan = stageopdracht.Bedrijf.Email;
            model.Titel = stageopdracht.Titel;
            model.Onderwerp = "Stageopdracht afgekeurd";
            return View(model);
        }

        [HttpPost]
        [ActionName("StageopdrachtAfkeuren")]
        [Authorize(Role.Admin)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StageopdrachtAfkeurenConfirmed(StageopdrachtAfkeurenVM model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }
            var stageopdracht = _stageopdrachtRepository.FindById(model.Id);
            Admin.KeurStageopdrachtAf(stageopdracht);
            var emailService = Request.GetOwinContext().GetUserManager<ApplicationUserManager>().EmailService;
            await emailService.SendAsync(new IdentityMessage
            {
                Destination = stageopdracht.Bedrijf.Email,
                Subject = model.Onderwerp,
                Body = model.Reden
            });
            _stageopdrachtRepository.SaveChanges();
            TempData["message"] = string.Format(Resources.SuccesStageAfgekeurd, stageopdracht.Titel);
            return RedirectToAction("BedrijfStageVoorstellen");
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