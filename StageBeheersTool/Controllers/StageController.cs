using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Ionic.Zip;
using StageBeheersTool.Helpers;
using StageBeheersTool.Models.DAL.Extensions;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.Models.Identity;
using StageBeheersTool.ViewModels;

namespace StageBeheersTool.Controllers
{
    public class StageController : BaseController
    {
        private readonly IStageopdrachtRepository _stageopdrachtRepository;
        private readonly IAcademiejaarRepository _academiejaarRepository;
        private readonly IStageRepository _stageRepository;
        private readonly IBegeleiderRepository _begeleiderRepository;
        private readonly ISpreadsheetService _spreadsheetService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IDocumentService _documentService;
        private readonly IInstellingenRepository _instellingenRepository;

        public StageController(IStageopdrachtRepository stageopdrachtRepository,
            IStageRepository stageRepository, IAcademiejaarRepository academiejaarRepository,
            IBegeleiderRepository begeleiderRepository, ISpreadsheetService spreadsheetService,
            IUserService userService, IEmailService emailService, IInstellingenRepository instellingenRepository,
            IDocumentService documentService)
        {
            _stageopdrachtRepository = stageopdrachtRepository;
            _academiejaarRepository = academiejaarRepository;
            _stageRepository = stageRepository;
            _begeleiderRepository = begeleiderRepository;
            _spreadsheetService = spreadsheetService;
            _userService = userService;
            _emailService = emailService;
            _documentService = documentService;
            _instellingenRepository = instellingenRepository;
        }

