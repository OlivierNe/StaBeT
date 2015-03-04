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
        private IUserService userService;

        public BedrijfController(IBedrijfRepository bedrijfRepository,
            IUserService userService)
        {
            this.bedrijfRepository = bedrijfRepository;
            this.userService = userService;
        }

        [Authorize(Roles = "bedrijf")]
        public ActionResult Details()
        {
            var bedrijf = userService.FindBedrijf();
            return View(bedrijf);
        }

        [Authorize(Roles = "bedrijf")]
        public ActionResult Edit()
        {
            var bedrijf = userService.FindBedrijf();
            return View(Mapper.Map<EditBedrijfVM>(bedrijf));
        }

        [Authorize(Roles = "bedrijf")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditBedrijfVM model)
        {
            var bedrijf = userService.FindBedrijf();
            var newBedrijf = Mapper.Map<Bedrijf>(model);
            bedrijfRepository.Update(bedrijf, newBedrijf);
            bedrijfRepository.SaveChanges();
            TempData["message"] = "Gegevens gewijzigd.";
            return RedirectToAction("Details");
        }
    }
}