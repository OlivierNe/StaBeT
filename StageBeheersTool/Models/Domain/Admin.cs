
namespace StageBeheersTool.Models.Domain
{
    public class Admin
    {
        public static bool KeurStageBegeleidingAanvraagGoed(StagebegeleidingAanvraag aanvraag)
        {
            if (aanvraag.Stage.Stagebegeleider != null)
            {
                return false;
            }
            aanvraag.Status = StagebegeleidAanvraagStatus.Goedgekeurd;
            aanvraag.Stage.Stagebegeleider = aanvraag.Begeleider;
            return true;
        }

        public static bool KeurStageBegeleidAanvraagAf(StagebegeleidingAanvraag aanvraag)
        {
            aanvraag.Status = StagebegeleidAanvraagStatus.Afgekeurd;
            if (aanvraag.Begeleider.Equals(aanvraag.Stage.Stagebegeleider))
            {
                aanvraag.Stage.Stagebegeleider = null;
                return false;
            }
            return true;
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