        #region Stage toewijzen
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
                return RedirectToLocal(Overzicht);
            }
            if (studentVoorkeurstage.Student.HeeftToegewezenStage())
            {
                SetViewError(Resources.ErrorStudentHeeftAlToegewezenStage);
                return RedirectToLocal(Overzicht);
            }
            var model = new StageAanStudentToewijzenVM
            {
                Stageopdracht = studentVoorkeurstage.Stageopdracht,
                Semester = studentVoorkeurstage.Stageopdracht.Semester1 ? 1 : 2,
                Student = studentVoorkeurstage.Student,
                StudentId = studentVoorkeurstage.Student.Id,
                StageopdrachtId = studentVoorkeurstage.Stageopdracht.Id
            };
            model.SetStageperiodes(_academiejaarRepository.FindVanHuidigAcademiejaar());
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
                    stage.SetAangepasteStageperiode(model.Begindatum, model.Einddatum, model.Semester);
                }
                stage.AcademiejaarInstellingen = _academiejaarRepository.FindByAcademiejaar(stage.Stageopdracht.Academiejaar);
                stage.Semester = model.Semester;
                _stageRepository.SaveChanges();
            }
            catch (ApplicationException ex)
            {
                SetViewError(ex.Message);
                return View(model);
            }
            SetViewMessage(string.Format(Resources.SuccesStageAanStudentToegewezen,
                studentVoorkeurstage.Stageopdracht.Titel, studentVoorkeurstage.Student.Naam));
            return RedirectToLocal(Overzicht);
        }

        #endregion

        #region lijsten
        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult List(StageListVM model)
        {
            var stages = _stageRepository.FindAllVanHuidigAcademiejaar()
                .WithFilter(model.Stageopdracht, model.Bedrijf, model.Student, model.Begeleider);
            model.Stages = stages;
            model.ToonBegeleider = true;
            model.ToonZoeken = true;
            model.ToonEdit = CurrentUser.IsAdmin();
            model.Title = Resources.TitelOverzichtStages + " " + AcademiejaarHelper.HuidigAcademiejaar();

            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageList", model);
            }
            return View("StageOverzicht", model);
        }

        [Authorize(Role.Begeleider)]
        public ActionResult MijnStages(StageListVM model)
        {
            var begeleider = _userService.GetBegeleider();
            model.Stages = begeleider.GetMijnStagesVanHuidigAcademiejaar();
            model.ToonActiviteitverslagen = true;
            model.Title = Resources.TitelMijnStages;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageList", model);
            }
            return View("StageOverzicht", model);
        }

        [Authorize(Role.Admin)]
        public ActionResult StagesToewijzen(StageListVM model)
        {
            var stageopdrachten = _stageopdrachtRepository.FindAllStudentVoorkeurenMetIngediendStagedossier();

            if (Request.IsAjaxRequest())
            {
                return PartialView("_StagesToewijzenList", stageopdrachten);
            }
            return View(stageopdrachten);
        }

        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult LijstExcel()
        {
            var begeleiders = _begeleiderRepository.FindAll();
            var academiejaren = _stageopdrachtRepository.FindAllAcademiejaren();
            var model = new StageLijstExcelVM();
            model.InitSelectLists(begeleiders, academiejaren);
            return View(model);
        }

        [Authorize(Role.Admin, Role.Begeleider)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LijstExcel(StageLijstExcelVM model)
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

            var stages = _stageRepository.FindAll()
                .WithFilter(model.SelectedStagebegeleiderId, model.SelectedAcademiejaar).ToList();

            _spreadsheetService.CreateSpreadsheet(model.TabbladNaam);
            _spreadsheetService.AddHeaders(model.SelectedOpties.ToList());
            _spreadsheetService.CreateColumnWidth(1, (uint)model.SelectedOpties.Length, 25);

            foreach (var stage in stages)
            {
                var row = new List<string>();
                foreach (var kolom in model.SelectedOpties)
                {
                    switch (kolom)
                    {
                        case "Titel":
                            row.Add(stage.Stageopdracht.Titel);
                            break;
                        case "Omschrijving":
                            row.Add(stage.Stageopdracht.Omschrijving);
                            break;
                        case "Stageplaats":
                            row.Add(stage.Stageopdracht.Stageplaats);
                            break;
                        case "Bedrijf":
                            row.Add(stage.Stageopdracht.Bedrijf.Naam);
                            break;
                        case "Begeleider":
                            row.Add(stage.Stageopdracht.Stagebegeleider == null ? "" : stage.Stageopdracht.Stagebegeleider.Naam);
                            break;
                        case "Student":
                            row.Add(stage.Student.Naam);
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

        #region details
        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult Details(int id)
        {
            var stage = _stageRepository.FindById(id);
            if (stage == null)
            {
                return HttpNotFound();
            }
            var model = new StageDetailsVM
            {
                Stage = stage,
                Begindatum = stage.GetBeginDatum(),
                Einddatum = stage.GetEinddatum(),
                ToonEdit = CurrentUser.IsAdmin(),
                ToonVerwijderen = CurrentUser.IsAdmin()
            };
            return View(model);
        }

        [Authorize(Role.Student)]
        public ActionResult MijnStage()
        {
            var student = _userService.GetStudent();
            var stage = student.Stage;
            return View(stage);
        }

        #endregion

        #region edit
        [Authorize(Role.Admin)]
        public ActionResult Edit(int id)
        {
            var stage = _stageRepository.FindById(id);
            if (stage == null)
            {
                return HttpNotFound();
            }
            var academiejaarInstellingen = _academiejaarRepository.FindVanHuidigAcademiejaar();
            var model = Mapper.Map<StageEditVM>(stage);
            model.SetStageperiodes(academiejaarInstellingen);
            return View(model);
        }

        [HttpPost]
        [Authorize(Role.Admin)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StageEditVM model)
        {
            if (ModelState.IsValid)
            {
                var stage = Mapper.Map<Stage>(model);
                _stageRepository.Update(stage);
                SetViewMessage(Resources.SuccesEditStage);
                return RedirectToAction("Details", new { stage.Id, Overzicht });
            }
            var academiejaarInstellingen = _academiejaarRepository.FindVanHuidigAcademiejaar();
            model.SetStageperiodes(academiejaarInstellingen);
            return View(model);
        }

        #endregion

        #region delete

        [Authorize(Role.Admin)]
        public ActionResult Delete(int id)
        {
            var stage = _stageRepository.FindById(id);
            if (stage == null)
            {
                return HttpNotFound();
            }
            return View(stage);
        }

        [Authorize(Role.Admin)]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string overzicht = "/Stage/List")
        {
            var stage = _stageRepository.FindById(id);
            if (stage == null)
            {
                return HttpNotFound();
            }
            _stageRepository.Delete(stage);
            SetViewMessage(Resources.SuccesDeleteStage);
            return RedirectToLocal(overzicht);
        }

        #endregion

        #region stagedossier

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
            return RedirectToLocal(Overzicht);
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
            return RedirectToLocal(Overzicht);
        }

        [Authorize(Role.Student)]
        public ActionResult AanduidenDossierIngediend(int id)
        {
            var stageopdracht = _stageopdrachtRepository.FindById(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            var student = _userService.GetStudent();
            if (student.HeeftStagedossierIngediend())
            {
                SetViewError(Resources.ErrorStagedossierReedsIngediend);
                return RedirectToAction("MijnVoorkeurStages", "Stageopdracht");
            }
            if (stageopdracht.IsBeschikbaar() == false)
            {
                SetViewError(Resources.ErrorStageNietMeerBeschikbaar);
                return RedirectToAction("MijnVoorkeurStages", "Stageopdracht");
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
            var student = _userService.GetStudent();
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
            return RedirectToAction("MijnVoorkeurStages", "Stageopdracht");
        }

        #endregion

        #region stagecontracten
        [Authorize(Role.Admin)]
        public ActionResult GenereerStagecontracten(StageListVM model)
        {
            var stages = _stageRepository.FindAllVanHuidigAcademiejaar()
                .Where(stage => stage.Stageopdracht.Stagebegeleider != null)
                .Include(stage => stage.Student)
                .OrderBy(stage => stage.Student.Familienaam);
            model.Stages = stages;
            return View(model);
        }
        
        //TODO:mergefields in word document bijvullen
        [Authorize(Role.Admin)]
        [HttpPost]
        public ActionResult GenereerStagecontracten(int[] id)
        {
            if (id == null)
            {
                SetViewError("Geen student geselecteerd.");
                return RedirectToAction("GenereerStagecontracten");
            }
            if (id.Length == 1)
            {
                return GenereerStagecontract(id[0]);
            }
            var outputStream = new MemoryStream();
            var stages = from key in id
                join stage in _stageRepository.FindAllVanHuidigAcademiejaar() on key equals stage.Id
                select stage;

            using (var zip = new ZipFile())
            {
                foreach (var stage in stages)
                {
                    var documentData = _documentService.GenerateStagecontract(stage);
                    zip.AddEntry("Stagecontract - " + stage.Student.Naam + ".docx", documentData.ToArray());
                }
                zip.Save(outputStream);
            }
            return new ZipFileResult(outputStream, "Stagecontracten");
        }

        [Authorize(Role.Admin)]
        public ActionResult GenereerStagecontract(int id)
        {
            var stage = _stageRepository.FindById(id);
            var documentData = _documentService.GenerateStagecontract(stage);
            return new DocFileResult(documentData, "Stagecontract - " + stage.Student.Naam);
        }

        #endregion

    }
}
