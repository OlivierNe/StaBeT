using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using AutoMapper;
using StageBeheersTool.Helpers;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.Models.Identity;
using StageBeheersTool.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace StageBeheersTool.Controllers
{
    [Authorize(Roles = Role.Admin)]
    public class InstellingenController : BaseController
    {
        private readonly IInstellingenRepository _instellingenRepository;
        private readonly IEmailService _emailService;
        private readonly IDocumentService _documentService;
        private readonly IStageRepository _stageRepository;

        public InstellingenController(IInstellingenRepository instellingenRepository,
            IEmailService emailService, IDocumentService documentService,
            IStageRepository stageRepository)
        {
            _instellingenRepository = instellingenRepository;
            _emailService = emailService;
            _documentService = documentService;
            _stageRepository = stageRepository;
        }

        #region academiejaar instellingen
        public ActionResult HuidigAcademiejaar()
        {
            var academiejaarInstellingen = _instellingenRepository.FindAcademiejaarInstellingVanHuidig();
            if (academiejaarInstellingen == null)
            {
                academiejaarInstellingen = new AcademiejaarInstellingen
                {
                    Academiejaar = AcademiejaarHelper.HuidigAcademiejaar()
                };
                _instellingenRepository.AddAcademiejaarInstelling(academiejaarInstellingen);
            }
            return View(Mapper.Map<AcademiejaarInstellingenVM>(academiejaarInstellingen));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HuidigAcademiejaar(AcademiejaarInstellingenVM model)
        {
            if (ModelState.IsValid)
            {
                var academiejaarInstellingen = Mapper.Map<AcademiejaarInstellingen>(model);
                _instellingenRepository.UpdateAcademiejaarInstelling(academiejaarInstellingen);
                SetViewMessage(string.Format(Resources.SuccesParametersAcademiejaar, model.Academiejaar));
            }
            return View(model);
        }

        public ActionResult AlleAcademiejaren()
        {
            return View(Mapper.Map<IEnumerable<AcademiejaarInstellingen>,
                IEnumerable<AcademiejaarInstellingenVM>>(_instellingenRepository.FindAllAcademiejaarInstellingen()));
        }

        public ActionResult CreateAcadInstellingen(string academiejaar = null)
        {
            AcademiejaarInstellingen academiejaarInstellingen;
            if (academiejaar != null)
            {
                academiejaarInstellingen = _instellingenRepository.FindAcademiejaarInstellingByAcademiejaar(academiejaar);
            }
            else
            {
                academiejaarInstellingen = new AcademiejaarInstellingen();
            }
            return View(Mapper.Map<AcademiejaarInstellingenVM>(academiejaarInstellingen));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAcadInstellingen(AcademiejaarInstellingenVM model)
        {
            if (_instellingenRepository.FindAcademiejaarInstellingByAcademiejaar(model.Academiejaar) != null)
            {
                ModelState.AddModelError("Academiejaar",
                    "Academiejaarinstellingen voor academiejaar " + model.Academiejaar + " bestaan al.");
            }
            if (ModelState.IsValid)
            {
                _instellingenRepository.AddAcademiejaarInstelling(Mapper.Map<AcademiejaarInstellingen>(model));
                SetViewMessage(string.Format(Resources.SuccesParametersAcademiejaar, model.Academiejaar));
                return View(model);
            }
            return View(model);
        }

        [Authorize(Role.Admin, Role.Bedrijf)]
        public ActionResult GetStageperiodes(string academiejaar)
        {
            if (academiejaar == null)
            {
                academiejaar = AcademiejaarHelper.HuidigAcademiejaar();
            }
            var academiejaarInstellingen = _instellingenRepository.FindAcademiejaarInstellingByAcademiejaar(academiejaar);
            if (academiejaarInstellingen == null)
            {
                return HttpNotFound();
            }

            var model = new
            {
                stageperiodeSemester1 = academiejaarInstellingen.StageperiodeSemester1(),
                stageperiodeSemester2 = academiejaarInstellingen.StageperiodeSemester2()
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region algemene instellingen

        public ActionResult AlgemeneInstellingen()
        {
            var model = new InstellingenVM();
            var instellingen = _instellingenRepository.FindAll();
            model.InitInstellingen(instellingen);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AlgemeneInstellingen(InstellingenVM model)
        {
            if (ModelState.IsValid)
            {
                _instellingenRepository.AddOrUpdate(new Instelling(Instelling.MailboxStages, model.MailboxStages));
                _instellingenRepository.AddOrUpdate(new Instelling(Instelling.AantalWekenStage,
                    model.AantalWekenStage.ToString(CultureInfo.InvariantCulture)));
                _instellingenRepository.AddOrUpdate(new Instelling(Instelling.BeginNieuwAcademiejaar,
                    new DateTime(1, model.Maand, model.Dag).ToString()));
                HttpContext.Cache.Remove("academiejaar");
                SetViewMessage(Resources.SuccesEditSaved);
            }
            return View(model);
        }

        #endregion

        #region standaardEmail

        public ActionResult StandaardEmails()
        {
            var standaardEmails = _emailService.FindStandaardEmails();
            foreach (var emailType in Enum.GetValues(typeof(EmailType)).Cast<EmailType>())
            {
                if (standaardEmails.FirstOrDefault(s => s.EmailType == emailType) == null)
                {
                    _emailService.AddStandaardEmail(new StandaardEmail
                    {
                        Gedeactiveerd = true,
                        EmailType = emailType
                    });
                }
            }
            return View(standaardEmails);
        }

        public ActionResult StandaardEmailEdit(int emailType)
        {
            var standaardEmail = _emailService.FindStandaardEmailByType((EmailType)emailType);
            if (standaardEmail == null)
            {
                standaardEmail = new StandaardEmail { EmailType = (EmailType)emailType };
                _emailService.AddStandaardEmail(standaardEmail);
            }
            return View(Mapper.Map<StandaardEmailVM>(standaardEmail));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StandaardEmailEdit(StandaardEmailVM model)
        {
            if (ModelState.IsValid)
            {
                var standaardEmail = Mapper.Map<StandaardEmail>(model);
                _emailService.UpdateStandaardEmail(standaardEmail);
                SetViewMessage(Resources.SuccesEditSaved);
                return RedirectToAction("StandaardEmails");
            }
            return View(model);
        }
        #endregion

        #region stagecontract template

        public ActionResult StagecontractTemplate()
        {
            return View();
        }

        public ActionResult DownloadStagecontractTemplate()
        {
            return new DocFileResult(Path.Combine(Server.MapPath("~/App_Data"),
                Path.GetFileName("Stagecontract Template.docx")), "Stagecontract Template");
        }

        public ActionResult UploadStagecontractTemplate(HttpPostedFileBase file)
        {
            if (file == null)
            {
                SetViewError("Geen bestand geselecteerd.");
                return RedirectToAction("StagecontractTemplate");
            }
            string fullPath = Request.MapPath("~/App_Data/Stagecontract Template.docx");
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
            file.SaveAs(Request.MapPath("~/App_Data/Stagecontract Template.docx"));

            SetViewMessage("Stagecontract template vervangen.");
            return RedirectToAction("StagecontractTemplate");
        }

        public ActionResult StagecontractTemplateTesten()
        {
            var testStage = _stageRepository.FindAll().FirstOrDefault();
            byte[] stagecontract = _documentService.GenerateStagecontract(testStage);
            return new DocFileResult(stagecontract, "Stagecontract - TEST");
        }

        public ActionResult StagecontractTemplateTerugzetten()
        {
            string fullPath = Server.MapPath("~/App_Data/Stagecontract Template.docx");
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
            System.IO.File.Copy(Server.MapPath("~/App_Data/Stagecontract Template - kopie.docx"), fullPath);
            SetViewMessage("Oorspronkelijke stagecontract template teruggezet.");
            return RedirectToAction("StagecontractTemplate");
        }

        #endregion

    }
}
