using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{
    public class StageBegeleidAanvraag
    {
        public int Id { get; set; }
        public virtual Begeleider Begeleider { get; set; }
        public virtual Stageopdracht Stageopdracht { get; set; }
        public BegeleidAanvraagStatus Status { get; set; }
        public bool IsGoedgekeurd
        {
            get
            {
                return Status == BegeleidAanvraagStatus.Goedgekeurd;
            }
        }
        public bool IsAfgekeurd
        {
            get
            {
                return Status == BegeleidAanvraagStatus.Afgekeurd;
            }
        }

        public StageBegeleidAanvraag()
        {
            Status = BegeleidAanvraagStatus.NietBeoordeeld;
        }
    }
}