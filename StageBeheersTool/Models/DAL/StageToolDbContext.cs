using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MySql.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using StageBeheersTool.Models.Domain;
using StageBeheersTool.Models.DAL.Mapping;


namespace StageBeheersTool.Models.DAL
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class StageToolDbContext : IdentityDbContext<ApplicationUser>
    {
        public StageToolDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new BedrijfMapper());
            modelBuilder.Configurations.Add(new StageopdrachtMapper());
            modelBuilder.Configurations.Add(new SpecialisatieMapper());
            modelBuilder.Configurations.Add(new ContactpersoonMapper());

            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
            

            modelBuilder.Entity<IdentityRole>()
                .Property(c => c.Name).HasMaxLength(128).IsRequired();
            modelBuilder.Entity<ApplicationUser>().ToTable("aspNetUsers")
                .Property(c => c.UserName).HasMaxLength(128).IsRequired();
        }

        public DbSet<Bedrijf> Bedrijven { get; set; }
        public DbSet<Stageopdracht> Stageopdrachten { get; set; }
        public DbSet<Specialisatie> Specialisaties { get; set; }

        public static StageToolDbContext Create()
        {
            return new StageToolDbContext();
        }


    }

}