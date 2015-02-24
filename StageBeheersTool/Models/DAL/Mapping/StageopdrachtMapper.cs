using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class StageopdrachtMapper : EntityTypeConfiguration<Stageopdracht>
    {
        public StageopdrachtMapper()
        {
            this.ToTable("Stageopdrachten");
            this.Property(so => so.Titel).IsRequired().HasMaxLength(200);
            this.Property(so => so.Omschrijving).IsRequired();
            this.Property(so => so.Academiejaar).IsRequired();
            this.HasOptional(so => so.Stagementor).WithMany().WillCascadeOnDelete(false);
            this.HasOptional(so => so.ContractOndertekenaar).WithMany().WillCascadeOnDelete(false);
            this.HasOptional(so => so.Specialisatie);
        }
    }
}