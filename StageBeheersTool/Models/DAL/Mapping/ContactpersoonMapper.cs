using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class ContactpersoonMapper : EntityTypeConfiguration<Contactpersoon>
    {
        public ContactpersoonMapper()
        {
            this.ToTable("Contactpersonen");
            this.Property(cp => cp.Voornaam).IsRequired().HasMaxLength(50);
            this.Property(cp => cp.Email).IsRequired().HasMaxLength(50); 
            this.Property(cp => cp.Familienaam).IsRequired().HasMaxLength(50);
            this.Property(cp => cp.Bedrijfsfunctie).IsRequired().HasMaxLength(200);

        }
    }
}