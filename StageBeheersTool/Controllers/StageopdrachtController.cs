using AutoMapper;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using StageBeheersTool.Models.Authentication;
using System.Threading.Tasks;


namespace StageBeheersTool.Controllers
{
    [Authorize]
    public class StageopdrachtController : Controller
    {
        private ISpecialisatieRepository specialisatieRepository;
        private IStageopdrachtRepository stageopdrachtRepository;
        private IUserService userService;

        public StageopdrachtController(IStageopdrachtRepository stageopdrachtRepository,
            ISpecialisatieRepository specialisatieRepository, IUserService userService)
        {
            this.stageopdrachtRepository = stageopdrachtRepository;
            this.specialisatieRepository = specialisatieRepository;
            this.userService = userService;
        }

        [Authorize(Roles = "bedrijf,student,begeleider,admin")]
        public ActionResult Index(StageopdrachtIndexVM model)
        {
            IEnumerable<Stageopdracht> stageopdrachten = null;
            if (userService.IsBedrijf())
            {
                stageopdrachten = userService.FindBedrijf().Stageopdrachten;
            }
            else if (userService.IsStudent())
            {
                stageopdrachten = stageopdrachtRepository.FindGeldigeStageopdrachten(model.Semester, model.AantalStudenten, model.SpecialisatieId,
                    model.Bedrijf, model.Locatie);
            }
            else
            {
                stageopdrachten = stageopdrachtRepository.FindByFilter(model.Semester, model.AantalStudenten, model.SpecialisatieId,
                    model.Bedrijf, model.Locatie).ToList();
            }
            model.setItems(stageopdrachten, specialisatieRepository.FindAll());
            model.ToonSearchForm = !userService.IsBedrijf();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View(model);
        }

        [Authorize(Roles = "begeleider")]
        public ActionResult GoedgekeurdeStages(StageopdrachtIndexVM model)
        {
            var stageopdrachten = stageopdrachtRepository.FindGoedgekeurdeStageopdrachtenByFilter(model.Semester, model.AantalStudenten,
                model.SpecialisatieId, model.Bedrijf, model.Locatie, model.Student).ToList();
            model.setItems(stageopdrachten, specialisatieRepository.FindAll());
            model.ToonSearchForm = true;
            model.ToonZoekenOpStudent = true;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("Index", model);
        }

        [Authorize(Roles = "admin")]
        [ActionName("Voorstellen")]
        public ActionResult VoorstellenVanBedrijven(StageopdrachtIndexVM model)
        {
            var stageopdrachten = stageopdrachtRepository.FindStageopdrachtVoorstellen().ToList();
            model.ToonOordelen = true;
            model.Stageopdrachten = stageopdrachten;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("Index", model);
        }

        [Authorize(Roles = "bedrijf")]
        public ActionResult Create(bool isStagementor = false, bool isContractOndertekenaar = false)
        {
            if (Request.IsAjaxRequest())
            {
                if (isStagementor)
                {
                    return PartialView("_CreateStagementorForm");
                }
                else if (isContractOndertekenaar)
                {
                    return PartialView("_CreateContractOndertekenaarForm");
                }
            }
            var bedrijf = userService.FindBedrijf();
            var constractondertekenaars = bedrijf.FindAllContractOndertekenaars();
            var stagementors = bedrijf.FindAllStagementors();
            var model = new StageopdrachtCreateVM(specialisatieRepository.FindAll(),
                bedrijf.FindAllContractOndertekenaars(), bedrijf.FindAllStagementors());
            model.setAdres(bedrijf.Gemeente, bedrijf.Postcode, bedrijf.Straat, bedrijf.Straatnummer);

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "bedrijf")]
        public ActionResult Create(StageopdrachtCreateVM model)
        {
            var bedrijf = userService.FindBedrijf();
            if (ModelState.IsValid)
            {
                var stageopdracht = Mapper.Map<Stageopdracht>(model);
                stageopdracht.Specialisatie = model.SpecialisatieId != null ? specialisatieRepository.FindBy((int)model.SpecialisatieId) : null;

                if (stageopdracht.Stagementor == null)
                {
                    stageopdracht.Stagementor = model.StagementorId != null ? bedrijf.FindContactpersoonById((int)model.StagementorId) : null;
                }

                if (model.StagementorId == -1)
                {
                    stageopdracht.Stagementor = stageopdracht.Contractondertekenaar;
                    bedrijf.AddContactpersoon(stageopdracht.Stagementor);
                }
                else if (model.StagementorId == null)
                {
                    bedrijf.AddContactpersoon(stageopdracht.Stagementor);
                }
                if (stageopdracht.Contractondertekenaar == null)
                {
                    stageopdracht.Contractondertekenaar = model.ContractondertekenaarId != null ? bedrijf.FindContactpersoonById((int)model.ContractondertekenaarId) : null;
                }
                if (model.ContractondertekenaarId == -1)
                {
                    stageopdracht.Contractondertekenaar = stageopdracht.Stagementor;
                    bedrijf.AddContactpersoon(stageopdracht.Contractondertekenaar);
                }
                else if (model.StagementorId == null)
                {
                    bedrijf.AddContactpersoon(stageopdracht.Contractondertekenaar);
                }

                bedrijf.AddStageopdracht(stageopdracht);
                userService.SaveChanges();
                TempData["message"] = "Stageopdracht succesvol aangemaakt.";
                return RedirectToAction("Details", new { id = stageopdracht.Id });
            }
            model.SetSelectLists(specialisatieRepository.FindAll(), bedrijf.FindAllContractOndertekenaars(),
                bedrijf.FindAllStagementors());
            return View(model);
        }

