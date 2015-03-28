using MySql.Data.Entity;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using OudeGegevens.Models;

namespace OudeGegevens
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class OudeGegevensDbContext : DbContext
    {
        public OudeGegevensDbContext()
            : base("OudeGegevens")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //modelBuilder.Entity<stagebedrijf>().HasMany(b => b.stage).WithRequired(s => s.stagebedrijf).HasForeignKey(s => s.stagebedrijfID);
        }

        public DbSet<docent> Docenten { get; set; } 
        public DbSet<stage> Stages { get; set; }
        public DbSet<relatie> Relaties { get; set; }//contactpersonen
        public DbSet<student> Studenten { get; set; }
        public DbSet<stagebedrijf> Stagebedrijf { get; set; }

    }
}
