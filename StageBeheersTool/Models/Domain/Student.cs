using System;
using System.Collections.Generic;
using System.Linq;

namespace StageBeheersTool.Models.Domain
{
    public class Student : HoGentPersoon
    {
        #region Properties
        public string Academiejaar { get; set; }
        public string Geboorteplaats { get; set; }
        public DateTime? Geboortedatum { get; set; }

        public virtual Keuzepakket Keuzepakket { get; set; }
        public virtual ICollection<VoorkeurStage> VoorkeurStages { get; set; }

        //collection in geval de student niet geslaagd is voor de stage en volgend jaar nog eens moet doen
        public virtual ICollection<Stage> Stages { get; set; }

        public Stageopdracht ToegewezenStageopdracht
        {
            get { return Stage == null ? null : Stage.Stageopdracht; }
        }

        public Stage Stage
        {
            get
            {
                if (Stages.Count > 1)
                {
                    return Stages.FirstOrDefault(s => s.Stageopdracht.IsInHuidigAcademiejaar());
                }
                return Stages.FirstOrDefault();
            }
        }

        public string GeboortedatumToString
        {
            get { return Geboortedatum == null ? "" : ((DateTime)Geboortedatum).ToString("dd-MM-yyyy"); }
        }

        #endregion

        #region Constructors
        public Student()
        {
            VoorkeurStages = new List<VoorkeurStage>();
            Stages = new List<Stage>();
        }
        #endregion

        #region Public Methods
        public void AddVoorkeurStage(Stageopdracht stageopdracht)
        {
            if (HeeftStageopdrachtAlsVoorkeur(stageopdracht.Id))
            {
                throw new ApplicationException(string.Format(Resources.ErrorVoorkeurStageToevoegen, stageopdracht.Titel));
            }
            VoorkeurStages.Add(new VoorkeurStage(this, stageopdracht));
        }

        public Stageopdracht FindVoorkeurStageopdracht(int id)
        {
            return VoorkeurStages.Select(s => s.Stageopdracht).SingleOrDefault(so => so.Id == id);
        }

        public VoorkeurStage FindVoorkeurStage(Stageopdracht stageopdracht)
        {
            return VoorkeurStages.SingleOrDefault(voorkeur => voorkeur.Stageopdracht.Equals(stageopdracht));
        }

        public IEnumerable<Stageopdracht> GetAllVoorkeurStages()
        {
            return VoorkeurStages.Select(voorkeur => voorkeur.Stageopdracht);
        }

        public bool HeeftStageopdrachtAlsVoorkeur(int id)
        {
            return FindVoorkeurStageopdracht(id) != null;
        }

        public bool KanVoorkeurstageVerwijderen(Stageopdracht stageopdracht)
        {
            var voorkeurstage = FindVoorkeurStage(stageopdracht);
            if (voorkeurstage == null)
                return false;
            return voorkeurstage.StagedossierIngediend == false;
        }

        public void RemoveVoorkeurStage(Stageopdracht stageopdracht)
        {
            var voorkeur = VoorkeurStages
                .SingleOrDefault(voorkeurStage => voorkeurStage.Stageopdracht.Equals(stageopdracht));
            if (voorkeur == null)
            {
                throw new ApplicationException(Resources.ErrorVoorkeurVerwijderenNietGevonden);
            }
            if (voorkeur.StagedossierIngediend)
            {
                throw new ApplicationException(Resources.ErrorVoorkeurStageVerwijderen);
            }
            VoorkeurStages.Remove(voorkeur);
        }

        public Stageopdracht FindGekozenVoorkeurStage()
        {
            return VoorkeurStages
                .Where(voorkeurStage => voorkeurStage.StagedossierIngediend)
                .Select(voorkeurStage => voorkeurStage.Stageopdracht)
                .SingleOrDefault();
        }

        public bool MagStageopdrachtBekijken(Stageopdracht stageopdracht)
        {
            if (Stages.Any(stage => stage.Stageopdracht.Equals(stageopdracht)))
            {
                return true;
            }
            return stageopdracht.IsBeschikbaar();
        }

        public bool HeeftStagedossierIngediend()
        {
            return VoorkeurStages.Any(voorkeurstage => voorkeurstage.StagedossierIngediend);
        }

        public StagedossierStatus? GetStagedossierStatus()
        {
            var voorkeur = VoorkeurStages.SingleOrDefault(voorkeurstage => voorkeurstage.StagedossierIngediend);
            return voorkeur == null ? null : (StagedossierStatus?)voorkeur.Status;
        }

        public void SetStagedossierIngediend(Stageopdracht stageopdracht)
        {
            if (stageopdracht.IsBeschikbaar() == false)
            {
                throw new ApplicationException(Resources.ErrorStageNietMeerBeschikbaar);
            }
            if (HeeftStagedossierIngediend())
            {
                throw new ApplicationException(Resources.ErrorStagedossierReedsIngediend);
            }
            var voorkeur = VoorkeurStages.SingleOrDefault(s => s.Stageopdracht.Id == stageopdracht.Id);
            if (voorkeur == null)
            {
                throw new ApplicationException(Resources.ErrorStagedossierStageopdrachtNietAlsVoorkeur);
            }
            voorkeur.StagedossierIngediend = true;
        }

        public bool HeeftToegewezenStage()
        {
            return ToegewezenStageopdracht != null;
        }

        public Stage GetStageStudentRelatie()
        {
            return Stages.SingleOrDefault(s => s.Stageopdracht.IsInHuidigAcademiejaar());
        }

        protected bool Equals(Student other)
        {
            return string.Equals(other.Id, this.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Student)obj);
        }

        public override int GetHashCode()
        {
            return this.Id;
        }

        #endregion



    }
}