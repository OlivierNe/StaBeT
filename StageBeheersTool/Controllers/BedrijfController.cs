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
    [Authorize(Roles = "bedrijf")]
    public class BedrijfController : Controller
    {
        private IBedrijfRepository bedrijfRepository;

        public BedrijfController(IBedrijfRepository bedrijfRepository)
        {
            this.bedrijfRepository = bedrijfRepository;
        }

        [Authorize(Roles = "bedrijf")]
        public ActionResult Details()
        {
            var bedrijf = FindBedrijf();
            return View(bedrijf);
        }

        [Authorize(Roles = "bedrijf")]
        public ActionResult Edit()
        {
            var bedrijf = FindBedrijf();
            return View(Mapper.Map<EditBedrijfVM>(bedrijf));
        }

        [Authorize(Roles = "bedrijf")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditBedrijfVM model)
        {
            var bedrijf = FindBedrijf();
            var newBedrijf = Mapper.Map<Bedrijf>(model);
            bedrijfRepository.Update(bedrijf, newBedrijf);
            bedrijfRepository.SaveChanges();
            TempData["message"] = "Gegevens gewijzigd.";
            return RedirectToAction("Details");
        }

        #region Helpers
        private Bedrijf FindBedrijf()
        {
            return bedrijfRepository.FindByEmail(User.Identity.Name);
        }
        #endregion
    }
}