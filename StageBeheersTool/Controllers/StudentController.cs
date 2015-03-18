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
using StageBeheersTool.Models.Services;
using PagedList;
using StageBeheersTool.Models.Authentication;

namespace StageBeheersTool.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {

        private IStudentRepository studentRepository;
        private IKeuzepakketRepository keuzepakketRepository;
        private IUserService userService;
        private IImageService imageService;

        public StudentController(IStudentRepository studentRepository, IKeuzepakketRepository keuzepakketRepository,
            IUserService userService, IImageService imageService)
        {
            this.studentRepository = studentRepository;
            this.keuzepakketRepository = keuzepakketRepository;
            this.imageService = imageService;
            this.userService = userService;
        }

        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult Index(int page = 1)
        {
            var studenten = studentRepository.FindAll();
            return View(studenten.ToPagedList(page, 10));
        }

       [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult LijstStudentenMetGoedgekeurdeStageopdrachtEnBegeleider(){
            var studenten = studentRepository.FindStudentenMetStageopdrachtEnBegeleider();
            return View(studenten);
        }

        [Authorize(Role.Admin, Role.Begeleider, Role.student)]
        public ActionResult Details(int? id)
        {
            Student student = null;
            if (userService.IsStudent())
            {
                student = studentRepository.FindByEmail(User.Identity.Name);
                return View(student);
            }
            if (id == null)
            {
                return HttpNotFound();
            }
            student = studentRepository.FindById((int)id);
            return View(student);
        }

        [Authorize(Role.student)]
        [ActionName("Edit")]
        public ActionResult Edit()
        {
            var student = userService.FindStudent();
            var model = Mapper.Map<Student, StudentEditVM>(student);
            model.InitSelectList(keuzepakketRepository.FindAll());
            model.KeuzepakketId = student.Keuzepakket == null ? null : (int?)student.Keuzepakket.Id;
            return View("Edit", model);
        }

        [Authorize(Role.student)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StudentEditVM model, HttpPostedFileBase fotoFile)
        {
            var student = userService.FindStudent();
            if (imageService.IsValidImage(fotoFile))
            {
                if (imageService.HasValidSize(fotoFile))
                {
                    model.FotoUrl = imageService.SaveImage(fotoFile, student.FotoUrl, "~/Images/Student");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Ongeldige afbeelding grootte, max. " + (imageService.MaxSize() / 1024) + " Kb.");
                    return View(model);
                }
            }
            var studentModel = Mapper.Map<StudentEditVM, Student>(model);
            studentModel.Keuzepakket = model.KeuzepakketId == null ? null : keuzepakketRepository.FindBy((int)model.KeuzepakketId);
            studentRepository.Update(student, studentModel);

            TempData["message"] = "Gegevens gewijzigd.";
            return RedirectToAction("Details");
        }


    }
}