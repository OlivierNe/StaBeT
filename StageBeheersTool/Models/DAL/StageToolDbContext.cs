using Microsoft.AspNet.Identity.EntityFramework;
using MySql.Data.Entity;
using System.Data.Entity;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.Models.DAL.Mapping;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Web.Mvc;
using System.Diagnostics;
using StageBeheersTool.Models.Identity;

namespace StageBeheersTool.Models.DAL
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class StageToolDbContext : IdentityDbContext<ApplicationUser>
    {
        public StageToolDbContext()
            //: base("OnlineConnection", throwIfV1Schema: false)
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.Log = message => Trace.WriteLine(message);
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
            modelBuilder.Configurations.Add(new StagebegeleidingAanvraagMapper());
            modelBuilder.Configurations.Add(new AcademiejaarInstellingenMapper());
            modelBuilder.Configurations.Add(new InstellingenMapper());
            modelBuilder.Configurations.Add(new StageMapper());
            modelBuilder.Configurations.Add(new VoorkeurStageMapper());
            modelBuilder.Configurations.Add(new ActiviteitsverslagMapper());
            modelBuilder.Configurations.Add(new EvaluatievraagMapper());
            modelBuilder.Configurations.Add(new EvaluatieantwoordMapper());

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<IdentityRole>()
                .Property(c => c.Name).HasMaxLength(128).IsRequired();
            modelBuilder.Entity<ApplicationUser>().ToTable("aspNetUsers")
                .Property(c => c.UserName).HasMaxLength(128).IsRequired();
            modelBuilder.Entity<ApplicationUser>().HasMany(u => u.Roles).WithRequired().HasForeignKey(r => r.UserId).WillCascadeOnDelete(true);
            modelBuilder.Entity<ApplicationUser>().HasMany(u => u.Claims).WithRequired().HasForeignKey(c => c.UserId).WillCascadeOnDelete(true);
            //modelBuilder.Entity<IdentityUserRole>().ToTable("aspNetUsers");
            //.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UserNameIndex") { IsUnique = false }));

        }

        public DbSet<Begeleider> Begeleiders { get; set; }
        public DbSet<Student> Studenten { get; set; }
        public DbSet<Bedrijf> Bedrijven { get; set; }
        public DbSet<Contactpersoon> Contactpersonen { get; set; }

        public DbSet<Stageopdracht> Stageopdrachten { get; set; }
        public DbSet<Stage> Stages { get; set; }
        public DbSet<VoorkeurStage> StudentVoorkeurStages { get; set; }
        public DbSet<StagebegeleidingAanvraag> StageBegeleidAanvragen { get; set; }

        public DbSet<Specialisatie> Specialisaties { get; set; }
        public DbSet<Keuzepakket> Keuzepakketten { get; set; }

        public DbSet<AcademiejaarInstellingen> AcademiejarenInstellingen { get; set; }
        public DbSet<Instelling> Instellingen { get; set; }

        public DbSet<Evaluatievraag> Evaluatievragen { get; set; }

        public static StageToolDbContext Create()
        {
            return DependencyResolver.Current.GetService(typeof(StageToolDbContext)) as StageToolDbContext;
        }

    }

}