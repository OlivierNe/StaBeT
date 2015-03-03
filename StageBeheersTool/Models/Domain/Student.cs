using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{
    public class Student : Persoon
    {
        #region Private Fields
        private string _fotoUrl;
        #endregion

        #region Properties
        public string HogentEmail { get; set; }
        public virtual Keuzepakket Keuzepakket { get; set; }
        public string FotoUrl
        {
            get
            {
                return _fotoUrl ?? "~/Images/Student/profiel.jpg";
            }
            set
            {
                _fotoUrl = value;
            }
        }
        public virtual ICollection<Stageopdracht> MijnStageopdrachten { get; set; }
        #endregion

        #region Constructors
        public Student()
        {
            MijnStageopdrachten = new List<Stageopdracht>();
        }
        #endregion

        #region Public Methods
        public void AddStageopdracht(Stageopdracht stageopdracht)
        {
            MijnStageopdrachten.Add(stageopdracht);
        }

        public Stageopdracht FindStageopdracht(int id)
        {
            return MijnStageopdrachten.FirstOrDefault(so => so.Id == id);
        }

        public bool HeeftStageopdracht(int id)
        {
            return FindStageopdracht(id) != null;
        }

        public bool RemoveStageopdracht(Stageopdracht stageopdracht)
        {
            return MijnStageopdrachten.Remove(stageopdracht);
        }
        #endregion
    }
}