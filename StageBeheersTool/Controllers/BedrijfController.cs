using AutoMapper;
using StageBeheersTool.Models.Authentication;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StageBeheersTool.Controllers
{
    public class BedrijfController : Controller
    {
        private readonly IBedrijfRepository _bedrijfRepository;
        private readonly IUserService _userService;

        public BedrijfController(IBedrijfRepository bedrijfRepository,
            IUserService userService)
        {
            _bedrijfRepository = bedrijfRepository;
            _userService = userService;
        }

        [Authorize(Role.Admin,Role.Begeleider)]
        public ActionResult Index()
        {
            var bedrijven = _bedrijfRepository.FindAll();

            return View(bedrijven);
        }

        [Authorize(Role.Bedrijf)]
        public ActionResult Details()
        {
            var bedrijf = _userService.FindBedrijf();
            return View(bedrijf);
        }

        [Authorize(Role.Bedrijf)]
        public ActionResult Edit()
        {
            var bedrijf = _userService.FindBedrijf();
            return View(Mapper.Map<EditBedrijfVM>(bedrijf));
        }

        [Authorize(Role.Bedrijf)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditBedrijfVM model)
        {
            var bedrijf = _userService.FindBedrijf();
            var bedrijfModel = Mapper.Map<Bedrijf>(model);
            bedrijfModel.Id = bedrijf.Id;
            _bedrijfRepository.Update(bedrijfModel);
            TempData["message"] = "Gegevens gewijzigd.";
            return RedirectToAction("Details");
        }
    }
}