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
            this.ToTable("Begeleiders");
            this.Property(begeleider => begeleider.HogentEmail).IsRequired().HasMaxLength(200)
               .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("HogentEmailIndex") { IsUnique = true }));
            this.HasMany(b => b.Stages).WithOptional(so => so.Stagebegeleider);
            this.HasMany(b => b.StageAanvragen).WithRequired(x => x.Begeleider).WillCascadeOnDelete(true);
        }
    }
}