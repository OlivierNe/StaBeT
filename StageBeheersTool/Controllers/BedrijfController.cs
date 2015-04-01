using AutoMapper;
using StageBeheersTool.Models.Authentication;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.ViewModels;
using System.Web.Mvc;

namespace StageBeheersTool.Controllers
{
    public class BedrijfController : Controller
    {
        private readonly IBedrijfRepository _bedrijfRepository;

        public BedrijfController(IBedrijfRepository bedrijfRepository)
        {
            _bedrijfRepository = bedrijfRepository;
        }

        [Authorize(Role.Admin, Role.Begeleider)]
        public ActionResult Index()
        {
            var bedrijven = _bedrijfRepository.FindAll();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_BedrijfList", bedrijven);
            }
            return View(bedrijven);
        }

        [Authorize(Role.Admin)]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Role.Admin)]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(RegisterBedrijfViewModel model)
        {
            if (ModelState.IsValid)
            {
                var bedrijf = Mapper.Map<Bedrijf>(model);
                _bedrijfRepository.Add(bedrijf);
                return RedirectToAction("Details", new{id=bedrijf.Id});
            }
            return View(model);
        }

        [Authorize(Role.Bedrijf, Role.Admin, Role.Begeleider)]
        public ActionResult Details(int? id)
        {
            var bedrijf = _bedrijfRepository.FindById(id);
            if (bedrijf == null)
            {
                return HttpNotFound();
            }
            return View(bedrijf);
        }

        [Authorize(Role.Bedrijf, Role.Admin)]
        public ActionResult Edit(int? id)
        {
            var bedrijf = _bedrijfRepository.FindById(id);
            if (bedrijf == null)
            {
                return HttpNotFound();
            }
            return View(Mapper.Map<EditBedrijfVM>(bedrijf));
        }

        [Authorize(Role.Bedrijf, Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditBedrijfVM model)
        {
            var bedrijf = Mapper.Map<Bedrijf>(model);
            _bedrijfRepository.Update(bedrijf);
            TempData["message"] = "Gegevens gewijzigd.";
            return RedirectToAction("Details", new { id = bedrijf.Id });
        }

        [Authorize(Role.Admin)]
        public ActionResult BedrijfJson(int id)
        {
            var bedrijf = _bedrijfRepository.FindById(id);
            var model = Mapper.Map<BedrijfInfoVM>(bedrijf);
            model.Stagementors = bedrijf.FindAllStagementors();
            model.Contractondertekenaars = bedrijf.FindAllContractOndertekenaars();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

    }
}