        [Authorize(Roles = "bedrijf,student,admin,begeleider")]
        public ActionResult Details(int id)
        {
            var model = new StageopdrachtDetailsVM();
            if (userService.IsBedrijf())
            {
                var bedrijf = userService.FindBedrijf();
                model.Stageopdracht = bedrijf.FindStageopdrachtById(id);
                model.ToonEdit = true;
            }
            else if (userService.IsStudent())
            {
                model.Stageopdracht = stageopdrachtRepository.FindGeldigeStageopdrachtById(id);
                var student = userService.FindStudent();
                model.ToonToevoegen = !student.HeeftStageopdrachtAlsVoorkeur(id);
                model.ToonVerwijderenBtn = !model.ToonToevoegen;
            }
            else if (userService.IsBegeleider())
            {
                var begeleider = userService.FindBegeleider();
                var stageopdracht = stageopdrachtRepository.FindById(id);
                model.Stageopdracht = stageopdracht;
                model.ToonAanvraagIndienen = model.Stageopdracht.IsGoedgekeurd()
                    && !model.Stageopdracht.HeeftStageBegeleider() && !begeleider.HeeftStageBegeleidingAangevraagd(id);
                model.ToonAanvraagAnnuleren = model.Stageopdracht.IsGoedgekeurd() && !stageopdracht.HeeftStageBegeleider() &&
                    begeleider.HeeftStageBegeleidingAangevraagd(stageopdracht.Id);
                if (stageopdracht.Stagebegeleider != null && begeleider.Id == stageopdracht.Stagebegeleider.Id)
                {
                    model.ToonEdit = true;
                }
            }
            else
            {
                model.Stageopdracht = stageopdrachtRepository.FindById(id);
            }

            if (model.Stageopdracht == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }


        [Authorize(Roles = "bedrijf,begeleider")]
        public ActionResult Edit(int id)
        {
            Stageopdracht stageopdracht = null;
            Bedrijf bedrijf = null;
            if (userService.IsBegeleider())
            {
                var begeleider = userService.FindBegeleider();
                stageopdracht = begeleider.FindStage(id);
                bedrijf = stageopdracht.Bedrijf;
            }
            else if (userService.IsBedrijf())
            {
                bedrijf = userService.FindBedrijf();
                stageopdracht = bedrijf.FindStageopdrachtById(id);
            }
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            var model = Mapper.Map<Stageopdracht, StageopdrachtEditVM>(stageopdracht);
            model.SpecialisatieId = stageopdracht.Specialisatie == null ? null : (int?)stageopdracht.Specialisatie.Id;
            model.ContractondertekenaarId = stageopdracht.Contractondertekenaar == null ? null : (int?)stageopdracht.Contractondertekenaar.Id;
            model.StagementorId = stageopdracht.Stagementor == null ? null : (int?)stageopdracht.Stagementor.Id;
            model.SetSelectLists(specialisatieRepository.FindAll(),
                bedrijf.FindAllContractOndertekenaars(),
                bedrijf.FindAllStagementors());
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "bedrijf, begeleider")]
        public ActionResult Edit(StageopdrachtEditVM model)
        {
            Bedrijf bedrijf = null;
            Stageopdracht stageopdracht = null;
            if (userService.IsBedrijf())
            {
                stageopdracht = stageopdrachtRepository.FindById(model.Id);
                bedrijf = userService.FindBedrijf();
            }
            else if (userService.IsBegeleider())
            {
                stageopdracht = stageopdrachtRepository.FindById(model.Id);
                bedrijf = stageopdracht.Bedrijf;
            }
            if (ModelState.IsValid)
            {
                var stageopdrachtModel = Mapper.Map<StageopdrachtEditVM, Stageopdracht>(model);
                stageopdrachtModel.Specialisatie = model.SpecialisatieId != null ? specialisatieRepository.FindBy((int)model.SpecialisatieId) : null;
                stageopdrachtModel.Stagementor = model.StagementorId != null ? bedrijf.FindContactpersoonById((int)model.StagementorId) : null;
                stageopdrachtModel.Contractondertekenaar = model.ContractondertekenaarId
                    != null ? bedrijf.FindContactpersoonById((int)model.ContractondertekenaarId) : null;
                stageopdrachtRepository.Update(stageopdracht, stageopdrachtModel);
                userService.SaveChanges();
                return RedirectToAction("Details", new { id = model.Id });
            }
            stageopdracht = Mapper.Map<StageopdrachtEditVM, Stageopdracht>(model);
            model.SetSelectLists(specialisatieRepository.FindAll(),
                bedrijf.FindAllContractOndertekenaars(),
                bedrijf.FindAllStagementors());
            return View(model);
        }

