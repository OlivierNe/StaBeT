using AutoMapper;
using StageBeheersTool.Helpers;
using StageBeheersTool.Models.Authentication;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace StageBeheersTool.Controllers
{
    [Authorize(Roles = Role.Admin)]
    public class AcademiejaarController : Controller
    {
        private readonly IAcademiejaarRepository _academiejaarRepository;

        public AcademiejaarController(IAcademiejaarRepository academiejaarRepository)
        {
            _academiejaarRepository = academiejaarRepository;
        }

        // GET: Academiejaar
        public ActionResult Index()
        {
            var academiejaarInstellingen = _academiejaarRepository.FindByHuidigAcademiejaar();
            if (academiejaarInstellingen == null)
            {
                academiejaarInstellingen = new AcademiejaarInstellingen
                {
                    Academiejaar = AcademiejaarHelper.HuidigAcademiejaar()
                };
                _academiejaarRepository.Add(academiejaarInstellingen);
            }
            return View(Mapper.Map<AcademiejaarInstellingenVM>(academiejaarInstellingen));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(AcademiejaarInstellingenVM model)
        {
            if (ModelState.IsValid)
            {
                var academiejaarInstellingen = Mapper.Map<AcademiejaarInstellingen>(model);
                _academiejaarRepository.Update(academiejaarInstellingen);
                TempData["message"] = string.Format(Resources.SuccesParametersAcademiejaar, model.Academiejaar);
                return View("Index", model);
            }
            return View("Index", model);
        }

        public ActionResult Lijst()
        {
            return View(Mapper.Map<IEnumerable<AcademiejaarInstellingen>, IEnumerable<AcademiejaarInstellingenVM>>(_academiejaarRepository.FindAll()));
        }

        public ActionResult Create(string academiejaar = null)
        {
            AcademiejaarInstellingen academiejaarInstellingen;
            if (academiejaar != null)
            {
                academiejaarInstellingen = _academiejaarRepository.FindByAcademiejaar(academiejaar);
            }
            else
            {
                academiejaarInstellingen = new AcademiejaarInstellingen();
            }
            return View(Mapper.Map<AcademiejaarInstellingenVM>(academiejaarInstellingen));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AcademiejaarInstellingenVM model)
        {
            if (ModelState.IsValid)
            {
                _academiejaarRepository.Add(Mapper.Map<AcademiejaarInstellingen>(model));
                TempData["message"] = string.Format(Resources.SuccesParametersAcademiejaar, model.Academiejaar);
                return View(model);
            }
            return View(model);
        }

    }
}