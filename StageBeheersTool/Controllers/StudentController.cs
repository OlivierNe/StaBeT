using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using HtmlAgilityPack;
using StageBeheersTool.Models.DAL.Extensions;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.Models.Identity;
using StageBeheersTool.ViewModels;
using System.Web;
using System.Web.Mvc;
using AutoMapper;

namespace StageBeheersTool.Controllers
{
    [Authorize]
    public class StudentController : BaseController
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IKeuzepakketRepository _keuzepakketRepository;
        private readonly IUserService _userService;
        private readonly IImageService _imageService;
        private readonly ISpreadsheetService _spreadsheetService;

        public StudentController(IStudentRepository studentRepository, IKeuzepakketRepository keuzepakketRepository,
            IUserService userService, IImageService imageService, ISpreadsheetService spreadsheetService)
        {
            _studentRepository = studentRepository;
            _keuzepakketRepository = keuzepakketRepository;
            _imageService = imageService;
            _userService = userService;
            _spreadsheetService = spreadsheetService;
        }

        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult List(StudentListVM model)
        {
            var studenten = _studentRepository.FindAll().WithFilter(model.Naam, model.Voornaam);
            model.Studenten = studenten;
            model.ToonActies = CurrentUser.IsAdmin();

            if (Request.IsAjaxRequest())
            {
                return PartialView("_studentList", model);
            }
            return View(model);
        }

        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult MetToegewezenStage(StudentListVM model)
        {
            var studenten = _studentRepository.FindStudentenMetToegewezenStage()
                .WithFilter(model.Naam, model.Voornaam);
            model.Studenten = studenten;

            model.ToonStage = true;

            if (Request.IsAjaxRequest())
            {
                return PartialView("_studentList", model);
            }
            return View("List", model);
        }

        [Authorize(Role.Admin)]
        public ActionResult Create()
        {
            var model = new StudentCreateVM();
            model.SetKeuzevakSelectList(_keuzepakketRepository.FindAll());
            model.LoginAccountAanmaken = true;
            return View(model);
        }

        [Authorize(Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StudentCreateVM model)
        {
            if (ModelState.IsValid)
            {
                var student = Mapper.Map<Student>(model);
                student.Keuzepakket = model.KeuzepakketId == null ? null : _keuzepakketRepository.FindBy((int)model.KeuzepakketId);
                try
                {
                    _studentRepository.Add(student);
                    SetViewMessage(string.Format(Resources.SuccesStudentCreate, student.Naam));
                    if (model.LoginAccountAanmaken)
                    {
                        _userService.CreateLogin(student.HogentEmail, "wachtwoord", Role.Student);//TODO: tijdelijk "wachtwoord"
                    }
                    return RedirectToAction("Details", new { student.Id });
                }
                catch (ApplicationException ex)
                {
                    SetViewError(ex.Message);
                }
            }
            model.SetKeuzevakSelectList(_keuzepakketRepository.FindAll());
            return View(model);
        }

        [Authorize(Role.Admin, Role.Begeleider, Role.Student, Role.Bedrijf)]
        public ActionResult Details(int? id)
        {
            var student = FindStudent(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            var model = Mapper.Map<StudentDetailsVM>(student);
            model.ToonEdit = CurrentUser.IsStudent() || CurrentUser.IsAdmin();
            model.ToonTerugNaarLijst = CurrentUser.IsAdmin() || CurrentUser.IsBegeleider();
            model.ToonDelete = CurrentUser.IsAdmin();
            return View(model);
        }

        [Authorize(Role.Student, Role.Admin)]
        [ActionName("Edit")]
        public ActionResult Edit(int? id)
        {
            var student = FindStudent(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            var model = Mapper.Map<Student, StudentEditVM>(student);
            model.SetKeuzevakSelectList(_keuzepakketRepository.FindAll());
            model.KeuzepakketId = student.Keuzepakket == null ? null : (int?)student.Keuzepakket.Id;
            model.ToonTerug = !CurrentUser.IsStudent();
            return View("Edit", model);
        }

        [Authorize(Role.Student, Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StudentEditVM model, HttpPostedFileBase fotoFile)
        {
            var student = FindStudent(model.Id);
            if (student == null)
            {
                return HttpNotFound();
            }
            if (_imageService.IsValidImage(fotoFile))
            {
                if (_imageService.HasValidSize(fotoFile))
                {
                    model.FotoUrl = _imageService.SaveImage(fotoFile, student.FotoUrl, "~/Images/Student");
                }
                else
                {
                    ModelState.AddModelError("", string.Format(Resources.ErrorOngeldigeAfbeeldingGrootte, (_imageService.MaxSize() / 1024)));
                    return View(model);
                }
            }
            var studentModel = Mapper.Map<StudentEditVM, Student>(model);
            studentModel.Id = student.Id;
            studentModel.Keuzepakket = model.KeuzepakketId == null ? null : _keuzepakketRepository.FindBy((int)model.KeuzepakketId);
            _studentRepository.Update(studentModel);
            SetViewMessage("Gegevens gewijzigd.");
            return RedirectToAction("Details", new { studentModel.Id, Overzicht });
        }

        [Authorize(Role.Admin)]
        public ActionResult Delete(int id)
        {
            var student = FindStudent(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        [Authorize(Role.Admin)]
        [ActionName("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string overzicht = "/Student/List")
        {
            var student = FindStudent(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            try
            {
                _studentRepository.Delete(student);
                SetViewMessage(string.Format(Resources.SuccesDeleteStudent, student.Naam));
            }
            catch (ApplicationException ex)
            {
                SetViewError(ex.Message);
                return View(student);
            }
            finally
            {
                _userService.DeleteLogin(student.HogentEmail);
            }
            return RedirectToLocal(overzicht);
        }

        public ActionResult ImportStudentenExcel()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportStudentenExcel(HttpPostedFileBase file)
        {
            try
            {
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(file.InputStream, false))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                    WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                    SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
                    string text;
                    foreach (Row r in sheetData.Elements<Row>())
                    {
                        foreach (Cell c in r.Elements<Cell>())
                        {
                            text = c.CellValue.Text;
                        }
                    }
                }
            }
            catch (FileFormatException ex)
            {
                HtmlDocument doc = new HtmlDocument();
                doc.Load(file.InputStream);
                HtmlNode table = doc.DocumentNode.SelectSingleNode("/table");
            }
            return View();
        }

        [Authorize(Role.Admin)]
        public ActionResult StudentJson(string email)
        {
            var student = _studentRepository.FindByEmail(email);
            if (student == null)
            {
                return HttpNotFound();
            }
            var model = Mapper.Map<StudentJsonVM>(student);
            model.Email = student.HogentEmail;
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #region Helpers
        private Student FindStudent(int? id)
        {
            if (CurrentUser.IsStudent())
            {
                return _userService.GetStudent();
            }
            if (id == null)
            {
                return null;
            }
            if (CurrentUser.IsBedrijf())
            {
                var bedrijf = _userService.GetBedrijf();
                return bedrijf.FindStudent((int)id);
            }
            return _studentRepository.FindById((int)id);//admin/begeleider
        }

        #endregion
    }


}