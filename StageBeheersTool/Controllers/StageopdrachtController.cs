﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using AutoMapper;
using StageBeheersTool.Helpers;
using StageBeheersTool.Models.DAL.Extensions;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.Models.Identity;
using StageBeheersTool.ViewModels;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace StageBeheersTool.Controllers
{
    [Authorize]
    public class StageopdrachtController : BaseController
    {
        private readonly IStageopdrachtRepository _stageopdrachtRepository;
        private readonly IBedrijfRepository _bedrijfRepository;
        private readonly ISpecialisatieRepository _specialisatieRepository;
        private readonly IInstellingenRepository _instellingenRepository;
        private readonly IUserService _userService;
        private readonly IBegeleiderRepository _begeleiderRepository;
        private readonly ISpreadsheetService _spreadsheetService;
        private readonly IEmailService _emailService;

        public StageopdrachtController(IStageopdrachtRepository stageopdrachtRepository,
            IBedrijfRepository bedrijfRepository, ISpecialisatieRepository specialisatieRepository,
            IUserService userService, IBegeleiderRepository begeleiderRepository,
            ISpreadsheetService spreadsheetService, IEmailService emailService,
            IInstellingenRepository instellingenRepository)
        {
            _stageopdrachtRepository = stageopdrachtRepository;
            _bedrijfRepository = bedrijfRepository;
            _specialisatieRepository = specialisatieRepository;
            _userService = userService;
            _begeleiderRepository = begeleiderRepository;
            _spreadsheetService = spreadsheetService;
            _emailService = emailService;
            _instellingenRepository = instellingenRepository;
        }

        #region lijsten

        [Authorize(Role.Student, Role.Bedrijf, Role.Begeleider, Role.Admin)]
        public ActionResult List(StageopdrachtListVM model)
        {
            if (CurrentUser.IsBegeleider())
            {
                return RedirectToAction("MijnStages", "Stage");
            }
            if (CurrentUser.IsStudent())
            {
                if (IdentityHelpers.StudentAcademiejaar() == AcademiejaarHelper.HuidigAcademiejaar())
                {
                    return RedirectToAction("MijnStage", "Stage");
                }
                return RedirectToAction("BeschikbareStageopdrachten", "Stageopdracht");
            }
            if (CurrentUser.IsAdmin())
            {
                return RedirectToAction("BedrijfStageVoorstellen", "Stageopdracht");
            }
            if (CurrentUser.IsBedrijf())
            {
                return RedirectToAction("BedrijfMijnStageopdrachten", "Stageopdracht");
            }
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// Nog niet beoordeelde stageopdracht voorstellen van bedrijven
        /// </summary>
        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult BedrijfStageVoorstellen(StageopdrachtListVM model)
        {
            var stageopdrachten = _stageopdrachtRepository.FindStageopdrachtVoorstellen()
               .WithFilter(locatie: model.Locatie, semester: model.Semester,
               specialisatie: model.Specialisatie, bedrijf: model.Bedrijf);
            model.ToonOordelen = CurrentUser.IsAdmin();
            model.ToonAantalStudenten = true;
            model.Stageopdrachten = stageopdrachten;
            model.Title = Resources.TitelBedrijfStageVoorstellen;
            return StageopdrachtView(model);
        }

        /// <summary>
        /// Alle goedgekeurde stageopdrachten van het huidige academiejaar
        /// </summary>
        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult GoedgekeurdeStages(StageopdrachtListVM model)
        {
            var stageopdrachten = _stageopdrachtRepository.FindGoedgekeurdeStageopdrachten()
                .WithFilter(bedrijf: model.Bedrijf, semester: model.Semester,
                aantalStudenten: model.AantalStudenten, specialisatie: model.Specialisatie,
                locatie: model.Locatie, academiejaar: model.Academiejaar ?? AcademiejaarHelper.HuidigAcademiejaar());
            model.Specialisaties = _specialisatieRepository.FindAll();
            model.Stageopdrachten = stageopdrachten;
            model.ToonZoeken = true;
            model.ToonAantalStudenten = true;
            model.ToonSemester = true;
            model.ToonBedrijf = true;
            model.ToonZoekenOpAcademiejaar = true;
            model.ToonZoekenOpBedrijf = true;
            model.Academiejaren = _stageopdrachtRepository.FindAllAcademiejaren();
            model.Title = Resources.TitelGoedgekeurdeStageopdrachten;
            return StageopdrachtView(model);
        }

        /// <summary>
        /// Alle afgekeurde stageopdrachten van het huidige academiejaar
        /// </summary>
        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult AfgekeurdeStages(StageopdrachtListVM model)
        {
            var stageopdrachten = _stageopdrachtRepository.FindAfgekeurdeStageopdrachten()
                .WithFilter(bedrijf: model.Bedrijf, semester: model.Semester,
                aantalStudenten: model.AantalStudenten, specialisatie: model.Specialisatie,
                locatie: model.Locatie, academiejaar: model.Academiejaar ?? AcademiejaarHelper.HuidigAcademiejaar());
            model.Stageopdrachten = stageopdrachten;
            model.ToonZoeken = true;
            model.Specialisaties = _specialisatieRepository.FindAll();
            model.ToonAantalStudenten = true;
            model.ToonSemester = true;
            model.ToonBedrijf = true;
            model.ToonZoekenOpBedrijf = true;
            model.ToonZoekenOpAcademiejaar = true;
            model.Academiejaren = _stageopdrachtRepository.FindAllAcademiejaren();
            model.Title = Resources.TitelAfgekeurdeStageopdrachten;
            return StageopdrachtView(model);
        }

        /// <summary>
        /// Alle beschikbare stageopdrachten waar een student nog uit kan kiezen
        /// (enige lijst die studenten te zien krijgen)
        /// </summary>
        [Authorize(Role.Admin, Role.Begeleider, Role.Student)]
        public ActionResult BeschikbareStageopdrachten(StageopdrachtListVM model)
        {
            var stageopdrachten = _stageopdrachtRepository.FindBeschikbareStageopdrachten()
                .WithFilter(student: model.Student, semester: model.Semester, aantalStudenten: model.AantalStudenten,
                specialisatie: model.Specialisatie, bedrijf: model.Bedrijf, locatie: model.Locatie);
            model.Stageopdrachten = stageopdrachten;
            model.ToonZoeken = true;
            model.Specialisaties = _specialisatieRepository.FindAll();
            model.ToonBedrijf = true;
            model.ToonSemester = true;
            model.ToonAantalStudenten = true;
            model.Title = Resources.TitelBeschikbareStageopdrachten;
            return StageopdrachtView(model);
        }

        /// <summary>
        /// Toegewezen stageopdrachten met minstens 1 student zonder begeleider van het huidige academiejaar
        /// </summary>
        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult ToegewezenStageopdrachtenZonderBegeleider(StageopdrachtListVM model)
        {
            var stageopdrachten = _stageopdrachtRepository.FindToegewezenStageopdrachtenZonderBegeleider()
                .WithFilter(student: model.Student, semester: model.Semester, locatie: model.Locatie,
                aantalStudenten: model.AantalStudenten, specialisatie: model.Specialisatie, bedrijf: model.Bedrijf);
            model.Stageopdrachten = stageopdrachten;
            model.ToonZoeken = true;
            model.Specialisaties = _specialisatieRepository.FindAll();
            model.ToonZoekenOpStudent = true;
            model.ToonStudenten = true;
            model.ToonBedrijf = true;
            model.ToonSemester = true;
            model.ToonZoekenOpBedrijf = true;
            model.Title = Resources.TitelToegewezenStageopdrachtenZonderBegeleider;
            return StageopdrachtView(model);
        }

        /// <summary>
        /// Toegewezen stageopdrachten met minstens 1 student en een stagebegeleider van het huidige academiejaar
        /// </summary>
        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult ToegewezenStageopdrachten(StageopdrachtListVM model)
        {
            var stageopdrachten = _stageopdrachtRepository.FindToegewezenStageopdrachten()
                .WithFilter(student: model.Student, semester: model.Semester, aantalStudenten: model.AantalStudenten,
               specialisatie: model.Specialisatie, bedrijf: model.Bedrijf, locatie: model.Locatie);
            model.Stageopdrachten = stageopdrachten;
            model.Specialisaties = _specialisatieRepository.FindAll();
            model.ToonZoeken = true;
            model.ToonZoekenOpStudent = true;
            model.ToonStudenten = true;
            model.ToonBedrijf = true;
            model.ToonSemester = true;
            model.ToonBegeleider = true;
            model.ToonZoekenOpBedrijf = true;
            model.Title = Resources.TitelToegewezenStageopdrachten;
            return StageopdrachtView(model);
        }

        /// <summary>
        /// Alle stages van de ingelogde begeleider
        /// </summary>
        [Authorize(Role.Begeleider)]
        public ActionResult MijnStageopdrachten()
        {
            var begeleider = _userService.GetBegeleider();
            var stageopdrachten = begeleider.GetStageopdrachtenVanHuidigAcademiejaar();
            var model = new StageopdrachtListVM
            {
                Stageopdrachten = stageopdrachten,
                Title = Resources.TitelMijnStageopdrachten,
                ToonStudenten = true,
                ToonZoekenOpStudent = true,
                ToonZoekenOpBedrijf = true
            };
            return StageopdrachtView(model);
        }

        /// <summary>
        /// alle stages van het ingelogde bedrijf
        /// </summary>
        [Authorize(Role.Bedrijf)]
        public ActionResult BedrijfMijnStageopdrachten()
        {
            var bedrijf = _userService.GetBedrijf();
            var stageopdrachten = bedrijf.GetStageopdrachtenVanHuidigAcademiejaar();
            var model = new StageopdrachtListVM
            {
                Stageopdrachten = stageopdrachten,
                Title = Resources.TitelMijnStageopdrachten + " " +
                    AcademiejaarHelper.HuidigAcademiejaar(),
                ToonStudenten = true,
                ToonZoekenOpStudent = true,
                ToonStatus = true
            };
            return StageopdrachtView(model);
        }

        /// <summary>
        /// alle stageopdrachten van een bedrijf
        /// </summary>
        [Authorize(Role.Admin)]
        public ActionResult VanBedrijf(int id)
        {
            var bedrijf = _bedrijfRepository.FindById(id);
            if (bedrijf == null)
            {
                return HttpNotFound();
            }
            var stageopdrachten = bedrijf.Stageopdrachten;
            var model = new StageopdrachtListVM
            {
                Stageopdrachten = stageopdrachten,
                Title = String.Format(Resources.TitelStageopdrachtenVanBedrijf, bedrijf.Naam),
                ToonStudenten = true,
                ToonZoekenOpStudent = true,
                ToonStatus = true,
                ToonAcademiejaar = true
            };
            return StageopdrachtView(model);
        }

        [Authorize(Role.Admin, Role.Begeleider, Role.Bedrijf)]
        public ActionResult Archief()
        {
            String[] academiejaren;
            if (CurrentUser.IsBedrijf())
            {
                var bedrijf = _userService.GetBedrijf();
                academiejaren = bedrijf.GetAcademiejaren();
            }
            else
            {
                academiejaren = _stageopdrachtRepository.FindAllAcademiejaren();
            }
            return View(academiejaren);
        }

        [Authorize(Role.Admin, Role.Begeleider, Role.Bedrijf)]
        public ActionResult VanAcademiejaar(string academiejaar, string student, string bedrijf)
        {
            String[] academiejaren;
            IEnumerable<Stageopdracht> stageopdrachten;
            if (CurrentUser.IsBedrijf())
            {
                var ingelogdBedrijf = _userService.GetBedrijf();
                stageopdrachten = ingelogdBedrijf.FindStageopdrachtenVanAcademiejaar(academiejaar)
                    .AsQueryable().WithFilter(student: student);
                academiejaren = ingelogdBedrijf.GetAcademiejaren();
            }
            else
            {
                stageopdrachten = _stageopdrachtRepository.FindAllVanAcademiejaar(academiejaar)
                .WithFilter(student: student, bedrijf: bedrijf);
                academiejaren = _stageopdrachtRepository.FindAllAcademiejaren();
            }
            var model = new StageopdrachtListVM
            {
                Stageopdrachten = stageopdrachten,
                Title = academiejaar == null ? "Alle stageopdrachten" : "Overzicht stageopdrachten " + academiejaar,
                Academiejaar = academiejaar,
                ToonStatus = academiejaar == null,
                ToonStudenten = true,
                ToonZoekenOpStudent = !CurrentUser.IsBedrijf(),
                ToonZoekenOpBedrijf = !CurrentUser.IsBedrijf(),
                ToonBedrijf = !CurrentUser.IsBedrijf(),
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
            var begeleider = _userService.GetBegeleider();
            var academiejaren = begeleider.GetAcademiejaren();
            return View(academiejaren);
        }

        [Authorize(Role.Begeleider)]
        public ActionResult MijnStagesVanAcademiejaar(string academiejaar, string student, string bedrijf)
        {
            var begeleider = _userService.GetBegeleider();
            var stageopdrachten = begeleider.FindStageopdrachtenVanAcademiejaar(academiejaar)
                .AsQueryable().WithFilter(student: student, bedrijf: bedrijf);
            var academiejaren = begeleider.GetAcademiejaren();
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
            return StageopdrachtView(model);
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
            if (model.SelectedOpties.Length <= 0)
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

            var stageopdrachten = _stageopdrachtRepository.FindAll()
                .WithFilter(academiejaar: model.SelectedAcademiejaar, begeleiderId: model.SelectedStagebegeleiderId,
                status: (StageopdrachtStatus?)model.SelectedStatus).ToList();

            _spreadsheetService.CreateSpreadsheet(model.TabbladNaam);
            _spreadsheetService.AddHeaders(model.SelectedOpties.ToList());
            _spreadsheetService.CreateColumnWidth(1, (uint)model.SelectedOpties.Length, 25);

            foreach (var stageopdracht in stageopdrachten)
            {
                var row = new List<string>();
                foreach (var kolom in model.SelectedOpties)
                {
                    switch (kolom)
                    {
                        case "Titel":
                            row.Add(stageopdracht.Titel);
                            break;
                        case "Omschrijving":
                            row.Add(stageopdracht.Omschrijving);
                            break;
                        case "Stageplaats":
                            row.Add(stageopdracht.Stageplaats);
                            break;
                        case "Bedrijf":
                            row.Add(stageopdracht.Bedrijf.Naam);
                            break;
                        case "Bedrijf E-mail":
                            row.Add(stageopdracht.Bedrijf.Email);
                            break;
                        case "Begeleider":
                            row.Add(stageopdracht.Stagebegeleider == null ? "" : stageopdracht.Stagebegeleider.Naam);
                            break;
                        case "Student":
                            row.Add(stageopdracht.ToonStudenten);
                            break;
                        case "Status":
                            row.Add(stageopdracht.Status.ToString());
                            break;
                    }
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
                var bedrijf = _userService.GetBedrijf();
                model = new StageopdrachtCreateVM(_specialisatieRepository.FindAll(),
                    bedrijf.FindAllContractOndertekenaars(), bedrijf.FindAllStagementors());
                model.SetAdres(bedrijf.Gemeente, bedrijf.Postcode, bedrijf.Straat);
            }
            else //admin
            {
                model = new StageopdrachtCreateVM(_bedrijfRepository.FindAll(), _specialisatieRepository.FindAll());
            }
            model.AantalStudenten = 2;
            model.SetStageperiodes(_instellingenRepository.FindAcademiejaarInstellingVanHuidig());
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
                bedrijf = _userService.GetBedrijf();
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
                var result = await _emailService.SendStandaardEmail(EmailType.StageopdrachtAangemaakt, geadresseerden: stageMailbox.Value);
                SetViewMessage(Resources.SuccesCreateStageopdracht + (result ? " E-mail verzonden." : ""));
                return RedirectToAction("Details", new { stageopdracht.Id, Overzicht });
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

            var academiejaarInstellingen = _instellingenRepository.FindAcademiejaarInstellingVanHuidig();
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
        public ActionResult Details(int id)
        {
            var model = new StageopdrachtDetailsVM();
            var stageopdracht = FindStageopdracht(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }

            var academiejaarInstellingen = _instellingenRepository.FindAcademiejaarInstellingVanHuidig();
            model.SetStageperiodes(academiejaarInstellingen);
            if (CurrentUser.IsBedrijf())
            {
                var bedrijf = stageopdracht.Bedrijf;
                model.ToonEdit = bedrijf.MagStageopdrachtWijzigen(stageopdracht, academiejaarInstellingen);
                model.ToonBedrijfeditDeadline = model.ToonEdit;
                model.ToonStatus = true;
            }
            else if (CurrentUser.IsStudent())
            {
                var student = _userService.GetStudent();
                if (student.MagStageopdrachtBekijken(stageopdracht) == false)
                {
                    return new HttpStatusCodeResult(403);
                }
                model.ToonVoorkeurVerwijderen = student.KanVoorkeurstageVerwijderen(stageopdracht);
                model.ToonVoorkeurToevoegen = !student.HeeftStageopdrachtAlsVoorkeur(stageopdracht.Id);
            }
            else if (CurrentUser.IsBegeleider())
            {
                var begeleider = _userService.GetBegeleider();
                model.ToonAanvraagIndienen = begeleider.MagAanvraagIndienen(stageopdracht);
                model.ToonAanvraagAnnuleren = begeleider.MagAanvraagAnnuleren(stageopdracht);
                model.ToonEdit = begeleider.MagStageopdrachtWijzigen(stageopdracht, academiejaarInstellingen);
                model.ToonBedrijfeditDeadline = model.ToonEdit;
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
            var academiejaarInstellingen = _instellingenRepository.FindAcademiejaarInstellingByAcademiejaar(stageopdracht.Academiejaar);
            if (CurrentUser.IsBedrijf() && stageopdracht.Bedrijf.MagStageopdrachtWijzigen(stageopdracht, academiejaarInstellingen) == false)
            {
                return new HttpStatusCodeResult(403);
            }
            if (CurrentUser.IsBegeleider() && _userService.GetBegeleider().MagStageopdrachtWijzigen(stageopdracht, academiejaarInstellingen) == false)
            {
                return new HttpStatusCodeResult(403);
            }
            var model = Mapper.Map<Stageopdracht, StageopdrachtEditVM>(stageopdracht);
            model.ContractondertekenaarId = stageopdracht.Contractondertekenaar == null ?
                null : (int?)stageopdracht.Contractondertekenaar.Id;
            model.StagementorId = stageopdracht.Stagementor == null ? null : (int?)stageopdracht.Stagementor.Id;
            model.SetSelectLists(_specialisatieRepository.FindAll(), stageopdracht.Bedrijf.FindAllContractOndertekenaars(),
                stageopdracht.Bedrijf.FindAllStagementors());
            model.IsToegewezen = stageopdracht.IsToegewezen();
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
                var academiejaarInstellingen = _instellingenRepository.FindAcademiejaarInstellingByAcademiejaar(stageopdracht.Academiejaar);
                if (CurrentUser.IsBedrijf() && stageopdracht.Bedrijf.MagStageopdrachtWijzigen(stageopdracht, academiejaarInstellingen) == false)
                {
                    return new HttpStatusCodeResult(403);
                }
                if (CurrentUser.IsBegeleider() && _userService.GetBegeleider().MagStageopdrachtWijzigen(stageopdracht, academiejaarInstellingen) == false)
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
                var mailboxStages = _instellingenRepository.Find(Instelling.MailboxStages);
                bool result;
                if (CurrentUser.IsBedrijf())
                {
                    result = await _emailService.SendStandaardEmail(EmailType.StageopdrachtGewijzigd, geadresseerden: mailboxStages.Value);
                }
                else
                {
                    result = await _emailService.SendStandaardEmail(EmailType.StageopdrachtGewijzigd,
                        geadresseerden: new[] { stageopdracht.Bedrijf.Email, mailboxStages.Value });
                }
                SetViewMessage(Resources.SuccesEditSaved + (result ? " E-mail verzonden." : ""));
                return RedirectToAction("Details", new { model.Id, Overzicht });
            }
            model.SetSelectLists(_specialisatieRepository.FindAll(),
                stageopdracht.Bedrijf.FindAllContractOndertekenaars(),
                stageopdracht.Bedrijf.FindAllStagementors());
            return View(model);
        }

        #endregion

        #region delete
        [Authorize(Role.Bedrijf, Role.Admin)]
        public ActionResult Delete(int id)
        {
            var stageopdracht = FindStageopdracht(id);
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
        public async Task<ActionResult> DeleteConfirmed(int id, string overzicht = "/Stageopdracht/List")
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
            bool result;
            try
            {
                _stageopdrachtRepository.Delete(stageopdracht);
                var stagesMailBox = _instellingenRepository.Find(Instelling.MailboxStages);
                result = await _emailService.SendStandaardEmail(EmailType.StageopdrachtVerwijderd, geadresseerden: stagesMailBox.Value);
            }
            catch (ApplicationException ex)
            {
                SetViewError(ex.Message);
                return View(stageopdracht);
            }
            SetViewMessage(string.Format(Resources.SuccesVerwijderStageopdracht, stageopdracht.Titel) + (result ? " E-mail verzonden." : ""));
            return RedirectToLocal(overzicht);
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
                var student = _userService.GetStudent();
                if (student.HeeftToegewezenStageInHuidigAcademiejaar())
                {
                    SetViewError("Toevoegen aan voorkeuren mislukt.");
                }
                else
                {
                    student.AddVoorkeurStage(stageopdracht);
                    _userService.SaveChanges();
                    SetViewMessage(string.Format(Resources.succesVoorkeurToevoegen, stageopdracht.Titel));
                }
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
                var student = _userService.GetStudent();
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
            var student = _userService.GetStudent();
            var stageopdrachten = student.GetAllVoorkeurStages();
            var gekozenStageopdracht = student.FindGekozenVoorkeurStage();

            var model = new StageopdrachtListVM
            {
                Stageopdrachten = stageopdrachten,
                Title = Resources.TitelMijnVoorkeurStages,
                ToonAantalStudenten = true,
                ToonBedrijf = true,
                ToonSemester = true,
                ToonDossierIndienen = true,
                ToonVoorkeurVerwijderen = student.KanVoorkeurstageVerwijderen(gekozenStageopdracht),
                StageIdDossierIngediend = gekozenStageopdracht == null ? null : (int?)gekozenStageopdracht.Id,
                CurrentStudentStagedossierStatus = student.GetStagedossierStatus()
            };
            return StageopdrachtView(model);
        }
        #endregion

        #region stageopdracht beoordelen

        [Authorize(Role.Admin)]
        public async Task<ActionResult> StageopdrachtGoedkeuren(int id)
        {
            var stageopdracht = _stageopdrachtRepository.FindById(id);
            Admin.KeurStageopdrachtGoed(stageopdracht);
            bool result = false;
            if (stageopdracht.Bedrijf.HeeftGeldigEmail())
            {
                result = await _emailService.SendStandaardEmail(EmailType.StageopdrachtGoedkeuren,
                    geadresseerden: stageopdracht.Bedrijf.Email);
            }
            else
            {
                SetViewError(String.Format(Resources.ErrorMailBedrijf, stageopdracht.Bedrijf.Naam, stageopdracht.Bedrijf.Email));
            }
            _stageopdrachtRepository.SaveChanges();
            SetViewMessage(string.Format(Resources.SuccesStageGoedgekeurd, stageopdracht.Titel) + (result ? " E-mail verzonden." : ""));
            return RedirectToLocal(Overzicht);
        }

        [Authorize(Role.Admin)]
        public ActionResult StageopdrachtAfkeuren(int id)
        {
            var stageopdracht = _stageopdrachtRepository.FindById(id);
            var model = Mapper.Map<StageopdrachtAfkeurenVM>(stageopdracht);
            model.Aan = stageopdracht.Bedrijf.Email;
            model.Titel = stageopdracht.Titel;
            return View(model);
        }

        [HttpPost]
        [Authorize(Role.Admin)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StageopdrachtAfkeuren(StageopdrachtAfkeurenVM model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }
            var stageopdracht = _stageopdrachtRepository.FindById(model.Id);
            try
            {
                Admin.KeurStageopdrachtAf(stageopdracht);
            }
            catch (ApplicationException ex)
            {
                SetViewError(ex.Message);
                return View(model);
            }
            bool result = false;
            if (stageopdracht.Bedrijf.HeeftGeldigEmail())
            {
                result = await _emailService.SendStandaardEmail(EmailType.StageopdrachtAfkeuren,
                   geadresseerden: model.Aan, reden: model.Reden);
            }
            else
            {
                SetViewError(String.Format(Resources.ErrorMailBedrijf, stageopdracht.Bedrijf.Naam, stageopdracht.Bedrijf.Email));
            }
            _stageopdrachtRepository.SaveChanges();
            SetViewMessage(string.Format(Resources.SuccesStageAfgekeurd, stageopdracht.Titel) + (result ? " E-mail verzonden." : ""));
            return RedirectToLocal(Overzicht);
        }
        #endregion

        #region stagebegeleiding aanvragen
        [Authorize(Role.Begeleider)]
        public ActionResult AanvraagIndienen(int id)
        {
            var stageopdracht = _stageopdrachtRepository.FindById(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            var begeleider = _userService.GetBegeleider();
            begeleider.AddAanvraag(stageopdracht);
            _stageopdrachtRepository.SaveChanges();
            return RedirectToAction("Details", new { id, Overzicht });
        }

        [Authorize(Role.Begeleider)]
        public ActionResult AanvraagAnnuleren(int id)
        {
            var stageopdracht = _stageopdrachtRepository.FindById(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            var begeleider = _userService.GetBegeleider();
            if (begeleider.HeeftStageBegeleidingAangevraagd(stageopdracht) == false)
            {
                return new HttpStatusCodeResult(403);
            }
            _stageopdrachtRepository.DeleteAanvraag(begeleider.FindAanvraag(stageopdracht));
            _stageopdrachtRepository.SaveChanges();
            return RedirectToAction("Details", new { id, Overzicht });
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
            var begeleider = _userService.GetBegeleider();
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
            return RedirectToLocal(Overzicht);
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
            return RedirectToLocal(Overzicht);
        }

        [Authorize(Role.Admin)]
        public ActionResult AanvraagVerwijderen(int id)
        {
            var aanvraag = _stageopdrachtRepository.FindAanvraagById(id);
            _stageopdrachtRepository.DeleteAanvraag(aanvraag);
            return RedirectToLocal(Overzicht);
        }
        #endregion

        #region Helpers

        private Stageopdracht FindStageopdracht(int id)
        {
            if (CurrentUser.IsBedrijf())
            {
                return _userService.GetBedrijf().FindStageopdrachtById(id);
            }
            return _stageopdrachtRepository.FindById(id);
        }

        private ActionResult StageopdrachtView(StageopdrachtListVM model)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("StageopdrachtOverzicht", model);
        }
        #endregion
    }
}