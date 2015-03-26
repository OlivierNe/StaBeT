using StageBeheersTool.Models.Domain;
using StageBeheersTool.ViewModels;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using PagedList;
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
        public ActionResult Index(int page = 1)
        {
            var studenten = _studentRepository.FindAll();
            return View(studenten.ToPagedList(page, 10));
        }

        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult LijstStudentenMetGoedgekeurdeStageopdrachtEnBegeleider()
        {
            var studenten = _studentRepository.FindStudentenMetStageopdrachtEnBegeleider();
            return View(studenten);
        }

        [Authorize(Role.Admin, Role.Begeleider, Role.student)]
        public ActionResult Details(int? id)
        {
            Student student = null;
            if (_userService.IsStudent())
            {
                student = _userService.FindStudent();
            }
            else if (id == null)
            {
                return HttpNotFound();
            }
            else
            {
                student = _studentRepository.FindById((int)id);
            }
            var model = Mapper.Map<StudentDetailsVM>(student);
            model.ToonEdit = _userService.IsStudent();
            return View(model);

        }

        [Authorize(Role.student)]
        [ActionName("Edit")]
        public ActionResult Edit()
        {
            var student = _userService.FindStudent();
            var model = Mapper.Map<Student, StudentEditVM>(student);
            model.InitSelectList(_keuzepakketRepository.FindAll());
            model.KeuzepakketId = student.Keuzepakket == null ? null : (int?)student.Keuzepakket.Id;
            return View("Edit", model);
        }

        [Authorize(Role.student)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StudentEditVM model, HttpPostedFileBase fotoFile)
        {
            var student = _userService.FindStudent();
            if (_imageService.IsValidImage(fotoFile))
            {
                if (_imageService.HasValidSize(fotoFile))
                {
                    model.FotoUrl = _imageService.SaveImage(fotoFile, student.FotoUrl, "~/Images/Student");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Ongeldige afbeelding grootte, max. " + (_imageService.MaxSize() / 1024) + " Kb.");
                    return View(model);
                }
            }
            var studentModel = Mapper.Map<StudentEditVM, Student>(model);
            studentModel.Keuzepakket = model.KeuzepakketId == null ? null : _keuzepakketRepository.FindBy((int)model.KeuzepakketId);
            _studentRepository.Update(student, studentModel);

            TempData["message"] = "Gegevens gewijzigd.";
            return RedirectToAction("Details");
        }


    }
}