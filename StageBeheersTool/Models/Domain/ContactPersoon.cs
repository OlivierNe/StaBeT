using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{/*
  * De informatie over een contactpersoon bevat minimaal: naam - 
  * voornaam - e-mail - gsm - functie binnen het bedrijf - aanspreektitel 
  * - functie stageopdracht (mentor-contractondertekenaar.
  * */
    public class Contactpersoon : Persoon
    {
        public string Aanspreektitel { get; set; }
        public bool IsStagementor { get; set; }
        public bool IsContractOndertekenaar { get; set; }
        public string Bedrijfsfunctie { get; set; }


    }

    
}