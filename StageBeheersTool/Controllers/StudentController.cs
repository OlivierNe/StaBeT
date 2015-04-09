using StageBeheersTool.Models.DAL.Extensions;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.ViewModels;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using StageBeheersTool.Models.Authentication;

namespace StageBeheersTool.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IKeuzepakketRepository _keuzepakketRepository;
        private readonly IUserService _userService;
        private readonly IImageService _imageService;

        public StudentController(IStudentRepository studentRepository, IKeuzepakketRepository keuzepakketRepository,
            IUserService userService, IImageService imageService)
        {
            _studentRepository = studentRepository;
            _keuzepakketRepository = keuzepakketRepository;
            _imageService = imageService;
            _userService = userService;
        }

        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult Index(StudentListVM model)
        {
            var studenten = _studentRepository.FindAll().WithFilter(model.Naam, model.Voornaam);
            model.Studenten = studenten;

            model.ToonCreateNew = CurrentUser.IsAdmin();

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
            model.Overzicht = ControllerContext.RouteData.Values["action"].ToString();

            if (Request.IsAjaxRequest())
            {
                return PartialView("_studentList", model);
            }
            return View("Index", model);
        }

        [Authorize(Role.Admin)]
        public ActionResult Create()
        {
            var model = new StudentCreateVM();
            model.SetKeuzevakSelectList(_keuzepakketRepository.FindAll());
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
                var result = _studentRepository.Add(student);
                if (result == true)
                {
                    TempData["message"] = string.Format(Resources.SuccesStudentCreate, student.HogentEmail);
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = string.Format(Resources.ErrorStudentCreateHogentEmailBestaatAl, student.HogentEmail);
                }
            }
            model.SetKeuzevakSelectList(_keuzepakketRepository.FindAll());
            return View(model);
        }

        [Authorize(Role.Admin, Role.Begeleider, Role.Student)]
        public ActionResult Details(int? id, string overzicht = "Index")
        {
            var student = FindStudent(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            var model = Mapper.Map<StudentDetailsVM>(student);
            model.ToonEdit = CurrentUser.IsStudent() || CurrentUser.IsAdmin();
            model.ToonTerugNaarLijst = CurrentUser.IsAdmin() || CurrentUser.IsBegeleider();
            model.Overzicht = overzicht;
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
            TempData["message"] = "Gegevens gewijzigd.";
            return RedirectToAction("Details", new { studentModel.Id });
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
        public ActionResult DeleteConfirmed(int id)
        {
            var student = FindStudent(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            _studentRepository.Delete(student);
            TempData["message"] = "Student " + student.HogentEmail + " succesvol verwijderd.";
            return RedirectToAction("Index");
        }

        [Authorize(Role.Admin)]
        public ActionResult StudentJson(string hoGentEmail)
        {
            var student = _studentRepository.FindByEmail(hoGentEmail);
            var model = Mapper.Map<StudentJsonVM>(student);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #region helpers

        private Student FindStudent(int? id)
        {
            if (CurrentUser.IsStudent())
            {
                return _userService.FindStudent();
            }
            if (id == null)
            {
                return null;
            }
            return _studentRepository.FindById((int)id);//admin/begeleider
        }
        #endregion
    }
}