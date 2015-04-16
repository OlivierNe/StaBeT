using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OudeGegevens;
using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using StageBeheersTool.Models.Authentication;
using StageBeheersTool.OudeGegevens;


namespace StageBeheersTool.Models.DAL
{
    public class StageToolDbInitializer :
        //DropCreateDatabaseAlways<StageToolDbContext>
   DropCreateDatabaseIfModelChanges<StageToolDbContext>
    {

        public void RunSeed(StageToolDbContext ctx)
        {
            //this.Seed(ctx);
        }

        protected override void Seed(StageToolDbContext context)
        {
            base.Seed(context);
            try
            {
                #region logins/roles
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                roleManager.Create(new IdentityRole("student"));
                roleManager.Create(new IdentityRole("admin"));
                roleManager.Create(new IdentityRole("begeleider"));
                roleManager.Create(new IdentityRole("bedrijf"));
                roleManager.Create(new IdentityRole("adminDisabled"));

                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

                ApplicationUser user = new ApplicationUser() { Email = "bedrijf@test.be", UserName = "bedrijf@test.be", EmailConfirmed = true };
                userManager.Create(user, "wachtwoord");
                userManager.AddToRole(user.Id, "bedrijf");

                ApplicationUser user2 = new ApplicationUser() { Email = "test@bedrijf2.be", UserName = "test@bedrijf2.be", EmailConfirmed = true };
                userManager.Create(user2, "wachtwoord");
                userManager.AddToRole(user2.Id, "bedrijf");

                ApplicationUser user3 = new ApplicationUser()
                {
                    Email = "student@test.be",
                    UserName = "student@test.be",
                    EmailConfirmed = true
                };
                userManager.Create(user3, "wachtwoord");
                userManager.AddToRole(user3.Id, "student");

                ApplicationUser user4 = new ApplicationUser()
                {
                    Email = "begeleider@test.be",
                    UserName = "begeleider@test.be",
                    EmailConfirmed = true
                };
                userManager.Create(user4, "wachtwoord");
                userManager.AddToRole(user4.Id, "begeleider");

                ApplicationUser user5 = new ApplicationUser()
                {
                    Email = "admin@test.be",
                    UserName = "admin@test.be",
                    EmailConfirmed = true
                };
                userManager.Create(user5, "wachtwoord");
                userManager.AddToRole(user5.Id, "admin");

                ApplicationUser user6 = new ApplicationUser()
                {
                    Email = "adminBegeleider@test.be",
                    UserName = "adminBegeleider@test.be",
                    EmailConfirmed = true
                };
                userManager.Create(user6, "wachtwoord");
                userManager.AddToRole(user6.Id, "admin");
                userManager.AddToRole(user6.Id, "begeleider");

                #endregion

                #region specialisaties
                Specialisatie specialisatie1 = new Specialisatie() { Naam = "Netwerken" };
                Specialisatie specialisatie2 = new Specialisatie() { Naam = "Programmeren" };
                Specialisatie specialisatie3 = new Specialisatie() { Naam = "Mobile" };
                Specialisatie specialisatie4 = new Specialisatie() { Naam = "Onderzoek" };
                Specialisatie specialisatie5 = new Specialisatie() { Naam = "Mainframe" };
                Specialisatie specialisatie6 = new Specialisatie() { Naam = "E-business" };
                Specialisatie specialisatie7 = new Specialisatie() { Naam = "Systeembeheer" };
                Specialisatie andere = new Specialisatie() { Naam = "Andere" };

                IList<Specialisatie> specialisaties = new List<Specialisatie>();
                specialisaties.Add(specialisatie1);
                specialisaties.Add(specialisatie2);
                specialisaties.Add(specialisatie3);
                specialisaties.Add(specialisatie4);
                specialisaties.Add(specialisatie5);
                specialisaties.Add(specialisatie6);
                specialisaties.Add(specialisatie7);
                specialisaties.Add(andere);
                context.Specialisaties.AddRange(specialisaties);
                #endregion

                #region instellingen
                var mailboxInstelling = new Instelling(Instelling.MailboxStages, "stagetoegepasteinformatica@hogent.be");
                context.Instellingen.Add(mailboxInstelling);
                #endregion

                #region Keuzepakketten
                //e-commerce-mobile-netwerken-mainframe
                Keuzepakket keuzepakket1 = new Keuzepakket() { Naam = "Netwerken" };
                Keuzepakket keuzepakket2 = new Keuzepakket() { Naam = "e-commerce" };
                Keuzepakket keuzepakket3 = new Keuzepakket() { Naam = "Mobile" };
                Keuzepakket keuzepakket4 = new Keuzepakket() { Naam = "Mainframe" };

                IList<Keuzepakket> keuzepakketten = new List<Keuzepakket>();
                keuzepakketten.Add(keuzepakket1);
                keuzepakketten.Add(keuzepakket2);
                keuzepakketten.Add(keuzepakket3);
                keuzepakketten.Add(keuzepakket4);
                context.Keuzepakketten.AddRange(keuzepakketten);
                #endregion

                #region bedrijf1

                Bedrijf bedrijf1 = new Bedrijf()
                  {
                      Email = "bedrijf@test.be",
                      Naam = "TESTbedrijf",
                      Bedrijfsactiviteiten = "TEST",
                      Bereikbaarheid = "TEST",
                      Postcode = "1243",
                      Gemeente = "TESTgemeente",
                      Straat = "TESTstraat"
                  };

                context.SaveChanges();
                var stagementors = new List<Contactpersoon>();
                for (int i = 1; i <= 5; i++)
                {
                    Contactpersoon stagementor1 = new Contactpersoon()
                    {
                        Voornaam = "voornaam" + i,
                        Familienaam = "Naam" + i,
                        Email = "stagementor" + i + "@bedrijf.be",
                        Gsm = "123456",
                        Telefoon = "1234567",
                        IsStagementor = true,
                        IsContractondertekenaar = false,
                        Bedrijfsfunctie = "bedrijfsfunctie"
                    };
                    bedrijf1.AddContactpersoon(stagementor1);
                    stagementors.Add(stagementor1);
                }

                Contactpersoon contractOndertekenaar1 = new Contactpersoon()
                {
                    Voornaam = "voornaam0",
                    Familienaam = "Naam0",
                    Email = "contractondertekenaar1@bedrijf.be",
                    Gsm = "123456",
                    Telefoon = "1234567",
                    IsStagementor = false,
                    IsContractondertekenaar = true,
                    Bedrijfsfunctie = "bedrijfsfunctie",
                    Aanspreektitel = "meneer"
                };

                bedrijf1.AddContactpersoon(contractOndertekenaar1);

                context.Bedrijven.Add(bedrijf1);
                #endregion

                #region studenten

                Student student = new Student()
                {
                    Voornaam = "TEST",
                    Familienaam = "TEST",
                    HogentEmail = "student@test.be"
                };

                var studenten = new List<Student>() { { student } };
                context.Studenten.AddRange(studenten);
                #endregion

                #region begeleider
                var begeleider = new Begeleider()
                {
                    Voornaam = "TEST",
                    Familienaam = "TEST",
                    HogentEmail = "begeleider@test.be"
                };

                var stageopdrachten = new List<Stageopdracht>();
                for (int i = 0, acadJaar = 2008; i < 36; i++)
                {
                    if (i % 6 == 0)
                    {
                        acadJaar++;
                    }
                    var academiejaar = acadJaar + "-" + (acadJaar + 1);
                    var stageopdracht = new Stageopdracht()
                    {
                        Omschrijving = "omschrijving",
                        Titel = "stage " + academiejaar,
                        Academiejaar = academiejaar,
                        AantalStudenten = 1,
                        Bedrijf = bedrijf1,
                        Gemeente = "Gemeente123",
                        Semester2 = true,
                        Specialisatie = "TEST",
                        Stagebegeleider = begeleider,
                        Status = StageopdrachtStatus.Goedgekeurd
                    };
                    stageopdrachten.Add(stageopdracht);
                }
                var opdracht = stageopdrachten[stageopdrachten.Count - 1];
                opdracht.Stages = new List<Stage>() { 
                { new Stage() { Stageopdracht = opdracht, Student = student } } };
                opdracht.Status = StageopdrachtStatus.Toegewezen;

                begeleider.Stages = stageopdrachten;
                context.Begeleiders.Add(begeleider);
                context.Begeleiders.Add(new Begeleider() { HogentEmail = "adminBegeleider@test.be" });
                #endregion

                #region goedgekeurde/toegewezen test stages
                var random = new Random();
                for (int i = 0; i < 15; i++)
                {
                    Stageopdracht stageopdracht = new Stageopdracht()
                    {
                        Titel = "TEST " + i,
                        Specialisatie = specialisaties[random.Next(0, (specialisaties.Count))].Naam,
                        Semester1 = random.Next(0, 2) == 0,
                        Semester2 = random.Next(0, 2) == 0,
                        Omschrijving = "omschrijving" + i,
                        Academiejaar = "2014-2015",
                        AantalStudenten = 2,
                        Contractondertekenaar = contractOndertekenaar1,
                        Stagementor = stagementors[random.Next(0, stagementors.Count)],
                        Bedrijf = bedrijf1,
                        Gemeente = "Gemeente1"
                    };
                    if (i % 2 == 0)
                    {
                        stageopdracht.Status = StageopdrachtStatus.Goedgekeurd;
                    }
                    bedrijf1.AddStageopdracht(stageopdracht);
                }

                var teststages = new List<Stageopdracht>();
                for (int i = 15; i < 51; i++)
                {

                    var stage = new Stageopdracht()
                    {
                        Omschrijving = "TEST" + i,
                        Titel = "TEST " + i,
                        Academiejaar = "2014-2015",
                        AantalStudenten = 1,
                        Bedrijf = bedrijf1,
                        Gemeente = "Gemeente123",
                        Semester2 = true,
                        Specialisatie = "TEST",
                        Status = StageopdrachtStatus.Goedgekeurd
                    };
                    //stage.Studenten = new List<StageStudentRelatie>() { { new StageStudentRelatie() { Stage = stage, Student = student1 } } };
                    teststages.Add(stage);
                }
                context.Stageopdrachten.AddRange(teststages);
                #endregion

                context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                string message = String.Empty;
                foreach (var eve in e.EntityValidationErrors)
                {

                    message +=
                        String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.GetValidationResult());
                    foreach (var ve in eve.ValidationErrors)
                    {
                        message +=
                            String.Format("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw new ApplicationException("Fout bij aanmaken database " + message);
            }
            AddOudeGegevens(context);
        }

        public async void AddOudeGegevens(StageToolDbContext context)
        {
            try
            {
                using (var oudeContext = new OudeGegevensDbContext())
                {
                    var begeleiders = new List<Begeleider>();
                    foreach (var docent in oudeContext.Docenten)
                    {
                        var begeleider = Converter.ToBegeleider(docent);
                        if (!context.Begeleiders.Any(b => b.HogentEmail == docent.email)
                            && begeleiders.All(s => s.HogentEmail != docent.email))
                        {
                            begeleiders.Add(begeleider);
                        }
                    }
                    context.Begeleiders.AddRange(begeleiders);
                    context.SaveChanges();

                    var studenten = new List<Student>();
                    foreach (var oudeStudent in oudeContext.Studenten)
                    {
                        var student = Converter.ToStudent(oudeStudent);
                        if (!context.Studenten.Any(s => s.HogentEmail == oudeStudent.email)
                            && studenten.All(s => s.HogentEmail != oudeStudent.email))
                        {
                            studenten.Add(student);
                        }
                    }
                    context.Studenten.AddRange(studenten);
                    context.SaveChanges();
                    var bedrijven = new List<Bedrijf>();
                    foreach (var stagebedrijf in oudeContext.Stagebedrijf.Include(b => b.relatie).Include(b => b.stage).ToList())
                    {
                        var bedrijf = Converter.ToBedrijf(stagebedrijf);

                        var bedrijfEmailBestaatAl = bedrijven.Count(b => b.Email == bedrijf.Email) > 0;//kolom email van bedrijven is uniek
                        if (bedrijfEmailBestaatAl)
                        {
                            var email = bedrijf.Email;
                            if (string.IsNullOrWhiteSpace(email))
                            {
                                bedrijf.Email = "geenEmail" + Guid.NewGuid();//bedrijf zonder contactpersonen
                            }
                            else
                            {
                                bedrijf.Email = email + 2;
                            }
                        }
                        bedrijven.Add(bedrijf);

                        var stageopdrachten = new List<Stageopdracht>();
                        foreach (var stage in stagebedrijf.stage.ToList()) //relatie: mentor, relatie1: constractond
                        {
                            var stageopdracht = Converter.ToStageopdracht(stage);
                            if (stage.relatie != null)
                            {
                                var stagementor = bedrijf.Contactpersonen.FirstOrDefault(c => c.Familienaam == stage.relatie.naam
                                                                                && c.Voornaam == stage.relatie.voornaam);
                                if (stagementor != null)
                                {
                                    stagementor.IsStagementor = true;
                                    stageopdracht.StagementorEmail = stagementor.Email;
                                    stageopdracht.StagementorNaam = stagementor.Naam;
                                }
                                stageopdracht.Stagementor = stagementor;
                            }
                            if (stage.relatie1 != null)
                            {
                                var contrOnd = bedrijf.Contactpersonen.FirstOrDefault(c => c.Familienaam == stage.relatie1.naam
                                                                                && c.Voornaam == stage.relatie1.voornaam);
                                stageopdracht.Contractondertekenaar = contrOnd;
                                if (contrOnd != null)
                                {
                                    contrOnd.IsContractondertekenaar = true;
                                    stageopdracht.ContractondertekenaarEmail = contrOnd.Email;
                                    stageopdracht.ContractondertekenaarNaam = contrOnd.Naam;
                                }
                            }
                            if (stage.docent != null)
                            {
                                stageopdracht.Stagebegeleider =
                                    context.Begeleiders.FirstOrDefault(d => d.HogentEmail == stage.docent.email);
                            }
                            var stageopdrachtstudenten = stage.studenten
                                .Select(oudeStudent => context.Studenten.FirstOrDefault(s => s.HogentEmail == oudeStudent.email))
                                .Where(student => student != null).ToList();

                            var stages = new List<Stage>();
                            foreach (var student in stageopdrachtstudenten)
                            {
                                stages.Add(new Stage { Stageopdracht = stageopdracht, Student = student });
                            }
                            stageopdracht.Stages = stages;
                            stageopdracht.Bedrijf = bedrijf;
                            stageopdrachten.Add(stageopdracht);
                        }
                        bedrijf.Stageopdrachten = stageopdrachten;
                    }
                    context.Bedrijven.AddRange(bedrijven);
                }
                await context.SaveChangesAsync();

                #region begeleider & student logins
                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

                //logins begeleiders
                var begeleiderLogins = new List<ApplicationUser>();
                foreach (var begeleider in context.Begeleiders)
                {
                    var user = new ApplicationUser()
                    {
                        Email = begeleider.HogentEmail,
                        UserName = begeleider.HogentEmail,
                        EmailConfirmed = true
                    };
                    begeleiderLogins.Add(user);
                }
                foreach (var begeleiderLogin in begeleiderLogins)
                {
                    userManager.Create(begeleiderLogin);
                    userManager.AddToRole(begeleiderLogin.Id, Role.Begeleider);
                }

                //login studenten
                var olivierStudent =
                    context.Studenten.FirstOrDefault(st => st.HogentEmail == "olivier.neirynck.q1177@student.hogent.be");
                var olivierLogin = new ApplicationUser
                {
                    Email = olivierStudent.HogentEmail,
                    UserName = olivierStudent.HogentEmail,
                    EmailConfirmed = true
                };
                userManager.Create(olivierLogin);
                userManager.AddToRole(olivierLogin.Id, Role.Student);
                #endregion

            }
            catch (DbEntityValidationException e)
            {
                string message = String.Empty;
                foreach (var eve in e.EntityValidationErrors)
                {

                    message +=
                        String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.GetValidationResult());
                    foreach (var ve in eve.ValidationErrors)
                    {
                        message +=
                            String.Format("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw new ApplicationException("Fout bij aanmaken database " + message);
            }
        }

    }
}