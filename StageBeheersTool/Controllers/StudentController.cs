﻿using System;
using System.IO;
using System.Linq;
using Ionic.Zip;
using StageBeheersTool.Helpers;
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
            model.Titel = "Studenten met stage " + AcademiejaarHelper.HuidigAcademiejaar();
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
                        _userService.CreateLogin(student.HogentEmail, Role.Student);
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
        public ActionResult Edit(StudentEditVM model)
        {
            var student = FindStudent(model.Id);
            if (student == null)
            {
                return HttpNotFound();
            }
            var studentModel = Mapper.Map<StudentEditVM, Student>(model);
            studentModel.Id = student.Id;
            try
            {
                var foto = _imageService.GetFoto(model.FotoFile, student.Naam);
                studentModel.Foto = foto;
            }
            catch (ApplicationException ex)
            {
                SetViewError(ex.Message);
            }
            studentModel.Keuzepakket = model.KeuzepakketId == null ? null : _keuzepakketRepository.FindBy((int)model.KeuzepakketId);
            _studentRepository.Update(studentModel);
            SetViewMessage("Wijzigingen opgeslagen");
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

        [Authorize(Role.Admin)]
        public ActionResult ImportStudentenExcel()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Role.Admin)]
        public ActionResult ImportStudentenExcel(HttpPostedFileBase file, HttpPostedFileBase zipFile)
        {
            if (file == null && zipFile == null)
            {
                SetViewError("Geen bestand geselecteerd.");
                return View();
            }
            var message = "";
            if (file != null)
            {
                try
                {
                    var studenten = _spreadsheetService.ImportStudenten(file.InputStream);
                    _studentRepository.AddAll(studenten);
                    var hogentEmails = studenten.Select(s => s.HogentEmail).ToList();
                    _userService.CreateLogins(hogentEmails, Role.Student);
                    message += "Studenten succesvol geïmporteerd. ";
                }
                catch (FileFormatException)
                {
                    SetViewError("Ongeldig excel bestand formaat.");
                    return View();
                }
            }
            if (zipFile != null)
            {
                using (ZipFile fotos = ZipFile.Read(zipFile.InputStream))
                {
                    foreach (ZipEntry fotoFile in fotos)
                    {
                        using (MemoryStream fotoStream = new MemoryStream())
                        {
                            var studentNaam = fotoFile.FileName.Substring(0, fotoFile.FileName.LastIndexOf('_'));
                            var idx = studentNaam.LastIndexOf('_');
                            var voornaam = studentNaam.Substring(idx).Trim('_', ' ');
                            var familienaam = studentNaam.Substring(0, idx).Trim('_', ' ').Replace('_', ' ');
                            var student = _studentRepository.FindByNaam(voornaam, familienaam);
                            if (student == null)
                                continue;
                            fotoFile.Extract(fotoStream);
                            if (student.Foto == null)
                            {
                                student.Foto = new Foto
                                {
                                    ContentType = "image/jpeg",
                                    FotoData = fotoStream.ToArray(),
                                    Naam = student.Naam
                                };
                            }
                            else
                            {
                                student.Foto.ContentType = "image/jpeg";
                                student.Foto.FotoData = fotoStream.ToArray();
                                student.Foto.Naam = student.Naam;
                            }
                            _studentRepository.Update(student);
                        }
                    }
                }
                message += "Student Foto's toegevoegd";
            }
            SetViewMessage(message);
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