        [Authorize(Roles = "bedrijf")]
        public ActionResult Delete(int id)
        {
            var bedrijf = userService.FindBedrijf();
            var stageopdracht = bedrijf.FindStageopdrachtById(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            return View(stageopdracht);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "bedrijf")]
        public ActionResult DeleteConfirmed(int id)
        {
            var bedrijf = userService.FindBedrijf();
            var stageopdracht = bedrijf.FindStageopdrachtById(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            if (stageopdracht.IsGoedgekeurd())
            {
                TempData["message"] = "Goegekeurde stages kunnen niet meer verwijderd worden.";
                return RedirectToAction("Index");
            }
            stageopdrachtRepository.Delete(stageopdracht);
            userService.SaveChanges();
            TempData["message"] = "Stageopdracht '" + stageopdracht.Titel + "' succesvol verwijderd.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "student")]
        public ActionResult ToevoegenAanVoorkeur(int id)
        {
            var stageopdracht = stageopdrachtRepository.FindGeldigeStageopdrachtById(id);

            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            var student = userService.FindStudent();
            student.AddVoorkeurStage(stageopdracht);
            userService.SaveChanges();
            return RedirectToAction("MijnVoorkeurStages");
        }

        [Authorize(Roles = "student")]
        public ActionResult VerwijderenUitVoorkeur(int[] ids)
        {
            var stageopdracht = stageopdrachtRepository.FindById(ids[0]);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            var student = userService.FindStudent();
            student.RemoveVoorkeurStages(ids);
            userService.SaveChanges();
            return RedirectToAction("MijnVoorkeurStages");
        }

        [Authorize(Roles = "student")]
        public ActionResult MijnVoorkeurStages()
        {
            IEnumerable<Stageopdracht> stageopdrachten = null;
            var student = userService.FindStudent();
            stageopdrachten = student.VoorkeurStages;
            var model = new StageopdrachtIndexVM() { Stageopdrachten = stageopdrachten };
            if (Request.IsAjaxRequest())
            {
                return PartialView("_StageopdrachtList", model);
            }
            return View("Index", model);
        }

        [Authorize(Roles = "admin")]
        [ActionName("Goedkeuren")]
        public async Task<ActionResult> StageopdrachtGoedkeuren(int id)
        {
            var stageopdracht = stageopdrachtRepository.FindById(id);
            Admin.KeurStageopdrachtGoed(stageopdracht);
            var emailService = Request.GetOwinContext().GetUserManager<ApplicationUserManager>().EmailService;
            await emailService.SendAsync(new IdentityMessage()
            {
                Destination = stageopdracht.Bedrijf.Email,
                Subject = "Stageopdracht goedgekeurd.",
                Body = "Stageopdracht " + stageopdracht.Titel + " goedgekeurd."
            });
            stageopdrachtRepository.SaveChanges();
            return RedirectToAction("Voorstellen");
        }

        [Authorize(Roles = "admin")]
        [ActionName("Afkeuren")]
        public ActionResult StageopdrachtAfkeuren(int id)
        {
            var stageopdracht = stageopdrachtRepository.FindById(id);
            var model = Mapper.Map<StageopdrachtAfkeurenVM>(stageopdracht);
            model.Aan = stageopdracht.Bedrijf.Email;
            model.Onderwerp = "Stageopdracht afgekeurd";
            return View(model);
        }

        [HttpPost]
        [ActionName("Afkeuren")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StageopdrachtAfkeurenConfirmed(StageopdrachtAfkeurenVM model)
        {
            if (ModelState.IsValid)
            {
                var stageopdracht = stageopdrachtRepository.FindById(model.Id);
                Admin.KeurStageopdrachtAf(stageopdracht);
                var emailService = Request.GetOwinContext().GetUserManager<ApplicationUserManager>().EmailService;
                await emailService.SendAsync(new IdentityMessage()
                {
                    Destination = stageopdracht.Bedrijf.Email,
                    Subject = model.Onderwerp,
                    Body = model.Reden
                });
                stageopdrachtRepository.SaveChanges();
                return RedirectToAction("Voorstellen");
            }
            return View("Afkeuren", model);
        }

        [Authorize(Roles = "begeleider")]
        public ActionResult AanvraagIndienen(int id)
        {
            var stageopdracht = stageopdrachtRepository.FindById(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            var begeleider = userService.FindBegeleider();
            begeleider.AddAanvraag(stageopdracht);
            stageopdrachtRepository.SaveChanges();
            return RedirectToAction("Details", new { id = id });
        }

        [Authorize(Roles = "begeleider")]
        public ActionResult AanvraagAnnuleren(int id)
        {
            var stageopdracht = stageopdrachtRepository.FindById(id);
            if (stageopdracht == null)
            {
                return HttpNotFound();
            }
            var begeleider = userService.FindBegeleider();
            if (begeleider.HeeftStageBegeleidingAangevraagd(id) == false)
            {
                return new HttpStatusCodeResult(403);
            }
            stageopdrachtRepository.DeleteAanvraag(begeleider.FindAanvraag(stageopdracht));
            stageopdrachtRepository.SaveChanges();
            return RedirectToAction("Details", new { id = id });
        }

        [Authorize(Roles = "admin")]
        [ActionName("Aanvragen")]
        public ActionResult StageBegeleiderAanvragen()
        {
            var aanvragen = stageopdrachtRepository.FindAllAanvragen();
            return View(aanvragen);
        }

        [Authorize(Roles = "begeleider")]
        public ActionResult AanvragenBegeleider()
        {
            var begeleider = userService.FindBegeleider();
            var aanvragen = stageopdrachtRepository.FindAllAanvragenFrom(begeleider);
            return View("Aanvragen", aanvragen);
        }


        [Authorize(Roles = "admin")]
        public ActionResult AanvraagGoedkeuren(int id)
        {
            var aanvraag = stageopdrachtRepository.FindAanvraagById(id);
            if (aanvraag == null)
            {
                return HttpNotFound();
            }
            Admin.KeurStageBegeleidAanvraagGoed(aanvraag);
            stageopdrachtRepository.SaveChanges();
            return RedirectToAction("Aanvragen");
        }

        [Authorize(Roles = "admin")]
        public ActionResult AanvraagAfkeuren(int id)
        {
            var aanvraag = stageopdrachtRepository.FindAanvraagById(id);
            if (aanvraag == null)
            {
                return HttpNotFound();
            }
            Admin.KeurStageBegeleidAanvraagAf(aanvraag);
            stageopdrachtRepository.SaveChanges();
            return RedirectToAction("Aanvragen");
        }



    }


}