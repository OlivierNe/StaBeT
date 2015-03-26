using AutoMapper;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.ViewModels;
using System.Web;
using System.Web.Mvc;
using StageBeheersTool.Models.Authentication;

namespace StageBeheersTool.Controllers
{
    public class BegeleiderController : Controller
    {
        private readonly IBegeleiderRepository _begeleiderRepository;
        private readonly IImageService _imageService;
        private readonly IUserService _userService;

        public BegeleiderController(IBegeleiderRepository begeleiderRepository, IImageService imageService,
            IUserService userservice)
        {
            _begeleiderRepository = begeleiderRepository;
            _imageService = imageService;
            _userService = userservice;
        }

        [Authorize(Role.Begeleider, Role.Admin)]
        public ActionResult Details(int? id)
        {
            Begeleider begeleider;
            if (id == null)
            {
                begeleider = _userService.FindBegeleider();
            }
            else
            {
                begeleider = _begeleiderRepository.FindById((int)id);
            }
            if (begeleider == null)
            {
                return HttpNotFound();
            }
            var model = Mapper.Map<BegeleiderDetailsVM>(begeleider);
            model.ToonEdit = begeleider.Equals(_userService.FindBegeleider());
            return View(model);
        }

        [Authorize(Role.Begeleider)]
        public ActionResult Edit()
        {
            var begeleider = _userService.FindBegeleider();
            var model = Mapper.Map<BegeleiderEditVM>(begeleider);
            return View(model);
        }

        [Authorize(Role.Begeleider)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BegeleiderEditVM model, HttpPostedFileBase fotoFile)
        {
            var begeleider = _userService.FindBegeleider();
            if (_imageService.IsValidImage(fotoFile))
            {
                if (_imageService.HasValidSize(fotoFile))
                {
                    model.FotoUrl = _imageService.SaveImage(fotoFile, begeleider.FotoUrl, "~/Images/Begeleider");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Ongeldige afbeelding grootte, max. " + (_imageService.MaxSize() / 1024) + " Kb.");
                    return View(model);
                }
            }
            var begeleiderModel = Mapper.Map<Begeleider>(model);
            _begeleiderRepository.Update(begeleiderModel);
            TempData["message"] = "Gegevens gewijzigd.";
            return RedirectToAction("Details");
        }

    }
}