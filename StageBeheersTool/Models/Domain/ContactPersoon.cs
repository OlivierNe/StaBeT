
namespace StageBeheersTool.Models.Domain
{
    public class Contactpersoon : Persoon
    {
        #region Properties
        public string Aanspreektitel { get; set; }
        public bool IsStagementor { get; set; }
        public bool IsContractondertekenaar { get; set; }
        public string Bedrijfsfunctie { get; set; }
        public virtual Bedrijf Bedrijf { get; set; }

        #endregion




    }


}