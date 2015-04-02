
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


        protected bool Equals(Contactpersoon other)
        {
            return string.Equals(other.Id, Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Contactpersoon)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }


    }


}