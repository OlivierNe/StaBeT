using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{
    public class Admin
    {
        public static void KeurStageBegeleidAanvraagGoed(StageBegeleidAanvraag aanvraag)
        {
            aanvraag.Status = BegeleidAanvraagStatus.Goedgekeurd;
            aanvraag.Stageopdracht.Stagebegeleider = aanvraag.Begeleider;
        }
        public static void KeurStageBegeleidAanvraagAf(StageBegeleidAanvraag aanvraag)
        {
            aanvraag.Status = BegeleidAanvraagStatus.Afgekeurd;
        }

        public static void KeurStageopdrachtGoed(Stageopdracht stageopdracht)
        {
            stageopdracht.Status = StageopdrachtStatus.Goedgekeurd;
        }

        public static void KeurStageopdrachtAf(Stageopdracht stageopdracht)
        {
            stageopdracht.Status = StageopdrachtStatus.Afgekeurd;
        }
    }
}