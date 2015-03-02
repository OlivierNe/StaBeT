using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{
    public class Student : Persoon
    {
        private string _fotoUrl;

        public string HogentEmail { get; set; }
        public string Keuzevak { get; set; }
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

        public Student()
        {
            MijnStageopdrachten = new List<Stageopdracht>();
        }

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
    }
}