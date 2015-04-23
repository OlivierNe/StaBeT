using System;
using System.Collections.Generic;
using System.Web.Mvc;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.Models.Identity;
using StageBeheersTool.ViewModels;

namespace StageBeheersTool.Controllers
{
    public class ActiviteitsverslagController : BaseController
    {
        private readonly IUserService _userService;

        public ActiviteitsverslagController(IUserService userService)
        {
            _userService = userService;
        }

        #region student
        [Authorize(Role.Student)]
        public ActionResult MijnActiviteitsverslagen()
        {
            var student = _userService.GetStudent();
            var stage = student.Stage;
            if (stage == null)
            {
                return new HttpNotFoundResult();
            }
            if (stage.Activiteitsverslagen == null || stage.Activiteitsverslagen.Count < 14)
            {
                stage.InitializeActiviteitsverslagen();
                _userService.SaveChanges();
            }
            return View(stage);
        }

        [Authorize(Role.Student)]
        public ActionResult Indienen(int week)
        {
            var student = _userService.GetStudent();
            var stage = student.Stage;
            if (stage == null)
            {
                return new HttpNotFoundResult();
            }
            var activiteitsverslag = stage.GetActiviteitsverslagVanWeek(week);
            var model = new ActiviteitsverslagIndienenVM { Week = week, Verslag = activiteitsverslag.Verslag };
            return View(model);
        }

        [Authorize(Role.Student)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Indienen(ActiviteitsverslagIndienenVM model)
        {
            if (ModelState.IsValid)
            {
                var student = _userService.GetStudent();
                var stage = student.Stage;
                if (stage == null)
                {
                    return new HttpNotFoundResult();
                }
                var activiteitsverslag = stage.GetActiviteitsverslagVanWeek(model.Week);
                activiteitsverslag.Verslag = model.Verslag;
                activiteitsverslag.DatumIngave = DateTime.Now;
                _userService.SaveChanges();
                SetViewMessage(String.Format(Resources.SuccesIndienenActiviteitsverslag, model.Week));
                return RedirectToAction("MijnActiviteitsverslagen");
            }
            return View(model);
        }


        [Authorize(Role.Student)]
        public ActionResult BekijkFeedback(int week)
        {
            var student = _userService.GetStudent();
            var stage = student.Stage;
            var verslag = stage.GetActiviteitsverslagVanWeek(week);
            return View(verslag);
        }

        #endregion

        #region begeleider

        [Authorize(Role.Begeleider)]
        public ActionResult VanStudent(int id)
        {
            var begeleider = _userService.GetBegeleider();
            var stage = begeleider.FindStage(id);
            return View(stage);
        }

        [Authorize(Role.Begeleider)]
        public ActionResult GeefFeedback(int id, int week)
        {
            var begeleider = _userService.GetBegeleider();
            var stage = begeleider.FindStage(id);
            if (stage == null)
            {
                return HttpNotFound();
            }
            var activieitsverslag = stage.GetActiviteitsverslagVanWeek(week);
            var model = new ActiviteitsverslagFeedbackVM
            {
                StageId = id,
                Week = week,
                Activiteitsverslag = activieitsverslag,
                Student = stage.Student.Naam,
                Feedback = activieitsverslag.Feedback
            };
            return View(model);
        }

        [Authorize(Role.Begeleider)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GeefFeedback(ActiviteitsverslagFeedbackVM model)
        {
            var begeleider = _userService.GetBegeleider();
            var stage = begeleider.FindStage(model.StageId);
            if (stage == null)
            {
                return HttpNotFound();
            }
            var activieitsverslag = stage.GetActiviteitsverslagVanWeek(model.Week);
            if (ModelState.IsValid)
            {
                activieitsverslag.Feedback = model.Feedback;
                _userService.SaveChanges();
                SetViewMessage("Feedback voor " + stage.Student.Naam + " week " + model.Week + " opgeslagen.");
                return RedirectToAction("VanStudent", new { id = model.StageId });
            }
            model.Activiteitsverslag = activieitsverslag;
            model.Student = stage.Student.Naam;
            return View(model);
        }


        #endregion

    }
}