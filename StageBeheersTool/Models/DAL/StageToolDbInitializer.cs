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
        //    DropCreateDatabaseAlways<StageToolDbContext>
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

                context.Specialisaties.Add(specialisatie1);
                context.Specialisaties.Add(specialisatie2);
                context.Specialisaties.Add(specialisatie3);
                context.Specialisaties.Add(specialisatie4);
                context.Specialisaties.Add(specialisatie5);

                Bedrijf bedrijf1 = new Bedrijf()
                  {
                      Email = "test@bedrijf.be",
                      Postcode = "1243",
                      Straatnummer = 1,
                      Naam = "bedrijf1",
                      Telefoonnummer = "?",
                      BedrijfsActiviteiten = "?",
                      Bereikbaarheid = "?",
                      Gemeente = "gemeente1",
                      Straat = "straat1"
                  };
                for (int i = 0; i < 15; i++)
                {
                    Stageopdracht stageopdracht = new Stageopdracht()
                    {
                        Titel = "titel" + i,
                        Specialisatie = specialisatie2,
                        Semester = 1,
                        Omschrijving = "omschrijving" + i,
                        Academiejaar = "2015-2016",
                        AantalStudenten = 2
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