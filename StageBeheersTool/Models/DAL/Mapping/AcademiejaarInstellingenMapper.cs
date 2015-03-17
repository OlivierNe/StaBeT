using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class AcademiejaarInstellingenMapper : EntityTypeConfiguration<AcademiejaarInstellingen>
    {
        public AcademiejaarInstellingenMapper()
        {
            this.HasKey(aj => aj.Academiejaar);
        }
    }
}