
namespace StageBeheersTool.Models.Domain
{

    public abstract class Persoon
    {
        public int Id { get; set; }
        public string Familienaam { get; set; }
        public string Voornaam { get; set; }
        public string Telefoon { get; set; }
        public string Email { get; set; }
        public string Gsm { get; set; }
        public string Gemeente { get; set; }
        public string Postcode { get; set; }
        public string Straat { get; set; }

        public string Naam
        {
            get
            {
                var naam = Familienaam + " " + Voornaam;
                if (string.IsNullOrWhiteSpace(naam))
                {
                    return Email;
                }
                return naam;
            }
        }

    }
}