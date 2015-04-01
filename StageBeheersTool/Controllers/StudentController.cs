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
        public ActionResult Index()
        {
            var studenten = _studentRepository.FindAll();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_studentList", studenten);
            }
            return View(studenten);
        }

        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult LijstStudentenMetGoedgekeurdeStageopdrachtEnBegeleider()
        {
            var studenten = _studentRepository.FindStudentenMetStageopdrachtEnBegeleider();
            return View(studenten);
        }

        [Authorize(Role.Admin, Role.Begeleider, Role.Student)]
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
            model.InitSelectList(_keuzepakketRepository.FindAll());
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