using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.DAL
{
    public class BegeleiderRepository : IBegeleiderRepository
    {
        private DbSet<Begeleider> begeleiders;
        private StageToolDbContext ctx;

        public BegeleiderRepository(StageToolDbContext ctx)
        {
            this.ctx = ctx;
            this.begeleiders = ctx.Begeleiders;
        }

        public Begeleider FindByEmail(string hoGentEmail)
        {
            return begeleiders.FirstOrDefault(b => b.HogentEmail == hoGentEmail);
        }


        public void SaveChanges()
        {
            ctx.SaveChanges();
        }

        public void Update(Begeleider begeleider, Begeleider newBegeleider)
        {
            begeleider.Voornaam = newBegeleider.Voornaam;
            begeleider.Familienaam = newBegeleider.Familienaam;
            begeleider.Email = newBegeleider.Email;
            begeleider.Gsmnummer = newBegeleider.Gsmnummer;
            begeleider.Telefoonnummer = newBegeleider.Telefoonnummer;
            begeleider.Postcode = newBegeleider.Postcode;
            begeleider.Gemeente = newBegeleider.Gemeente;
            begeleider.Straat = newBegeleider.Straat;
            begeleider.Straatnummer = newBegeleider.Straatnummer;
            begeleider.FotoUrl = newBegeleider.FotoUrl;
        }

        public void Add(Begeleider begeleider)
        {
            begeleiders.Add(begeleider);
        }
    }
}