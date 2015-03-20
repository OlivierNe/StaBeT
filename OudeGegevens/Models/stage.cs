using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OudeGegevens.Models
{
    public class stage
    {
        public stage()
        {
            studenten = new List<student>();
        }

        public int stageID { get; set; }
        public string titel { get; set; }
        public string omschrijving { get; set; }
        public string typeStage { get; set; }
        public Nullable<int> programmeertaal { get; set; }
        public Nullable<int> databaseUsed { get; set; }
        public Nullable<int> operatingSysteem { get; set; }
        public string softwareOmgeving { get; set; }
        public string hardwareOmgeving { get; set; }
        public Nullable<short> aantalStudenten { get; set; }
        public Nullable<int> stagebedrijfID { get; set; }
        public string straat { get; set; }
        public string pc { get; set; }
        public string gemeente { get; set; }
        public Nullable<int> mentor { get; set; }
        public string docentID { get; set; }
        public Nullable<int> contractondertekenaar { get; set; }
        public string opmerking { get; set; }
        public Nullable<bool> aangebracht_door_studenten { get; set; }
        public string statusContract { get; set; }
        public string statusOpdracht { get; set; }
        public string dagPresentatie { get; set; }
        public string uurPresentatie { get; set; }
        public string lokaal { get; set; }
        public string bijzitter { get; set; }
        public string acjaar { get; set; }
        
        [ForeignKey("databaseUsed")]
        public virtual database database { get; set; }

        [ForeignKey("docentID")]
        public virtual docent docent { get; set; }

        [ForeignKey("operatingSysteem")]
        public virtual operatingsystem operatingsystem { get; set; }

        [ForeignKey("programmeertaal")]
        public virtual programmeertaal programmeertaal1 { get; set; }

        [ForeignKey("stagebedrijfID")]
        public virtual stagebedrijf stagebedrijf { get; set; }

        [ForeignKey("mentor")]
        public virtual relatie relatie { get; set; }

        [ForeignKey("contractondertekenaar")]
        public virtual relatie relatie1 { get; set; }

        public virtual ICollection<student> studenten { get; set; }
    
    }
}
