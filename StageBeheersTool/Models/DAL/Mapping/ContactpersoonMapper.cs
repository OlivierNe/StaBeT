using StageBeheersTool.Models.Domain;
using System.Data.Entity.ModelConfiguration;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class ContactpersoonMapper : EntityTypeConfiguration<Contactpersoon>
    {
        public ContactpersoonMapper()
        {
            ToTable("Contactpersonen");
            Property(cp => cp.Id).HasColumnName("Contactpersoon_id");
            Property(cp => cp.Email).HasMaxLength(50);
            Property(cp => cp.Voornaam).HasMaxLength(20);
            Property(cp => cp.Familienaam).HasMaxLength(30);
            Property(cp => cp.Aanspreektitel).HasMaxLength(20);
            Property(cp => cp.Postcode).HasMaxLength(15);
            Property(cp => cp.Gemeente).HasMaxLength(30);
            Property(cp => cp.Straat).HasMaxLength(50);
            Property(cp => cp.Telefoon).HasMaxLength(20);
            Property(cp => cp.Gsm).HasMaxLength(20);
            Property(cp => cp.Familienaam).IsRequired().HasMaxLength(50);
            Property(cp => cp.Bedrijfsfunctie).HasMaxLength(50);
        }
    }
}
