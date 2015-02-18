using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{
    //Een stage-opdracht bevat minstens volgende gegevens: titel, omschrijving opdracht,
    //specialisatie stageopdracht (programmeren, webontwikkeling, mainframe, e-business,
    //mobile, systeembeheer..), sem 1 of sem2, aantal studenten opdracht, stagementor,
    //contractondertekenaar. De stage coördinator, de administratie  studentensecretariaat
    //en het bedrijf dient een e-mail te ontvangen met de gepaste tekst over de (wijzigingen) stage-opdracht.
    public class Stageopdracht
    {
        public int Id { get; set; }
        public string Titel { get; set; }
        public string Omschrijving { get; set; }
        public virtual Specialisatie Specialisatie { get; set; }
        public int Semester { get; set; }
        public int AantalStudenten { get; set; }
        public string Academiejaar { get; set; }
        public StagecontractOndertekenaar ContractOndertekenaar { get; set; }
        public Stagementor Stagementor { get; set; }


    }
}