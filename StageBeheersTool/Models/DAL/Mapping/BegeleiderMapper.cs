using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class BegeleiderMapper : EntityTypeConfiguration<Begeleider>
    {
        public BegeleiderMapper()
        {
            this.ToTable("Begeleiders");
            this.Property(b => b.HogentEmail).IsRequired().HasMaxLength(200)
                .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("HogentEmailIndex") { IsUnique = true }));
        }
    }
}