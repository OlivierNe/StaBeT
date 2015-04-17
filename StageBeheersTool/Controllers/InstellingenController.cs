﻿using AutoMapper;
using StageBeheersTool.Helpers;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.Models.Identity;
using StageBeheersTool.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace StageBeheersTool.Controllers
{
    [Authorize(Roles = Role.Admin)]
    public class InstellingenController : BaseController
    {
        private readonly IAcademiejaarRepository _academiejaarRepository;
        private readonly IInstellingenRepository _instellingenRepository;

        public InstellingenController(IAcademiejaarRepository academiejaarRepository,
            IInstellingenRepository instellingenRepository)
        {
            _academiejaarRepository = academiejaarRepository;
            _instellingenRepository = instellingenRepository;
        }

        #region academiejaar instellingen
        public ActionResult HuidigAcademiejaar()
        {
            var academiejaarInstellingen = _academiejaarRepository.FindVanHuidigAcademiejaar();
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
        public ActionResult HuidigAcademiejaar(AcademiejaarInstellingenVM model)
        {
            if (ModelState.IsValid)
            {
                var academiejaarInstellingen = Mapper.Map<AcademiejaarInstellingen>(model);
                _academiejaarRepository.Update(academiejaarInstellingen);
                SetViewMessage(string.Format(Resources.SuccesParametersAcademiejaar, model.Academiejaar));
            }
            return View(model);
        }

        public ActionResult AlleAcademiejaren()
        {
            return View(Mapper.Map<IEnumerable<AcademiejaarInstellingen>, IEnumerable<AcademiejaarInstellingenVM>>(_academiejaarRepository.FindAll()));
        }

        public ActionResult CreateAcadInstellingen(string academiejaar = null)
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
        public ActionResult CreateAcadInstellingen(AcademiejaarInstellingenVM model)
        {
            if (ModelState.IsValid)
            {
                _academiejaarRepository.Add(Mapper.Map<AcademiejaarInstellingen>(model));
                SetViewMessage(string.Format(Resources.SuccesParametersAcademiejaar, model.Academiejaar));
                return View(model);
            }
            return View(model);
        }
        #endregion

        #region algemene instellingen

        public ActionResult AlgemeneInstellingen()
        {
            var model = new InstellingenVM();
            var mailboxStages = _instellingenRepository.Find(Instelling.MailboxStages);
            if (mailboxStages != null)
            {
                model.MailboxStages = mailboxStages.Value;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AlgemeneInstellingen(InstellingenVM model)
        {
            if (ModelState.IsValid)
            {
                _instellingenRepository.AddOrUpdate(new Instelling(Instelling.MailboxStages, model.MailboxStages));
                SetViewMessage(Resources.SuccesEditSaved);
            }
            return View(model);
        }

        #endregion

    }
}