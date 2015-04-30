
namespace StageBeheersTool.Models.Domain
{

    public abstract class HoGentPersoon : Persoon
    {
        #region Properties
        public string HogentEmail { get; set; }
       // public string FotoUrl { get; set; }
        public virtual Foto Foto { get; set; }

        public new string Naam
        {
            get
            {
                var naam = Familienaam + " " + Voornaam;
                if (string.IsNullOrWhiteSpace(naam))
                {
                    return HogentEmail;
                }
                return naam;
            }
        }
        #endregion
    }
}