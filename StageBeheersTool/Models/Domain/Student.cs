using System;
using System.Collections.Generic;
using System.Linq;

namespace StageBeheersTool.Models.Domain
{
    public class Student : HoGentPersoon
    {
        #region Properties

        public virtual Keuzepakket Keuzepakket { get; set; }
        public virtual ICollection<VoorkeurStage> VoorkeurStages { get; set; }

        //collection in geval student niet geslaagd is voor stage en volgend jaar nog eens moet doen
        public virtual ICollection<Stage> Stages { get; set; }

        public Stageopdracht ToegewezenStageopdracht
        {
            get
            {
                var mijnStageopdracht = Stages.SingleOrDefault(s => s.Stageopdracht.IsInHuidigAcademiejaar());
                return mijnStageopdracht == null ? null : mijnStageopdracht.Stageopdracht;
            }
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
            VoorkeurStages.Add(new VoorkeurStage(this, stageopdracht));
        }

        public Stageopdracht FindVoorkeurStageopdracht(int id)
        {
            return VoorkeurStages.Select(s => s.Stageopdracht).SingleOrDefault(so => so.Id == id);
        }

        public IEnumerable<Stageopdracht> GetAllVoorkeurStages()
        {
            return VoorkeurStages.Select(voorkeur => voorkeur.Stageopdracht);
        }

        public bool HeeftStageopdrachtAlsVoorkeur(int id)
        {
            return FindVoorkeurStageopdracht(id) != null;
        }

        public bool RemoveVoorkeurStage(Stageopdracht stageopdracht)
        {
            var voorkeur = VoorkeurStages.SingleOrDefault(s => s.Stageopdracht.Id == stageopdracht.Id);
            return VoorkeurStages.Remove(voorkeur);
        }

        public Stageopdracht FindGekozenVoorkeurStage()
        {
            return VoorkeurStages.Where(s => s.StagedossierIngediend).Select(s => s.Stageopdracht).SingleOrDefault();
        }

        //public void RemoveVoorkeurStages(int[] ids)
        //{
        //    foreach (var id in ids)
        //    {
        //        var stageopdracht = FindVoorkeurStageopdracht(id);
        //        if (stageopdracht != null)
        //        {
        //            RemoveVoorkeurStage(stageopdracht);
        //        }
        //    }
        //}

        public bool MagStageopdrachtBekijken(Stageopdracht stageopdracht)
        {
            if (Stages.Any(str => str.StageopdrachtId == stageopdracht.Id))
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