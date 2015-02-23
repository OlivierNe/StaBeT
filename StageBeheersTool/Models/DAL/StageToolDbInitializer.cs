using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using StageBeheersTool;

namespace StageBeheersTool.Models.DAL
{
    public class StageToolDbInitializer :
    //        DropCreateDatabaseAlways<StageToolDbContext>
     DropCreateDatabaseIfModelChanges<StageToolDbContext>
    {
        protected override void Seed(StageToolDbContext context)
        {
            base.Seed(context);
            try
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new StageToolDbContext()));
                roleManager.Create(new IdentityRole("student"));
                roleManager.Create(new IdentityRole("admin"));
                roleManager.Create(new IdentityRole("begeleider"));
                roleManager.Create(new IdentityRole("bedrijf"));

                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new StageToolDbContext()));

                ApplicationUser user = new ApplicationUser() { Email = "test@bedrijf.be", UserName = "test@bedrijf.be", EmailConfirmed = true };
                userManager.Create(user, "wachtwoord");
                userManager.AddToRole(user.Id, "bedrijf");

                //netwerk – programmeren – mobile – onderzoek – mainframe 
                Specialisatie specialisatie1 = new Specialisatie() { Naam = "Netwerken" };
                Specialisatie specialisatie2 = new Specialisatie() { Naam = "Programmeren" };
                Specialisatie specialisatie3 = new Specialisatie() { Naam = "Mobile" };
                Specialisatie specialisatie4 = new Specialisatie() { Naam = "Onderzoek" };
                Specialisatie specialisatie5 = new Specialisatie() { Naam = "Mainframe" };

                IList<Specialisatie> specialisaties = new List<Specialisatie>();
                specialisaties.Add(specialisatie1);
                specialisaties.Add(specialisatie2);
                specialisaties.Add(specialisatie3);
                specialisaties.Add(specialisatie4);
                specialisaties.Add(specialisatie5);

                context.Specialisaties.AddRange(specialisaties);

            

                Bedrijf bedrijf1 = new Bedrijf()
                  {
                      Email = "test@bedrijf.be",
                      Naam = "bedrijf1",
                      Telefoonnummer = "?",
                      BedrijfsActiviteiten = "?",
                      Bereikbaarheid = "?",
                      Postcode = "1243",
                      Straatnummer = "1",
                      Gemeente = "gemeente1",
                      Straat = "straat1"
                  };
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
                        IsContractOndertekenaar = false,
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
                    IsContractOndertekenaar = true,
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
                        Specialisatie = specialisaties[random.Next(0, 4)],
                        Semester = 1,
                        Omschrijving = "omschrijving" + i,
                        Academiejaar = "2015-2016",
                        AantalStudenten = 2,
                        ContractOndertekenaar = contractOndertekenaar1,
                        Stagementor = stagementors[random.Next(0, stagementors.Count)],
                        Bedrijf = bedrijf1
                    };
                    bedrijf1.AddStageopdracht(stageopdracht);
                }

                context.Bedrijven.Add(bedrijf1);
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