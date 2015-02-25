using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{
    /*
     * contactpersoon: naam - voornaam - e-mail - gsm - functie binnen het bedrijf - aanspreektitel - functie stageopdracht (mentor-contractondertekenaar.
     * */
    /*
     * student: naam voornaam - adres - telefoon - gsm
     * */

    public abstract class Persoon
    {
        public int Id { get; set; }
        public string Familienaam { get; set; }
        public string Voornaam { get; set; }
        public string Telefoonnummer { get; set; }
        public string Email { get; set; }
        public string Gsmnummer { get; set; }
        public string Gemeente { get; set; }
        public string Postcode { get; set; }
        public string Straat { get; set; }
        public string Straatnummer { get; set; }

        public string Naam
        {
            get
            {
                return Familienaam + " " + Voornaam;
            }
        }


    }
}