using StageBeheersTool.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;


namespace StageBeheersTool.Models.DAL.Mapping
{
    public class BegeleiderMapper : EntityTypeConfiguration<Begeleider>
    {
        public BegeleiderMapper()
        {
            ToTable("Begeleiders");
            Property(begeleider => begeleider.Id).HasColumnName("begeleider_id");
            Property(begeleider => begeleider.HogentEmail).IsRequired().HasMaxLength(100)
               .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("HogentEmailIndex") { IsUnique = true }));
            Property(begeleider => begeleider.Email).HasMaxLength(50);
            Property(begeleider => begeleider.Familienaam).HasMaxLength(30);
            Property(begeleider => begeleider.Voornaam).HasMaxLength(20);
            Property(begeleider => begeleider.Telefoon).HasMaxLength(20);
            Property(begeleider => begeleider.Gsm).HasMaxLength(20);
            Property(begeleider => begeleider.Gemeente).HasMaxLength(30);
            Property(begeleider => begeleider.Straat).HasMaxLength(50);
            Property(begeleider => begeleider.Postcode).HasMaxLength(15);

            HasOptional(begeleider => begeleider.Foto).WithOptionalDependent().WillCascadeOnDelete(true);
            HasMany(begeleider => begeleider.Stageopdrachten).WithOptional(stageopdracht => stageopdracht.Stagebegeleider);
            HasMany(begeleider => begeleider.StageAanvragen).WithRequired(aanvraag => aanvraag.Begeleider).WillCascadeOnDelete(true);
        }
    }
}