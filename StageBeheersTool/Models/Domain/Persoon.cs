
namespace StageBeheersTool.Models.Domain
{

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
                var naam = Familienaam + " " + Voornaam;
                if (string.IsNullOrWhiteSpace(naam))
                {
                    return "/";
                }
                return naam;
            }
        }

    }
}