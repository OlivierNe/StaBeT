using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{
    //Het bedrijf moet bij registratie volgende informatie ingeven: 
    //Bedrijfsnaam – adres – url - e-mail (aanmeldingsaccount) – telefoon – 
    //bereikbaarheid (wagen – openbaar vervoer – georganiseerd vervoer door bedrijf) 
    //– soort bedrijfsactiviteiten (bank – software ontwikkelaar – openbare diensten ….)
    //Na registratie ontvangt het bedrijf een bevestigings e-mail met gegenereerd wachtwoord 
    //voor de eerste aanmelding. Bij de eerste aanmelding dient een nieuw wachtwoord te worden ingegeven.
    public class Bedrijf
    {
        public int Id { get; set; }
        public string Email { get; set; } //aanmeldingsaccount
        public string Naam { get; set; } //bedrijfsnaam
        public string Gemeente { get; set; }
        public string Postcode { get; set; }
        public string Straat { get; set; }
        public int Straatnummer { get; set; }
        public string Telefoonnummer { get; set; }
        public string Bereikbaarheid { get; set; } //(wagen – openbaar vervoer – georganiseerd vervoer door bedrijf) 
        public string BedrijfsActiviteiten { get; set; } //(bank – software ontwikkelaar – openbare diensten ….)


    }
}