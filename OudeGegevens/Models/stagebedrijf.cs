using System;
using System.Collections.Generic;

namespace OudeGegevens.Models
{
    public class stagebedrijf
    {
        public stagebedrijf()
        {
            this.relatie = new HashSet<relatie>();
            this.stage = new HashSet<stage>();
        }

        public int stagebedrijfID { get; set; }
        public string naam { get; set; }
        public string straat { get; set; }
        public string pc { get; set; }
        public string gemeente { get; set; }
        public string landCode { get; set; }
        public string tel { get; set; }
        public string fax { get; set; }
        public string website { get; set; }
        public string sector { get; set; }
        public string bereikbaarheid { get; set; }
        public bool actief { get; set; }
        public string categorie { get; set; }
        public string opm { get; set; }
        public bool sollicitatie { get; set; }
        public bool via_website { get; set; }
        public string waardering { get; set; }
        public Nullable<decimal> @long { get; set; }
        public Nullable<decimal> lat { get; set; }

        public virtual ICollection<relatie> relatie { get; set; }
        public virtual ICollection<stage> stage { get; set; }
    }
}
