using System;
using System.Collections.Generic;

namespace OudeGegevens.Models
{
    public class docent
    {
        public docent()
        {
            this.stage = new HashSet<stage>();
        }

        public string docentID { get; set; }
        public string naam { get; set; }
        public string voornaam { get; set; }
        public string straat { get; set; }
        public string pc { get; set; }
        public string gemeente { get; set; }
        public string telefoon { get; set; }
        public string email { get; set; }
        public Nullable<float> aantalStageUren { get; set; }
        public string aanspreking { get; set; }
        public Nullable<short> aantaleffstages { get; set; }
        public string wachtwoord { get; set; }

        public virtual ICollection<stage> stage { get; set; }

    }
}
