using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using StageBeheersTool.Helpers;
using StageBeheersTool.Models.DAL.Extensions;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.Models.Identity;
using StageBeheersTool.ViewModels;

namespace StageBeheersTool.Controllers
{
    public class StageController : Controller
    {
        private readonly IStageopdrachtRepository _stageopdrachtRepository;
        private readonly IAcademiejaarRepository _academiejaarRepository;
        private readonly IStageRepository _stageRepository;
        private readonly IBegeleiderRepository _begeleiderRepository;
        private readonly ISpreadsheetService _spreadsheetService;

        public StageController(IStageopdrachtRepository stageopdrachtRepository,
            IStageRepository stageRepository, IAcademiejaarRepository academiejaarRepository,
            IBegeleiderRepository begeleiderRepository, ISpreadsheetService spreadsheetService)
        {
            _stageopdrachtRepository = stageopdrachtRepository;
            _academiejaarRepository = academiejaarRepository;
            _stageRepository = stageRepository;
            _begeleiderRepository = begeleiderRepository;
            _spreadsheetService = spreadsheetService;
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
                return RedirectToAction("MetIngediendStagedossier", "Stageopdracht");
            }
            if (studentVoorkeurstage.Student.HeeftToegewezenStage())
            {
                SetViewError(Resources.ErrorStudentHeeftAlToegewezenStage);
                return RedirectToAction("MetIngediendStagedossier", "Stageopdracht");
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
            return RedirectToAction("MetIngediendStagedossier", "Stageopdracht");
        }

        #endregion

        [Authorize(Role.Admin)]
        public ActionResult List(StageListVM model)
        {
            var stages = _stageRepository.FindAllVanHuidigAcademiejaar()
                .WithFilter(model.Stageopdracht, model.Bedrijf, model.Student, model.Begeleider);
            model.Stages = stages;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageList", model);
            }
            return View("StageOverzicht", model);
        }

        [Authorize(Role.Admin)]
        public ActionResult Details(int id)
        {
            var stage = _stageRepository.FindById(id);
            var model = new StageDetailsVM
            {
                Stage = stage,
                Begindatum = stage.GetBeginDatum(),
                Einddatum = stage.GetEinddatum()
            };
            return View(model);
        }

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
                return RedirectToAction("Details", new { stage.Id });
            }
            var academiejaarInstellingen = _academiejaarRepository.FindVanHuidigAcademiejaar();
            model.SetStageperiodes(academiejaarInstellingen);
            return View(model);
        }

        [Authorize(Role.Admin)]
        public ActionResult LijstExcel()
        {
            var begeleiders = _begeleiderRepository.FindAll();
            var academiejaren = _stageopdrachtRepository.FindAllAcademiejaren();
            var model = new StageLijstExcelVM();
            model.InitSelectLists(begeleiders, academiejaren);
            return View(model);
        }


        [Authorize(Role.Admin)]
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


        #region Helpers
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