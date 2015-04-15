using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using StageBeheersTool.Helpers;
using StageBeheersTool.Models.DAL.Extensions;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.ViewModels;
using System.Web.Mvc;
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
        private readonly IInstellingenRepository _instellingenRepository;
        private readonly IUserService _userService;
        private readonly IBegeleiderRepository _begeleiderRepository;
        private readonly ISpreadsheetService _spreadsheetService;
        private readonly IEmailService _emailService;

        public StageopdrachtController(IStageopdrachtRepository stageopdrachtRepository,
            IBedrijfRepository bedrijfRepository, ISpecialisatieRepository specialisatieRepository,
            IUserService userService, IAcademiejaarRepository academiejaarRepository,
            IBegeleiderRepository begeleiderRepository, ISpreadsheetService spreadsheetService,
            IEmailService emailService, IInstellingenRepository instellingenRepository)
        {
            _stageopdrachtRepository = stageopdrachtRepository;
            _bedrijfRepository = bedrijfRepository;
            _specialisatieRepository = specialisatieRepository;
            _academiejaarRepository = academiejaarRepository;
            _userService = userService;
            _begeleiderRepository = begeleiderRepository;
            _spreadsheetService = spreadsheetService;
            _emailService = emailService;
            _instellingenRepository = instellingenRepository;
        }

        #region lijsten

        [Authorize(Role.Student, Role.Bedrijf, Role.Begeleider, Role.Admin)]
        public ActionResult Index(StageopdrachtListVM model)
        {
            if (CurrentUser.IsBegeleider())
            {
                return RedirectToAction("MijnStages", "Stageopdracht");
            }
            if (CurrentUser.IsBedrijf())
            {
                return RedirectToAction("BedrijfMijnStages", "Stageopdracht");
            }
            if (CurrentUser.IsAdmin())
            {
                return RedirectToAction("BedrijfStageVoorstellen", "Stageopdracht");
            }
            //student
            return RedirectToAction("BeschikbareStages", "Stageopdracht");
        }

        /// <summary>
        /// Nog niet beoordeelde stageopdracht voorstellen van bedrijven
        /// </summary>
        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult BedrijfStageVoorstellen(StageopdrachtListVM model)
        {
            var stageopdrachten = _stageopdrachtRepository.FindStageopdrachtVoorstellen()
               .WithFilter(model.Semester, model.AantalStudenten, model.Specialisatie, model.Bedrijf, model.Locatie, null);
            model.ToonOordelen = CurrentUser.IsAdmin();
            model.ToonAantalStudenten = true;
            model.Stageopdrachten = stageopdrachten;
            model.Title = Resources.TitelBedrijfStageVoorstellen;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("StageopdrachtOverzicht", model);
        }

        /// <summary>
        /// Alle goedgekeurde stageopdrachten van het huidige academiejaar
        /// </summary>
        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult GoedgekeurdeStages(StageopdrachtListVM model)
        {
            var stageopdrachten = _stageopdrachtRepository.FindGoedgekeurdeStageopdrachten().WithFilter(model.Semester,
                model.AantalStudenten, model.Specialisatie, model.Bedrijf, model.Locatie, null);
            model.Stageopdrachten = stageopdrachten;
            model.ToonZoeken = true;
            model.ToonAantalStudenten = true;
            model.ToonSemester = true;
            model.ToonBedrijf = true;
            model.Title = Resources.TitelGoedgekeurdeStageopdrachten;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("StageopdrachtOverzicht", model);
        }

        /// <summary>
        /// Alle afgekeurde stageopdrachten van het huidige academiejaar
        /// </summary>
        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult AfgekeurdeStages(StageopdrachtListVM model)
        {
            var stageopdrachten = _stageopdrachtRepository.FindAfgekeurdeStageopdrachten().WithFilter(model.Semester,
                model.AantalStudenten, model.Specialisatie, model.Bedrijf, model.Locatie, null);
            model.Stageopdrachten = stageopdrachten;
            model.ToonZoeken = true;
            model.ToonAantalStudenten = true;
            model.ToonSemester = true;
            model.ToonBedrijf = true;
            model.Title = Resources.TitelAfgekeurdeStageopdrachten;
            //  model.Overzicht = ControllerContext.RouteData.Values["action"].ToString();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("StageopdrachtOverzicht", model);
        }

        /// <summary>
        /// Alle beschikbare stages waar een student nog uit kan kiezen
        /// (enige lijst die studenten te zien krijgen)
        /// </summary>
        [Authorize(Role.Admin, Role.Begeleider, Role.Student)]
        public ActionResult BeschikbareStages(StageopdrachtListVM model)
        {
            var stageopdrachten = _stageopdrachtRepository.FindBeschikbareStages().WithFilter(model.Semester,
                model.AantalStudenten, model.Specialisatie, model.Bedrijf, model.Locatie, model.Student);
            model.Stageopdrachten = stageopdrachten;
            model.ToonZoeken = true;
            model.ToonBedrijf = true;
            model.ToonSemester = true;
            model.ToonAantalStudenten = true;
            model.Title = Resources.TitelBeschikbareStages;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("StageopdrachtOverzicht", model);
        }

        /// <summary>
        /// Toegewezen stages met minstens 1 student zonder begeleider van het huidige academiejaar
        /// </summary>
        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult ToegewezenStagesZonderBegeleider(StageopdrachtListVM model)
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
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("StageopdrachtOverzicht", model);
        }

        /// <summary>
        /// Toegewezen stages met minstens 1 student en een stagebegeleider van het huidige academiejaar
        /// </summary>
        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult ToegewezenStages(StageopdrachtListVM model)
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
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("StageopdrachtOverzicht", model);
        }

        /// <summary>
        /// Alle stages van de ingelogde begeleider
        /// </summary>
        [Authorize(Role.Begeleider)]
        public ActionResult MijnStages()
        {
            var stageopdrachten = _stageopdrachtRepository.FindStageopdrachtenVanHuidigeBegeleider();
            var model = new StageopdrachtListVM
            {
                Stageopdrachten = stageopdrachten,
                Title = Resources.TitelMijnStages,
                ToonStudenten = true,
            };
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("StageopdrachtOverzicht", model);
        }

        /// <summary>
        /// alle stages van het ingelogde bedrijf
        /// </summary>
        [Authorize(Role.Bedrijf)]
        public ActionResult BedrijfMijnStages()
        {
            var stageopdrachten = _stageopdrachtRepository.FindStageopdrachtenVanHuidigBedrijf();
            var model = new StageopdrachtListVM
            {
                Stageopdrachten = stageopdrachten,
                Title = Resources.TitelMijnStages,
                ToonStudenten = true,
                ToonStatus = true,
            };
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("StageopdrachtOverzicht", model);
        }

        [Authorize(Role.Admin, Role.Begeleider, Role.Bedrijf)]
        public ActionResult Archief()
        {
            var academiejaren = _stageopdrachtRepository.FindAllAcademiejaren();
            return View(academiejaren);
        }


        [Authorize(Role.Admin, Role.Begeleider, Role.Bedrijf)]
        public ActionResult VanAcademiejaar(string academiejaar, string student, string bedrijf)
        {
            var stageopdrachten = _stageopdrachtRepository.FindAllVanAcademiejaar(academiejaar)
                .WithFilter(student: student, bedrijf: bedrijf);
            var academiejaren = _stageopdrachtRepository.FindAllAcademiejaren();
            var model = new StageopdrachtListVM
            {
                Stageopdrachten = stageopdrachten,
                Title = academiejaar == null ? "Alle stageopdrachten" : "Overzicht stageopdrachten " + academiejaar,
                Academiejaar = academiejaar,
                ToonStatus = academiejaar == null,
                ToonStudenten = true,
                ToonBedrijf = true,
                ToonAcademiejaar = (academiejaar == null),
                Academiejaren = academiejaren
            };
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View(model);
        }

        [Authorize(Role.Begeleider)]
        public ActionResult MijnArchief()
        {
            var academiejaren = _stageopdrachtRepository.FindAllAcademiejarenVanBegeleider();
            return View(academiejaren);
        }

        [Authorize(Role.Begeleider)]
        public ActionResult MijnStagesVanAcademiejaar(string academiejaar, string student, string bedrijf)
        {
            var stageopdrachten = _stageopdrachtRepository.FindMijnStagesVanAcademiejaar(academiejaar)
                .WithFilter(student: student, bedrijf: bedrijf);
            var academiejaren = _stageopdrachtRepository.FindAllAcademiejarenVanBegeleider();
            var model = new StageopdrachtListVM
            {
                Stageopdrachten = stageopdrachten,
                Title = academiejaar == null ? "Al mijn stages" : "Mijn Stages " + academiejaar,
                Academiejaar = academiejaar,
                ToonStudenten = true,
                ToonBedrijf = true,
                ToonAcademiejaar = (academiejaar == null),
                Academiejaren = academiejaren
            };
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("StageopdrachtOverzicht", model);
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
                ModelState.AddModelError("", Resources.ErrorExcelGeenKolommen);
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
            var academiejaarInstellingen = _academiejaarRepository.FindVanHuidigAcademiejaar();
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
        public async Task<ActionResult> Create(StageopdrachtCreateVM model)
        {
            if (CurrentUser.IsAdmin() && model.BedrijfId == 0)
            {
                ModelState.AddModelError("BedrijfId", Resources.ErrorBedrijfVerplicht);
            }
            Bedrijf bedrijf;
            if (CurrentUser.IsBedrijf())
            {
                bedrijf = _userService.FindBedrijf();
            }
            else
            {
                bedrijf = _bedrijfRepository.FindById(model.BedrijfId);
            }

            if (ModelState.IsValid)
            {
                var stageopdracht = Mapper.Map<Stageopdracht>(model);
                if (stageopdracht.Stagementor == null)
                {
                    stageopdracht.Stagementor = model.StagementorId != null ?
                        bedrijf.FindContactpersoonById((int)model.StagementorId) : null;
                }
                else if (model.StagementorId == -1)
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
                    stageopdracht.Contractondertekenaar = model.ContractondertekenaarId != null ?
                        bedrijf.FindContactpersoonById((int)model.ContractondertekenaarId) : null;
                }
                else if (model.ContractondertekenaarId == -1)
                {
                    stageopdracht.Contractondertekenaar = stageopdracht.Stagementor;
                    bedrijf.AddContactpersoon(stageopdracht.Contractondertekenaar);
                }
                else if (model.ContractondertekenaarId == null)
                {
                    bedrijf.AddContactpersoon(stageopdracht.Contractondertekenaar);
                }
                bedrijf.AddStageopdracht(stageopdracht);
                _userService.SaveChanges();
                var stageMailbox = _instellingenRepository.Find(Instelling.MailboxStages);
                await _emailService.SendAsync(EmailMessages.StageopdrachtAangemaakt(stageopdracht, stageMailbox));
                SetViewMessage(Resources.SuccesCreateStageopdracht);
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

            var academiejaarInstellingen = _academiejaarRepository.FindVanHuidigAcademiejaar();
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
        public ActionResult Details(int id, string overzicht = "/Stageopdracht/Index")
        {
            var model = new StageopdrachtDetailsVM();
            var stageopdracht = FindStageopdracht(id);
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
                var bedrijf = stageopdracht.Bedrijf;
                model.ToonEdit = bedrijf.MagStageopdrachtWijzigen(stageopdracht, academiejaarInstellingen);
                model.EditDeadline = academiejaarInstellingen == null ? null : academiejaarInstellingen.DeadlineBedrijfStageEditToString();
                model.ToonBedrijfeditDeadline = model.ToonEdit;
                model.ToonStatus = true;
            }
            else if (CurrentUser.IsStudent())
            {
                var student = _userService.FindStudent();
                if (student.MagStageopdrachtBekijken(stageopdracht) == false)
                {
                    return new HttpStatusCodeResult(403);
                }
                model.ToonVoorkeurVerwijderen = student.KanVoorkeurstageVerwijderen(stageopdracht);
                model.ToonVoorkeurToevoegen = !student.HeeftStageopdrachtAlsVoorkeur(stageopdracht.Id);
            }
            else if (CurrentUser.IsBegeleider())
            {
                var begeleider = _userService.FindBegeleider();
                model.ToonAanvraagIndienen = begeleider.MagAanvraagIndienen(stageopdracht);
                model.ToonAanvraagAnnuleren = begeleider.MagAanvraagAnnuleren(stageopdracht);
                model.ToonEdit = begeleider.MagStageopdrachtWijzigen(stageopdracht);
                model.ToonStudenten = true;
            }
            else if (CurrentUser.IsAdmin())
            {
                model.ToonEdit = true;
                model.ToonOordelen = true;
                model.ToonStatus = true;
                model.ToonStudenten = true;
            }
            if (AcademiejaarHelper.VroegerDanHuidig(stageopdracht.Academiejaar))
            {
                model.ToonEdit = false;
            }
            model.Stageopdracht = stageopdracht;
            model.ToonVerwijderen = CurrentUser.IsAdmin() || CurrentUser.IsBedrijf() && (model.Stageopdracht.IsGoedgekeurd() == false);
            model.Overzicht = overzicht;
            return View(model);
        }
        #endregion

        #region edit
        [Authorize(Role.Bedrijf, Role.Begeleider, Role.Admin)]
        public ActionResult Edit(int id)
        {
            var stageopdracht = FindStageopdracht(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            var academiejaarInstellingen = _academiejaarRepository.FindByAcademiejaar(stageopdracht.Academiejaar);
            if (CurrentUser.IsBedrijf() && stageopdracht.Bedrijf.MagStageopdrachtWijzigen(stageopdracht, academiejaarInstellingen) == false)
            {
                return new HttpStatusCodeResult(403);
            }
            if (CurrentUser.IsBegeleider() && _userService.FindBegeleider().MagStageopdrachtWijzigen(stageopdracht) == false)
            {
                return new HttpStatusCodeResult(403);
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
        public async Task<ActionResult> Edit(StageopdrachtEditVM model)
        {
            var stageopdracht = FindStageopdracht(model.Id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                var academiejaarInstellingen = _academiejaarRepository.FindByAcademiejaar(stageopdracht.Academiejaar);
                if (CurrentUser.IsBedrijf() && stageopdracht.Bedrijf.MagStageopdrachtWijzigen(stageopdracht, academiejaarInstellingen) == false)
                {
                    return new HttpStatusCodeResult(403);
                }
                if (CurrentUser.IsBegeleider() && _userService.FindBegeleider().MagStageopdrachtWijzigen(stageopdracht) == false)
                {
                    return new HttpStatusCodeResult(403);
                }
                var bedrijf = stageopdracht.Bedrijf;
                stageopdracht = Mapper.Map<StageopdrachtEditVM, Stageopdracht>(model);
                stageopdracht.Stagementor = model.StagementorId != null ?
                    bedrijf.FindContactpersoonById((int)model.StagementorId) : null;
                stageopdracht.Contractondertekenaar = model.ContractondertekenaarId != null ?
                    bedrijf.FindContactpersoonById((int)model.ContractondertekenaarId) : null;
                stageopdracht.Bedrijf = bedrijf;
                _stageopdrachtRepository.Update(stageopdracht);
                await _emailService.SendAsync(EmailMessages.StageopdrachtGewijzigd(stageopdracht));
                return RedirectToAction("Details", new { model.Id });
            }
            model.SetSelectLists(_specialisatieRepository.FindAll(),
                stageopdracht.Bedrijf.FindAllContractOndertekenaars(),
                stageopdracht.Bedrijf.FindAllStagementors());
            return View(model);
        }

        #endregion

        #region stageopdracht verwijderen
        [Authorize(Role.Bedrijf, Role.Admin)]
        public ActionResult Delete(int id, string overzicht = "/Stageopdracht/Index")
        {
            var stageopdracht = FindStageopdracht(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            ViewBag.Overzicht = overzicht;
            return View(stageopdracht);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Role.Bedrijf, Role.Admin)]
        public ActionResult DeleteConfirmed(int id, string overzicht = "/Stageopdracht/Index")
        {
            var stageopdracht = FindStageopdracht(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            if (CurrentUser.IsBedrijf() && stageopdracht.IsGoedgekeurd())
            {
                SetViewError(Resources.ErrorVerwijderGoedgekeurdeStage);
                return View(stageopdracht);
            }
            try
            {
                _stageopdrachtRepository.Delete(stageopdracht);
                var stagesMailBox = _instellingenRepository.Find(Instelling.MailboxStages);
                _emailService.SendAsync(EmailMessages.StageopdrachtVerwijderd(stageopdracht, stagesMailBox));
            }
            catch (ApplicationException ex)
            {
                SetViewError(ex.Message);
                return View(stageopdracht);
            }
            SetViewMessage(string.Format(Resources.SuccesVerwijderStageopdracht, stageopdracht.Titel));
            return Redirect(overzicht);
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
            try
            {
                var student = _userService.FindStudent();
                student.AddVoorkeurStage(stageopdracht);
                _userService.SaveChanges();
                SetViewMessage(string.Format(Resources.succesVoorkeurToevoegen, stageopdracht.Titel));
            }
            catch (ApplicationException ex)
            {
                SetViewError(ex.Message);
            }
            return RedirectToAction("MijnVoorkeurStages");
        }

        [Authorize(Role.Student)]
        public ActionResult VerwijderenUitVoorkeur(int id)
        {
            var stageopdracht = _stageopdrachtRepository.FindById(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            try
            {
                var student = _userService.FindStudent();
                student.RemoveVoorkeurStage(stageopdracht);
                _userService.SaveChanges();
                SetViewMessage(string.Format(Resources.SuccesVoorkeurStageVerwijderen, stageopdracht.Titel));
            }
            catch (ApplicationException ex)
            {
                SetViewError(ex.Message);
            }
            return RedirectToAction("MijnVoorkeurStages");
        }

        [Authorize(Role.Student)]
        public ActionResult MijnVoorkeurStages()
        {
            var student = _userService.FindStudent();
            var stageopdrachten = student.GetAllVoorkeurStages();
            var gekozenStage = student.FindGekozenVoorkeurStage();

            var model = new StageopdrachtListVM
            {
                Stageopdrachten = stageopdrachten,
                Title = Resources.TitelMijnVoorkeurStages,
                ToonAantalStudenten = true,
                ToonBedrijf = true,
                ToonSemester = true,
                ToonDossierIndienen = true,
                StageIdDossierIngediend = gekozenStage == null ? null : (int?)gekozenStage.Id,
                CurrentStudentStagedossierStatus = student.GetStagedossierStatus()
            };

            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("StageopdrachtOverzicht", model);
        }
        #endregion

        #region stagedossier

        [Authorize(Role.Student)]
        public ActionResult AanduidenDossierIngediend(int id)
        {
            var stageopdracht = _stageopdrachtRepository.FindById(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            var student = _userService.FindStudent();
            if (student.HeeftStagedossierIngediend())
            {
                SetViewError(Resources.ErrorStagedossierReedsIngediend);
                return RedirectToAction("MijnVoorkeurStages");
            }
            if (stageopdracht.IsBeschikbaar() == false)
            {
                SetViewError(Resources.ErrorStageNietMeerBeschikbaar);
                return RedirectToAction("MijnVoorkeurStages");
            }
            return View(stageopdracht);
        }

        [Authorize(Role.Student)]
        [ActionName("AanduidenDossierIngediend")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AanduidenDossierIngediendConfirmed(int id)
        {
            var stageopdracht = _stageopdrachtRepository.FindById(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            var student = _userService.FindStudent();
            try
            {
                student.SetStagedossierIngediend(stageopdracht);
                _userService.SaveChanges();
                SetViewMessage(string.Format(Resources.SuccesStagedossierAangeduidAlsIngediend,
                     stageopdracht.Titel));
                var stagesMailbox = _instellingenRepository.Find(Instelling.MailboxStages);
                await _emailService.SendAsync(EmailMessages.StagedossierIngediend(stageopdracht, student, stagesMailbox));
            }
            catch (ApplicationException ex)
            {
                SetViewError(ex.Message);
            }
            return RedirectToAction("MijnVoorkeurStages");
        }

        [Authorize(Role.Admin)]
        public ActionResult MetIngediendStagedossier()
        {
            var stageopdrachten = _stageopdrachtRepository.FindAllStudentVoorkeurenMetIngediendStagedossier();

            if (Request.IsAjaxRequest())
            {
                return PartialView("_StagesMetStagedossier", stageopdrachten);
            }
            return View("StagesMetStagedossier", stageopdrachten);
        }

        [Authorize(Role.Admin)]
        public ActionResult StagedossierGoedkeuren(int studentId, int stageId)
        {
            var studentVoorkeurstage = _stageopdrachtRepository
                .FindStudentVoorkeurStageByIds(stageId: stageId, studentId: studentId);
            if (studentVoorkeurstage == null)
            {
                return HttpNotFound();
            }
            Admin.KeurStagedossierGoed(studentVoorkeurstage);
            SetViewMessage(string.Format(Resources.SuccesStagedossierGoedgekeurd,
                studentVoorkeurstage.Student.Naam));
            _stageopdrachtRepository.SaveChanges();
            return RedirectToAction("MetIngediendStagedossier");
        }

        [Authorize(Role.Admin)]
        public ActionResult StagedossierAfkeuren(int studentId, int stageId)
        {
            var studentVoorkeurstage = _stageopdrachtRepository
                .FindStudentVoorkeurStageByIds(stageId: stageId, studentId: studentId);
            if (studentVoorkeurstage == null)
            {
                return HttpNotFound();
            }
            Admin.KeurStagedossierAf(studentVoorkeurstage);
            SetViewMessage(string.Format(Resources.SuccesStagedossierAfgekeurd, studentVoorkeurstage.Student.Naam));
            _stageopdrachtRepository.SaveChanges();
            return RedirectToAction("MetIngediendStagedossier");
        }

        [Authorize(Role.Admin)]
        public ActionResult StageToewijzen(int studentId, int stageId)
        {
            var studentVoorkeurstage = _stageopdrachtRepository
              .FindStudentVoorkeurStageByIds(stageId: stageId, studentId: studentId);
            if (studentVoorkeurstage == null)
            {
                return HttpNotFound();
            }
            if (studentVoorkeurstage.HeeftGoedgekeurdStagedossier() == false)
            {
                SetViewError(Resources.ErrorStageAanStudentKoppelenZonderGoedgekeurdStagedossier);
                return RedirectToAction("MetIngediendStagedossier");
            }
            if (studentVoorkeurstage.Student.HeeftToegewezenStage())
            {
                SetViewError(Resources.ErrorStudentHeeftAlToegewezenStage);
                return RedirectToAction("MetIngediendStagedossier");
            }
            var academiejaarInstellingen = _academiejaarRepository.FindVanHuidigAcademiejaar();
            var model = new StageAanStudentToewijzenVM();

            if (academiejaarInstellingen != null)
            {
                model.Semester1Begin = academiejaarInstellingen.Semester1Begin;
                model.Semester1Einde = academiejaarInstellingen.Semester1Einde;
                model.Semester2Begin = academiejaarInstellingen.Semester2Begin;
                model.Semester2Einde = academiejaarInstellingen.Semester2Einde;
            }
            model.Semester = studentVoorkeurstage.Stageopdracht.Semester1 ? 1 : 2;
            model.Stageopdracht = studentVoorkeurstage.Stageopdracht;
            model.Student = studentVoorkeurstage.Student;
            model.StudentId = studentVoorkeurstage.Student.Id;
            model.StageopdrachtId = studentVoorkeurstage.Stageopdracht.Id;

            return View(model);
        }

        [HttpPost]
        [Authorize(Role.Admin)]
        [ValidateAntiForgeryToken]
        public ActionResult StageToewijzen(StageAanStudentToewijzenVM model)
        {
            var studentVoorkeurstage = _stageopdrachtRepository
               .FindStudentVoorkeurStageByIds(stageId: model.StageopdrachtId, studentId: model.StudentId);
            if (studentVoorkeurstage == null)
            {
                return HttpNotFound();
            }
            model.Stageopdracht = studentVoorkeurstage.Stageopdracht;
            model.Student = studentVoorkeurstage.Student;
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            try
            {
                var stage = Admin.KoppelStageopdrachtAanStudent(studentVoorkeurstage);
                if (model.AangepasteStageperiode)
                {
                    stage.SetAangepasteStageperiode((DateTime)model.Begindatum, (DateTime)model.Einddatum, model.Semester);
                }
                else
                {
                    var acadParameters = _academiejaarRepository.FindVanHuidigAcademiejaar();
                    stage.SetStageperiode(acadParameters, model.Semester);
                }
                _stageopdrachtRepository.SaveChanges();
            }
            catch (ApplicationException ex)
            {
                SetViewError(ex.Message);
                return View(model);
            }
            SetViewMessage(string.Format(Resources.SuccesStageAanStudentToegewezen,
                studentVoorkeurstage.Stageopdracht.Titel, studentVoorkeurstage.Student.Naam));
            return RedirectToAction("MetIngediendStagedossier");
        }

        #endregion

        #region stageopdracht beoordelen
        [Authorize(Role.Admin)]
        public async Task<ActionResult> StageopdrachtGoedkeuren(int id)
        {
            var stageopdracht = _stageopdrachtRepository.FindById(id);
            Admin.KeurStageopdrachtGoed(stageopdracht);
            if (stageopdracht.Bedrijf.HeeftGeldigEmail())
            {
                await _emailService.SendAsync(EmailMessages.StageopdrachtGoedkeurenMail(stageopdracht));
            }
            else
            {
                SetViewError(String.Format(Resources.ErrorMailBedrijf, stageopdracht.Bedrijf.Naam, stageopdracht.Bedrijf.Email));
            }
            _stageopdrachtRepository.SaveChanges();
            SetViewMessage(string.Format(Resources.SuccesStageGoedgekeurd, stageopdracht.Titel));
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
            if (stageopdracht.Bedrijf.HeeftGeldigEmail())
            {
                await _emailService.SendAsync(EmailMessages.StageopdrachtAfkeurenMail(stageopdracht, model.Reden,
                      model.Onderwerp));
            }
            else
            {
                SetViewError(String.Format(Resources.ErrorMailBedrijf, stageopdracht.Bedrijf.Naam, stageopdracht.Bedrijf.Email));
                return View(model);
            }
            _stageopdrachtRepository.SaveChanges();
            SetViewMessage(string.Format(Resources.SuccesStageAfgekeurd, stageopdracht.Titel));
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
            try
            {
                Admin.KeurStageBegeleidingAanvraagGoed(aanvraag);
                SetViewMessage(string.Format(Resources.SuccesStagebegeleidingAanvraagGoedgekeurd,
                    aanvraag.Begeleider.Naam, aanvraag.Stage.Titel));
                _stageopdrachtRepository.SaveChanges();
            }
            catch (ApplicationException ex)
            {
                SetViewError(ex.Message);
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
            SetViewMessage(result);
            _stageopdrachtRepository.SaveChanges();
            return RedirectToAction("AanvragenStagebegeleiding");
        }
        #endregion

        #region Helpers

        private Stageopdracht FindStageopdracht(int id)
        {
            if (CurrentUser.IsBedrijf())
            {
                return _userService.FindBedrijf().FindStageopdrachtById(id);
            }
            return _stageopdrachtRepository.FindById(id);
        }

        private void SetViewError(string error)
        {
            TempData["error"] = error;
        }

        private void SetViewMessage(string message)
        {
            TempData["message"] = message;
        }

        #endregion
    }
}