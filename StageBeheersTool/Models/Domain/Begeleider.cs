using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{
    public class Begeleider : Persoon
    {
        #region private fields
        private string _fotoUrl;
        #endregion

        #region Properties
        public string HogentEmail { get; set; }
        public string FotoUrl
        {
            get
            {
                return _fotoUrl ?? "~/Images/Begeleider/profiel.jpg";
            }
            set
            {
                _fotoUrl = value;
            }
        }
        public virtual ICollection<Stageopdracht> MijnStageopdrachten { get; set; }
        #endregion

        #region Public Constructors
        public Begeleider()
        {
            MijnStageopdrachten = new List<Stageopdracht>();
        }

        #endregion

        #region Public methods
        public void AddStageopdracht(Stageopdracht stageopdracht)
        {
            MijnStageopdrachten.Add(stageopdracht);
        }

        public void RemoveStageopdracht(Stageopdracht stageopdracht)
        {
            MijnStageopdrachten.Remove(stageopdracht);
        }

        public Stageopdracht FindStageopdracht(int id)
        {
            return MijnStageopdrachten.FirstOrDefault(so => so.Id == id);
        }

        public bool HeeftStageopdracht(int id)
        {
            return FindStageopdracht(id) != null;
        }
        #endregion

    }
}

