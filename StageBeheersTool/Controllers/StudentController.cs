using StageBeheersTool.Models.Domain;
using StageBeheersTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using AutoMapper;
using System.Net;
using System.IO;

namespace StageBeheersTool.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {

        private IStudentRepository studentRepository;
        private IKeuzepakketRepository keuzepakketRepository;

        public StudentController(IStudentRepository studentRepository, IKeuzepakketRepository keuzepakketRepository)
        {
            this.studentRepository = studentRepository;
            this.keuzepakketRepository = keuzepakketRepository;
        }

        [Authorize(Roles = "student")]
        public ActionResult Details()
        {
            var student = studentRepository.FindByEmail(User.Identity.Name);
            return View(student);
        }

        [Authorize(Roles = "student")]
        public ActionResult Edit(int? id)
        {
            Student student = null;
            if (User.IsInRole("student") || id == null)
            {
                student = studentRepository.FindByEmail(User.Identity.Name);
            }
            else
            {
                student = studentRepository.FindById((int)id);
            }
            var model = Mapper.Map<Student, StudentEditVM>(student);
            model.InitSelectList(keuzepakketRepository.FindAll());
            model.KeuzepakketId = student.Keuzepakket == null ? null : (int?)student.Keuzepakket.Id;
            return View(model);
        }

        [Authorize(Roles = "student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StudentEditVM model, HttpPostedFileBase fotoFile)
        {
            Student student = null;
            if (User.IsInRole("student"))
            {
                student = studentRepository.FindByEmail(User.Identity.Name);
                if (student.Id != model.Id)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }
            else
            {
                student = studentRepository.FindById(model.Id);
            }
            if (fotoFile != null && fotoFile.ContentLength > 0 && fotoFile.ContentType.StartsWith("image/"))
            {
                if (fotoFile.ContentLength > 512000)
                {
                    ModelState.AddModelError(string.Empty, "Ongeldige afbeelding grootte, max. 500kb.");
                    return View(model);
                }
                else
                {
                    string oldFotoUrl = student.FotoUrl;
                    if (System.IO.File.Exists(oldFotoUrl))
                    {
                        System.IO.File.Delete(oldFotoUrl);
                    }
                    string filename = User.Identity.GetUserId() + Path.GetExtension(fotoFile.FileName);
                    string relativePath = "~/Images/Student/" + filename;
                    model.FotoUrl = relativePath;
                    string absolutePath = Path.Combine(Server.MapPath("~/Images/Student"), Path.GetFileName(filename));
                    fotoFile.SaveAs(absolutePath);
                }
            }
            var newStudent = Mapper.Map<StudentEditVM, Student>(model);
            newStudent.Keuzepakket = model.KeuzepakketId == null ? null : keuzepakketRepository.FindBy((int)model.KeuzepakketId);
            studentRepository.Update(student, newStudent);
            studentRepository.SaveChanges();

            TempData["message"] = "Gegevens gewijzigd.";
            return RedirectToAction("Details");
        }


    }
}