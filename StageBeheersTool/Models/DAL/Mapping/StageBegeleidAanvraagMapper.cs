﻿using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.DAL.Mapping
{
    public class StageBegeleidAanvraagMapper : EntityTypeConfiguration<StageBegeleidAanvraag>
    {
        public StageBegeleidAanvraagMapper()
        {
            this.HasRequired(sba => sba.Stageopdracht);
            this.HasRequired(sba => sba.Begeleider);
        }
    }
}