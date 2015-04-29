using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.Models.Identity;
using StageBeheersTool.ViewModels;

namespace StageBeheersTool.Controllers
{
    public class EvaluatieformulierController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IEvaluatievraagRepository _evaluatievraagRepository;

        public EvaluatieformulierController(IUserService userService, IEvaluatievraagRepository evaluatievraagRepository)
        {
            _userService = userService;
            _evaluatievraagRepository = evaluatievraagRepository;
        }

        [Authorize(Role.Bedrijf)]
        public ActionResult Index(int stageId)
        {
            return View(stageId);
        }

        [Authorize(Role.Bedrijf)]
        public ActionResult Stagebezoek(int stageId, int stagebezoek)
        {
            var bedrijf = _userService.GetBedrijf();
            var stage = bedrijf.FindStage(stageId);
            if (bedrijf.HeeftStage(stageId) == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            var evaluatievragen = _evaluatievraagRepository.FindByStagebezoek(stagebezoek)
                .Where(vraag => vraag.Voor == "Stagementor").ToList();
            var evaluatieantwoorden = stage.EvaluatieAntwoorden.ToList();
            var model = new EvaluatieCreateVM(evaluatievragen, evaluatieantwoorden, stagebezoek, stageId);
            return View(model);
        }

        [Authorize(Role.Bedrijf)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Stagebezoek(EvaluatieCreateVM model)
        {
            var keys = Request.Form.AllKeys;
            var evaluatievragen = _evaluatievraagRepository.FindByStagebezoek(model.Stagebezoek)
               .Where(vraag => vraag.Voor == "Stagementor").ToList();
            var bedrijf = _userService.GetBedrijf();
            var stage = bedrijf.FindStage(model.StageId);
            if (stage == null)
            {
                return HttpNotFound();
            }
            foreach (var key in keys)
            {
                int id;
                var result = int.TryParse(key, out id);
                if (result)
                {
                    var evaluatievraag = evaluatievragen.FirstOrDefault(vraag => vraag.Id == id);
                    if (evaluatievraag != null)
                    {
                        var antwoord = Request.Form[key];
                        stage.AddEvaluatieAntwoord(evaluatievraag, antwoord);
                    }
                    _userService.SaveChanges();
                }
            }
            SetViewMessage(Resources.SuccesSaveEvaluatieformulier);
            return RedirectToAction("MijnToegewezenStages", "Stage");
        }

    }
}