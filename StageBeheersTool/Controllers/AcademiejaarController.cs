using AutoMapper;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StageBeheersTool.Controllers
{
    [Authorize(Roles = "admin")]
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
                academiejaarInstellingen = new AcademiejaarInstellingen()
                {
                    Academiejaar = Helpers.HuidigAcademiejaar()
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
                ViewBag.message = "Wijzigingen opgeslagen";
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
            if (academiejaar != null)
            {
                _academiejaarRepository.FindByAcademiejaar(academiejaar);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AcademiejaarInstellingenVM model)
        {
            if (ModelState.IsValid)
            {
                _academiejaarRepository.Add(Mapper.Map<AcademiejaarInstellingen>(model));
                ViewBag.success = "instellingen opgeslagen";
                return View(model);
            }
            return View(model);
        }

    }
}