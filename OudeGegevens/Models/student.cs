using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OudeGegevens.Models
{
    public class student
    {
        public string studentID { get; set; }
        public string naam { get; set; }
        public string voornaam { get; set; }
        public string straat { get; set; }
        public string pc { get; set; }
        public string gemeente { get; set; }
        public string telefoon { get; set; }
        public string GSM { get; set; }
        public string email { get; set; }
        public string email2 { get; set; }
        public Nullable<int> stageID { get; set; }
        public string gebdatum { get; set; }
        public string gebplaats { get; set; }
        public string keuzepakket { get; set; }
        public string acjaar { get; set; }

        [ForeignKey("stageID")]
        public virtual stage stage { get; set; }
    }
}
