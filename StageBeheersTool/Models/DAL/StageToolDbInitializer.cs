using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OudeGegevens;
using StageBeheersTool.Helpers;
using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using StageBeheersTool.Models.Identity;
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
                context.Instellingen.Add(new Instelling(Instelling.MailboxStages, "stagetoegepasteinformatica@hogent.be"));
                context.Instellingen.Add(new Instelling(Instelling.AantalWekenStage, "14"));
                context.Instellingen.Add(new Instelling(Instelling.BeginNieuwAcademiejaar, "01/09/0001 0:00:00"));

                context.AcademiejarenInstellingen.Add(new AcademiejaarInstellingen()
                {
                    Academiejaar = "2014-2015",
                    Semester1Begin = new DateTime(2014, 9, 22),
                    Semester2Einde = new DateTime(2015, 2, 6),
                    Semester2Begin = new DateTime(2015, 2, 9),
                    Semester1Einde = new DateTime(2015, 6, 26)
                });

                context.AcademiejarenInstellingen.Add(new AcademiejaarInstellingen()
                {
                    Academiejaar = "2015-2016",
                    Semester1Begin = new DateTime(2015, 9, 21),
                    Semester2Einde = new DateTime(2016, 2, 5),
                    Semester2Begin = new DateTime(2016, 2, 8),
                    Semester1Einde = new DateTime(2016, 7, 1)
                });
                #endregion

                #region evaluatievragen

                var stagebezoek1Teller = 0;
                var stagebezoek2Teller = 0;
                var stagebezoek3Teller = 0;
                var stagebezoekExtraTeller = 0;

                var evaluatievragen = new List<Evaluatievraag>
                {
                    new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Voert de student de taakomschrijving uit (zie stage-overeenkomst)?",
                        Stagebezoek = 1,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek1Teller
                    },
                    new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Toont de student inzet en enthousiasme voor het werk?",
                        Stagebezoek = 1,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek1Teller
                    },
                   new Evaluatievraag
                    {
                       SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Heeft de student inzicht in de uit te voeren taken?",
                        Stagebezoek = 1,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek1Teller
                    },
                    new Evaluatievraag
                    {
                      SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Voert de student de taken nauwkeurig en goed uit?",
                        Stagebezoek = 1,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek1Teller
                    },
                    new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Respecteert de student gemaakte afspraken?",
                        Stagebezoek = 1,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek1Teller
                    },
                    new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Neemt de student initiatief?",
                        Stagebezoek = 1,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek1Teller
                    },
                    new Evaluatievraag
                    {
                      SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Kan de student omgaan met collega’s?",
                        Stagebezoek = 1,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek1Teller
                    },
                   new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Rapporteert de student (wekelijks stagedagboek)?",
                        Stagebezoek = 1,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek1Teller
                    },
                    new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Zoekt de student actief naar feedback?",
                        Stagebezoek = 1,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek1Teller
                    },
                    new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Staat de student open voor kritiek?",
                        Stagebezoek = 1,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek1Teller
                    },
                     new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Openvraag,
                        Vraag = "Problemen gemeld door de stagementor:",
                        Stagebezoek = 1,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek1Teller
                    },
                     new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Openvraag,
                        Vraag = "Verbeterpunten voor de stagiair?",
                        Stagebezoek = 1,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek1Teller
                    },
                     new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Begrijp je de opdracht(en)?",
                        Stagebezoek = 1,
                        Voor = "Student",
                        Volgorde = ++stagebezoek1Teller
                    },
                     new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Heb je een planning gemaakt?",
                        Stagebezoek = 1,
                        Voor = "Student",
                        Volgorde = ++stagebezoek1Teller
                    },
                     new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Openvraag,
                        Vraag = "Taken die de stagiair uitvoert:",
                        Stagebezoek = 1,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek1Teller
                    },
                     new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Openvraag,
                        Vraag = "Problemen gemeld door de stagiair",
                        Stagebezoek = 1,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek1Teller
                    },
                     new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Openvraag,
                        Vraag = "Raadgevingen en afspraken met de stagiair:",
                        Stagebezoek = 1,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek1Teller
                    },

                     new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Verloopt de stage zoals gepland?",
                        Stagebezoek = 2,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek2Teller
                    },
                     new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Merkt u een evolutie bij de student?",
                        Stagebezoek = 2,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek2Teller
                    },
                     new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Werkt de student zelfstandig?",
                        Stagebezoek = 2,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek2Teller
                    },
                     new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Openvraag,
                        Vraag = "Extra info van de stagementor betreffende de student:",
                        Stagebezoek = 2,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek2Teller
                    },
                     new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Meningvraag,
                        Vraag = "Je werd goed opgevangen bij het begin van je stage",
                        Stagebezoek = 2,
                        Voor = "Student",
                        Volgorde = ++stagebezoek2Teller
                    },
                     new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Meningvraag,
                        Vraag = "Je stage past bij je opleiding",
                        Stagebezoek =2,
                        Voor = "Student",
                        Volgorde = ++stagebezoek2Teller
                    },
                     new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Meningvraag,
                        Vraag = "Je stage leunt aan bij je interesse",
                        Stagebezoek = 2,
                        Voor = "Student",
                        Volgorde = ++stagebezoek2Teller
                    },
                     new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Meningvraag,
                        Vraag = "Je stage is leerzaam",
                        Stagebezoek = 2,
                        Voor = "Student",
                        Volgorde = ++stagebezoek2Teller
                    },
                     new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Meningvraag,
                        Vraag = "Het niveau van je stage is hoog",
                        Stagebezoek = 2,
                        Voor = "Student",
                        Volgorde = ++stagebezoek2Teller
                    },
                     new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Meningvraag,
                        Vraag = "Je werkt graag samen met je collega’s",
                        Stagebezoek = 2,
                        Voor = "Student",
                        Volgorde = ++stagebezoek2Teller
                    },
                     new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Meningvraag,
                        Vraag = "Je kan terecht bij je stagementor",
                        Stagebezoek = 2,
                        Voor = "Student",
                        Volgorde = ++stagebezoek2Teller
                    },
                     new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Meerkeuzevraag,
                        MeerkeuzeAntwoorden = "Analyse en ontwerpen;Programmeren;Databanken;Netwerken;Multimedia",
                        Vraag = "Welke opleidingsonderdelen heb je al gebruikt?",
                        Stagebezoek = 2,
                        Voor = "Student",
                        Volgorde = ++stagebezoek2Teller
                    },
                     new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Openvraag,
                        Vraag = "Heb je al nieuwe materie aangeleerd? Indien ja, welke?",
                        Stagebezoek = 2,
                        Voor = "Student",
                        Volgorde = ++stagebezoek2Teller
                    },
                     new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Je stagementor heeft het evaluatieformulier 1 met jou besproken",
                        Stagebezoek = 2,
                        Voor = "Student",
                        Volgorde = ++stagebezoek2Teller
                    },
                     new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Openvraag,
                        Vraag = "Heb je al problemen ondervonden op je stage? Waar heb je hulp gezocht?",
                        Stagebezoek = 2,
                        Voor = "Student",
                        Volgorde = ++stagebezoek2Teller
                    },
                     new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Bent u tevreden over de student?",
                        Stagebezoek = 3,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek3Teller
                    }, new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Merkt u een evolutie bij de student?",
                        Stagebezoek = 3,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek3Teller
                    },  new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Werkt de student zelfstandig?",
                        Stagebezoek = 3,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek3Teller
                    }, new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Komt de student de gemaakte afspraken na?",
                        Stagebezoek = 3,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek3Teller
                    }, new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Communiceert de student met de collega’s?",
                        Stagebezoek = 3,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek3Teller
                    }, new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Hanteert de student een correct taalgebruik?",
                        Stagebezoek = 3,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek3Teller
                    }, new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Gaat de student op een positieve manier om met kritiek?",
                        Stagebezoek = 3,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek3Teller
                    }, new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Kan de student ook moeilijkere taken aan?",
                        Stagebezoek = 3,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek3Teller
                    }, new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Openvraag,
                        Vraag = "Bezit de student voldoende kennis en vaardigheden om de opdrachten goed uit te voeren?",
                        Stagebezoek = 3,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek3Teller
                    }, new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Openvraag,
                        Vraag = "Positieve punten over de stagiair?",
                        Stagebezoek = 3,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek3Teller
                    }, new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Openvraag,
                        Vraag = "Verbeterpunten voor de opleiding?",
                        Stagebezoek = 3,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek3Teller
                    }, new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Openvraag,
                        Vraag = "Problemen gemeld door de stagementor:",
                        Stagebezoek = 3,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek3Teller
                    }, new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Volg je de opgestelde planning?",
                        Stagebezoek = 3,
                        Voor = "Student",
                        Volgorde = ++stagebezoek3Teller
                    }, new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Ligt de moeilijkheidsgraad van je taken hoog?",
                        Stagebezoek = 3,
                        Voor = "Student",
                        Volgorde = ++stagebezoek3Teller
                    }, new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Pas je (nieuwe) materie gemakkelijk toe?",
                        Stagebezoek = 3,
                        Voor = "Student",
                        Volgorde = ++stagebezoek3Teller
                    },new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Hou je rekening met de raadgevingen van je stagementor?",
                        Stagebezoek = 3,
                        Voor = "Student",
                        Volgorde = ++stagebezoek3Teller
                    }, new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.JaNeevraag,
                        Vraag = "Heb je al een inleiding gemaakt voor je stageverslag?",
                        Stagebezoek = 3,
                        Voor = "Student",
                        Volgorde = ++stagebezoek3Teller
                    }, new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Openvraag,
                        Vraag = "Raadgevingen en afspraken met de stagiair:",
                        Stagebezoek = 3,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoek3Teller
                    }, new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Openvraag,
                        Vraag = "Problemen gemeld door de stagementor: ",
                        Stagebezoek = -1,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoekExtraTeller
                    }, new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Openvraag,
                        Vraag = "Afspraken met de stagementor: ",
                        Stagebezoek = -1,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoekExtraTeller
                    }, new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Openvraag,
                        Vraag = "Problemen gemeld door de stagiair:",
                        Stagebezoek = -1,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoekExtraTeller
                    }, new Evaluatievraag
                    {
                        SoortVraag = SoortVraag.Openvraag,
                        Vraag = "Raadgevingen en afspraken met de stagiair:",
                        Stagebezoek = -1,
                        Voor = "Stagementor",
                        Volgorde = ++stagebezoekExtraTeller
                    },
                };
                context.Evaluatievragen.AddRange(evaluatievragen);
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

                //testgegevens:
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
                //for (int i = 0, acadJaar = 2008; i < 36; i++)
                //{
                //    if (i % 6 == 0)
                //    {
                //        acadJaar++;
                //    }
                //    var academiejaar = acadJaar + "-" + (acadJaar + 1);
                //    var stageopdracht = new Stageopdracht()
                //    {
                //        Omschrijving = "omschrijving",
                //        Titel = "stage " + academiejaar,
                //        Academiejaar = academiejaar,
                //        AantalStudenten = 1,
                //        Bedrijf = bedrijf1,
                //        Gemeente = "Gemeente123",
                //        Semester2 = true,
                //        Specialisatie = "TEST",
                //        Stagebegeleider = begeleider,
                //        Status = StageopdrachtStatus.Goedgekeurd
                //    };
                //    stageopdrachten.Add(stageopdracht);
                //}
                //var opdracht = stageopdrachten[stageopdrachten.Count - 1];
                //opdracht.Stages = new List<Stage>() { 
                //{ new Stage() { Stageopdracht = opdracht, Student = student } } };
                //opdracht.Status = StageopdrachtStatus.Toegewezen;

                //begeleider.Stageopdrachten = stageopdrachten;

                context.Begeleiders.Add(begeleider);
                context.Begeleiders.Add(new Begeleider() { HogentEmail = "adminBegeleider@test.be" });
                #endregion

                #region teststageopdrachten
                /*
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
                context.Stageopdrachten.AddRange(teststages);*/
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
                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
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
                        var student = Converter.ToStudent(oudeStudent, userManager);
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
                                stages.Add(new Stage { Stageopdracht = stageopdracht, Student = student, Semester = 2 });
                            }
                            stageopdracht.Stages = stages;
                            stageopdracht.Bedrijf = bedrijf;
                            stageopdrachten.Add(stageopdracht);
                        }
                        bedrijf.Stageopdrachten = stageopdrachten;
                    }
                    context.Bedrijven.AddRange(bedrijven);
                }

                foreach (var so in context.Stageopdrachten.Where(so => so.Status == StageopdrachtStatus.Toegewezen && so.Stages.Count == 0).ToList())
                {
                    so.Status = StageopdrachtStatus.Goedgekeurd;
                }

                await context.SaveChangesAsync();

                #region begeleider & student logins

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
                var anneleen = context.Users.FirstOrDefault(u => u.UserName == "anneleen.bekkens@hogent.be");
                if (anneleen != null)
                    userManager.AddToRole(anneleen.Id, Role.Admin);
                var sofie = new ApplicationUser { Email = "sofie.moreau@hogent.be", UserName = "sofie.moreau@hogent.be", EmailConfirmed = true };
                userManager.Create(sofie);
                userManager.AddToRole(sofie.Id, Role.Admin);

                //login studenten
                var acadj = AcademiejaarHelper.HuidigAcademiejaar();
                var studentLogins = new List<ApplicationUser>();
                foreach (var student in context.Studenten.Where(s => s.Stages.Any(stage => stage.Stageopdracht.Academiejaar == acadj)))
                {
                    var user = new ApplicationUser()
                    {
                        Email = student.HogentEmail,
                        UserName = student.HogentEmail,
                        EmailConfirmed = true
                    };
                    studentLogins.Add(user);
                }
                foreach (var studentLogin in studentLogins)
                {
                    userManager.Create(studentLogin);
                    userManager.AddToRole(studentLogin.Id, Role.Student);
                }

                //login bedrijven: gaat niet -> bedrijven moeten wachtwoord opgeven. account aanmaken in AccountController/Activate

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