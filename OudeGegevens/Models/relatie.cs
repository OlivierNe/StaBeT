using System.ComponentModel.DataAnnotations.Schema;

namespace OudeGegevens.Models
{
    public class relatie
    {
        public int relatieID { get; set; }
        public string naam { get; set; }
        public string voornaam { get; set; }
        public string functie { get; set; }
        public string tel { get; set; }
        public string email { get; set; }
        public int stagebedrijfID { get; set; }
        public string aanspreektitel { get; set; }
        public string dienst { get; set; }
        public string internadres { get; set; }
        public string GSMnummer { get; set; }

        [ForeignKey("stagebedrijfID")]
        public virtual stagebedrijf stagebedrijf { get; set; }
    }
}
