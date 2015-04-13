using StageBeheersTool.Models.Domain;
using System.Data.Entity.ModelConfiguration;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class StageopdrachtMapper : EntityTypeConfiguration<Stageopdracht>
    {
        public StageopdrachtMapper()
        {
            ToTable("Stageopdrachten");
            Property(so => so.Id).HasColumnName("stageopdracht_id");
            Property(so => so.Titel).IsRequired().HasMaxLength(250);
            Property(so => so.Omschrijving).IsRequired();
            Property(so => so.Academiejaar).IsRequired().HasMaxLength(9);
            Property(so => so.Gemeente).HasMaxLength(30);
            Property(so => so.Straat).HasMaxLength(50);
            Property(so => so.Specialisatie).HasMaxLength(50);
            Property(so => so.ContractondertekenaarNaam).HasMaxLength(50);
            Property(so => so.ContractondertekenaarEmail).HasMaxLength(50);
            Property(so => so.StagementorNaam).HasMaxLength(50);
            Property(so => so.StagementorEmail).HasMaxLength(50);
            Property(so => so.Gemeente).HasMaxLength(30);
            Property(so => so.Straat).HasMaxLength(50);
            Property(so => so.Postcode).HasMaxLength(15);

            HasOptional(so => so.Stagementor).WithMany().WillCascadeOnDelete(false);
            HasOptional(so => so.Contractondertekenaar).WithMany().WillCascadeOnDelete(false);

        }
    }
}