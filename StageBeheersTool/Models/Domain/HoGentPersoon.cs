using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{

    public abstract class HoGentPersoon : Persoon
    {
        #region Private Fields
        private string _fotoUrl;
        #endregion

        #region Properties
        public string HogentEmail { get; set; }
        public string FotoUrl
        {
            get
            {
                return _fotoUrl ?? "~/Images/profiel.jpg";
            }
            set
            {
                _fotoUrl = value;
            }
        }
        public virtual ICollection<Stageopdracht> VoorkeurStages { get; set; }

        #endregion

        #region Constructor
        public HoGentPersoon()
        {
            VoorkeurStages = new List<Stageopdracht>();
        }
        #endregion

        #region Public methods
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