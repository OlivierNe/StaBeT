using System;

namespace StageBeheersTool.Models.Domain
{
    public class Admin
    {
        public static void KeurStageBegeleidingAanvraagGoed(StagebegeleidingAanvraag aanvraag)
        {
            if (aanvraag.Stage.Stagebegeleider != null)
            {
                throw new ApplicationException(string.Format(Resources.ErrorStagebegeleidingAanvraagGoedkeuren, 
                    aanvraag.Stage.Titel, aanvraag.Stage.Stagebegeleider.Naam));
            }
            aanvraag.Status = StagebegeleidAanvraagStatus.Goedgekeurd;
            aanvraag.Stage.Stagebegeleider = aanvraag.Begeleider;
        }

        public static string KeurStageBegeleidAanvraagAf(StagebegeleidingAanvraag aanvraag)
        {
            aanvraag.Status = StagebegeleidAanvraagStatus.Afgekeurd;
            if (aanvraag.Begeleider.Equals(aanvraag.Stage.Stagebegeleider))
            {
                aanvraag.Stage.Stagebegeleider = null;
                return string.Format(Resources.SuccesStagebegeleidingAfgekeurdBegeleiderLosgekoppeld,
                    aanvraag.Begeleider.Naam, aanvraag.Stage.Titel);
            }
            return Resources.SuccesStagebegeleidingAfgekeurd;
        }

        public static void KeurStageopdrachtGoed(Stageopdracht stageopdracht)
        {
            stageopdracht.Status = StageopdrachtStatus.Goedgekeurd;
        }

        public static void KeurStageopdrachtAf(Stageopdracht stageopdracht)
        {
            stageopdracht.Status = StageopdrachtStatus.Afgekeurd;
            stageopdracht.Stagebegeleider = null;
        }

        public static bool KeurStagedossierGoed(VoorkeurStage voorkeurStage)
        {
            voorkeurStage.Status = StagedossierStatus.Goedgekeurd;
            return true;
        }

        public static bool KeurStagedossierAf(VoorkeurStage voorkeurStage)
        {
            voorkeurStage.Status = StagedossierStatus.Afgekeurd;
            return true;
        }

        public static Stage KoppelStageopdrachtAanStudent(VoorkeurStage studentVoorkeurstage)
        {
            if (studentVoorkeurstage.HeeftGoedgekeurdStagedossier() == false)
            {
                throw new ApplicationException(Resources.ErrorStageAanStudentKoppelenZonderGoedgekeurdStagedossier);
            }
            if (studentVoorkeurstage.Student.HeeftToegewezenStage())
            {
                throw new ApplicationException(Resources.ErrorStudentHeeftAlToegewezenStage);
            }
            var student = studentVoorkeurstage.Student;
            var stageopdracht = studentVoorkeurstage.Stageopdracht;
            var stage = new Stage(stageopdracht, student);
            student.Stages.Add(stage);
            return stage;
        }

    }
}