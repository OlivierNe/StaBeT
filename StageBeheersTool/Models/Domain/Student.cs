using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{
    public class Student : HoGentPersoon
    {

        #region Properties
        public virtual Keuzepakket Keuzepakket { get; set; }
        //public virtual Begeleider Begeleider { get; set; }
        public virtual ICollection<Stageopdracht> VoorkeurStages { get; set; }
        public virtual ICollection<Stageopdracht> Stageopdrachten { get; set; }

        #endregion

        #region Constructors
        public Student()
        {
            VoorkeurStages = new List<Stageopdracht>();
            Stageopdrachten = new List<Stageopdracht>();
        }
        #endregion

        #region Public Methods
        public void AddVoorkeurStage(Stageopdracht stageopdracht)
        {
            VoorkeurStages.Add(stageopdracht);
        }

        public Stageopdracht FindVoorkeurStageopdracht(int id)
        {
            return VoorkeurStages.FirstOrDefault(so => so.Id == id);
        }

        public bool HeeftStageopdrachtAlsVoorkeur(int id)
        {
            return FindVoorkeurStageopdracht(id) != null;
        }

        public bool RemoveVoorkeurStage(Stageopdracht stageopdracht)
        {
            return VoorkeurStages.Remove(stageopdracht);
        }

        public void RemoveVoorkeurStages(int[] ids)
        {
            foreach (var id in ids)
            {
                var stageopdracht = FindVoorkeurStageopdracht(id);
                if (stageopdracht != null)
                {
                    RemoveVoorkeurStage(stageopdracht);
                }
            }
        }

        protected bool Equals(Student other)
        {
            return string.Equals(other.HogentEmail, HogentEmail);
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
            return (HogentEmail != null ? HogentEmail.GetHashCode() : 0);
        }

        #endregion
    }
}