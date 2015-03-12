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
        public virtual Stageopdracht Stageopdracht { get; set; }

        #endregion

        #region Constructors
        public Student()
        {
            VoorkeurStages = new List<Stageopdracht>();
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
        #endregion
    }
}