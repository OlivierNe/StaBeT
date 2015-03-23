using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OudeGegevens;
using StageBeheersTool.Controllers;
using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using StageBeheersTool;
using StageBeheersTool.Models.Authentication;
using StageBeheersTool.Models.Services;
using StageBeheersTool.OudeGegevens;
using System.Data.Entity.Migrations;
using StageBeheersTool.ViewModels;


namespace StageBeheersTool.Models.DAL
{
    public class StageToolDbInitializer :
        // DropCreateDatabaseAlways<StageToolDbContext>
     DropCreateDatabaseIfModelChanges<StageToolDbContext>
    {

        public void RunSeed(StageToolDbContext ctx)
        {
            this.Seed(ctx);
        }

        protected override void Seed(StageToolDbContext context)
        {
            base.Seed(context);
            try
            {
                #region logins/roles
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new StageToolDbContext()));
                roleManager.Create(new IdentityRole("student"));
                roleManager.Create(new IdentityRole("admin"));
                roleManager.Create(new IdentityRole("begeleider"));
                roleManager.Create(new IdentityRole("bedrijf"));

                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new StageToolDbContext()));

                ApplicationUser user = new ApplicationUser() { Email = "test@bedrijf.be", UserName = "test@bedrijf.be", EmailConfirmed = true };
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
                userManager.Create(user3, "student");
                userManager.AddToRole(user3.Id, "student");
                context.Studenten.Add(new Student() { HogentEmail = "student@test.be" });

                ApplicationUser user4 = new ApplicationUser()
                {
                    Email = "begeleider@test.be",
                    UserName = "begeleider@test.be",
                    EmailConfirmed = true
                };
                userManager.Create(user4, "begeleider");
                userManager.AddToRole(user4.Id, "begeleider");
                context.Begeleiders.Add(new Begeleider() { HogentEmail = "begeleider@test.be" });

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
                      Email = "test@bedrijf.be",
                      Naam = "bedrijf1",
                      Telefoonnummer = "?",
                      Bedrijfsactiviteiten = "?",
                      Bereikbaarheid = "?",
                      Postcode = "1243",
                      Straatnummer = "1",
                      Gemeente = "gemeente1",
                      Straat = "straat1"
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
                        Gsmnummer = "123456",
                        Telefoonnummer = "1234567",
                        IsStagementor = true,
                        IsContractondertekenaar = false,
                        Bedrijfsfunctie = "bedrijfsfunctie",
                        Aanspreektitel = "meneer"
                    };
                    bedrijf1.AddContactpersoon(stagementor1);
                    stagementors.Add(stagementor1);
                }

                Contactpersoon contractOndertekenaar1 = new Contactpersoon()
                {
                    Voornaam = "voornaam0",
                    Familienaam = "Naam0",
                    Email = "contractondertekenaar1@bedrijf.be",
                    Gsmnummer = "123456",
                    Telefoonnummer = "1234567",
                    IsStagementor = false,
                    IsContractondertekenaar = true,
                    Bedrijfsfunctie = "bedrijfsfunctie",
                    Aanspreektitel = "meneer"
                };

                bedrijf1.AddContactpersoon(contractOndertekenaar1);

                var random = new Random();
                for (int i = 0; i < 15; i++)
                {
                    Stageopdracht stageopdracht = new Stageopdracht()
                    {
                        Titel = "titel" + i,
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
                    stageopdracht.AantalToegewezenStudenten = random.Next(0, stageopdracht.AantalStudenten);
                    if (i % 2 == 0)
                    {
                        stageopdracht.Status = StageopdrachtStatus.Goedgekeurd;
                    }
                    bedrijf1.AddStageopdracht(stageopdracht);
                }

                context.Bedrijven.Add(bedrijf1);
                context.SaveChanges();
                #endregion

                #region bedrijf2
                var bedrijf2 = new Bedrijf()
                          {
                              Email = "test@bedrijf2.be",
                              Naam = "bedrijf2",
                              Telefoonnummer = "?",
                              Postcode = "1243",
                              Straatnummer = "2",
                              Gemeente = "gemeente2",
                              Straat = "straat2"
                          };
                var stagementors2 = new List<Contactpersoon>();
                for (int i = 1; i <= 5; i++)
                {
                    Contactpersoon stagementor2 = new Contactpersoon()
                    {
                        Voornaam = "voornaam" + i,
                        Familienaam = "Naam" + i,
                        Email = "stagementor" + i + "@bedrijf2.be",
                        Gsmnummer = "123456",
                        Telefoonnummer = "1234567",
                        IsStagementor = true,
                        IsContractondertekenaar = false,
                        Bedrijfsfunctie = "bedrijfsfunctie",
                        Aanspreektitel = "meneer"
                    };
                    bedrijf2.AddContactpersoon(stagementor2);
                    stagementors2.Add(stagementor2);
                }
                Contactpersoon contractOndertekenaar2 = new Contactpersoon()
                {
                    Voornaam = "voornaam0",
                    Familienaam = "Naam0",
                    Email = "contractondertekenaar1@bedrijf2.be",
                    Gsmnummer = "123456",
                    Telefoonnummer = "1234567",
                    IsStagementor = false,
                    IsContractondertekenaar = true,
                    Bedrijfsfunctie = "bedrijfsfunctie"
                };
                bedrijf2.AddContactpersoon(contractOndertekenaar2);
                for (int i = 0; i < 5; i++)
                {
                    Stageopdracht stageopdracht = new Stageopdracht()
                    {
                        Titel = "opdracht " + i,
                        Specialisatie = specialisaties[random.Next(0, (specialisaties.Count - 1))].Naam,
                        Semester1 = random.Next(0, 2) == 0,
                        Semester2 = random.Next(0, 2) == 0,
                        Omschrijving = "omschrijving " + i,
                        Academiejaar = "2014-2015",
                        AantalStudenten = random.Next(1, 4),
                        Contractondertekenaar = contractOndertekenaar2,
                        Stagementor = stagementors2[random.Next(0, stagementors2.Count)],
                        Bedrijf = bedrijf2,
                        Gemeente = "Gemeente2",
                        Status = StageopdrachtStatus.Goedgekeurd
                    };
                    stageopdracht.AantalToegewezenStudenten = random.Next(0, stageopdracht.AantalStudenten);
                    bedrijf2.AddStageopdracht(stageopdracht);
                }

                context.Bedrijven.Add(bedrijf2);
                context.SaveChanges();
                #endregion

                #region student1
                Student student1 = new Student()
                {
                    Voornaam = "Olivier",
                    Familienaam = "Neirynck",
                    HogentEmail = "olivier.neirynck.q1177@student.hogent.be"
                };


                context.Studenten.Add(student1);
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

                    foreach (var stagebedrijf in oudeContext.Stagebedrijf.Include(b => b.relatie).Include(b => b.stage).ToList())
                    {
                        var bedrijf = Converter.ToBedrijf(stagebedrijf);

                        context.Bedrijven.AddOrUpdate(bedrijf);

                        var stageopdrachten = new List<Stageopdracht>();
                        foreach (var stage in stagebedrijf.stage.ToList()) //relatie: mentor, relatie: constractond
                        {
                            var stageopdracht = Converter.ToStageopdracht(stage);
                            if (stage.relatie != null)
                            {
                                stageopdracht.Stagementor =
                                    bedrijf.Contactpersonen.FirstOrDefault(c => c.Familienaam == stage.relatie.naam
                                                                                && c.Voornaam == stage.relatie.voornaam);
                            }
                            if (stage.relatie1 != null)
                            {
                                stageopdracht.Contractondertekenaar =
                                    bedrijf.Contactpersonen.FirstOrDefault(c => c.Familienaam == stage.relatie1.naam
                                                                                && c.Voornaam == stage.relatie1.voornaam);
                            }
                            if (stage.docent != null)
                            {
                                stageopdracht.Stagebegeleider =
                                    context.Begeleiders.FirstOrDefault(d => d.HogentEmail == stage.docent.email);
                            }
                            var stageopdrachtstudenten = stage.studenten
                                .Select(oudeStudent => context.Studenten.FirstOrDefault(s => s.HogentEmail == oudeStudent.email))
                                .Where(student => student != null).ToList();
                            stageopdracht.Studenten = stageopdrachtstudenten;
                            stageopdracht.Bedrijf = bedrijf;
                            stageopdrachten.Add(stageopdracht);
                        }
                        bedrijf.Stageopdrachten = stageopdrachten;
                    }
                }
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
        }

    }
}