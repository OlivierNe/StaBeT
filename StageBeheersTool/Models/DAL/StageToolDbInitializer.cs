using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.DAL
{
    public class StageToolDbInitializer : 
        //DropCreateDatabaseAlways<StageToolDbContext>
         DropCreateDatabaseIfModelChanges<StageToolDbContext>
    {
        protected override void Seed(StageToolDbContext context)
        {
            base.Seed(context);
            try
            {
              /*  Bedrijf b = new Bedrijf()
                {
                    Email = "test@test.be",
                    Postcode = 1243,
                    StraatNr = 1,Naam="naam"
                };
                context.Bedrijven.Add(b);
                context.SaveChanges();*/
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