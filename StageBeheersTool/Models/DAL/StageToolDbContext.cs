using Microsoft.AspNet.Identity.EntityFramework;
using MySql.Data.Entity;
using System.Data.Entity;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.Models.DAL.Mapping;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Web.Mvc;
using System.Diagnostics;
using StageBeheersTool.Models.Authentication;

namespace StageBeheersTool.Models.DAL
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class StageToolDbContext : IdentityDbContext<ApplicationUser>
    {
        public StageToolDbContext()
            //: base("OnlineConnection", throwIfV1Schema: false)
            : base("DefaultConnection", throwIfV1Schema: false)
        {
#if DEBUG
            Database.Log = message => Trace.WriteLine(message);
#endif
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new BedrijfMapper());
            modelBuilder.Configurations.Add(new StageopdrachtMapper());
            modelBuilder.Configurations.Add(new SpecialisatieMapper());
            modelBuilder.Configurations.Add(new ContactpersoonMapper());
            modelBuilder.Configurations.Add(new StudentMapper());
            modelBuilder.Configurations.Add(new BegeleiderMapper());
            modelBuilder.Configurations.Add(new StageBegeleidAanvraagMapper());
            modelBuilder.Configurations.Add(new AcademiejaarInstellingenMapper());

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<IdentityRole>()
                .Property(c => c.Name).HasMaxLength(128).IsRequired();
            modelBuilder.Entity<ApplicationUser>().ToTable("aspNetUsers")
                .Property(c => c.UserName).HasMaxLength(128).IsRequired();
            //.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex") { IsUnique = false }));
        }

        public DbSet<Bedrijf> Bedrijven { get; set; }
        public DbSet<Stageopdracht> Stageopdrachten { get; set; }
        public DbSet<Specialisatie> Specialisaties { get; set; }
        public DbSet<Keuzepakket> Keuzepakketten { get; set; }
        public DbSet<Contactpersoon> Contactpersonen { get; set; }
        public DbSet<Begeleider> Begeleiders { get; set; }
        public DbSet<Student> Studenten { get; set; }
        public DbSet<StageBegeleidAanvraag> StageBegeleidAanvragen { get; set; }
        public DbSet<AcademiejaarInstellingen> AcademiejarenInstellingen { get; set; }

        public static StageToolDbContext Create()
        {
            return DependencyResolver.Current.GetService(typeof(StageToolDbContext)) as StageToolDbContext;
        }

    }

}