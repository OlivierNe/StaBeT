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
        private IAcademiejaarRepository academiejaarRepository;

        public AcademiejaarController(IAcademiejaarRepository academiejaarRepository)
        {
            this.academiejaarRepository = academiejaarRepository;
        }

        // GET: Academiejaar
        public ActionResult Index()
        {
            var academiejaarInstellingen = academiejaarRepository.FindByHuidigAcademiejaar();
            if (academiejaarInstellingen == null)
            {
                academiejaarInstellingen = new AcademiejaarInstellingen()
                {
                    Academiejaar = Helpers.HuidigAcademiejaar()
                };
                academiejaarRepository.Add(academiejaarInstellingen);
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
                academiejaarRepository.Update(academiejaarInstellingen);
                return RedirectToAction("Index");
            }
            return View("Index", model);
        }

        public ActionResult Lijst()
        {
            return View(Mapper.Map<IEnumerable<AcademiejaarInstellingen>, IEnumerable<AcademiejaarInstellingenVM>>(academiejaarRepository.FindAll()));
        }

        public ActionResult Create(string academiejaar = null)
        {
            if (academiejaar != null)
            {
                academiejaarRepository.FindByAcademiejaar(academiejaar);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AcademiejaarInstellingenVM model)
        {
            if (ModelState.IsValid)
            {
                academiejaarRepository.Add(Mapper.Map<AcademiejaarInstellingen>(model));
                ViewBag.success = "instellingen opgeslagen";
                return View(model);
            }
            return View(model);
        }

    }
}