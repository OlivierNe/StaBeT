using System.Linq;

namespace StageBeheersTool.Models.Domain
{
    public class Evaluatievraag
    {

        public int Id { get; set; }
        public string Vraag { get; set; }
        public SoortVraag SoortVraag { get; set; }
        public int Stagebezoek { get; set; }
        public string Voor { get; set; }
        public string MeerkeuzeAntwoorden { get; set; }
        public int Volgorde { get; set; }

        public Evaluatievraag()
        {
            SoortVraag = SoortVraag.Openvraag;
        }

        public bool IsGeldigAntwoord(string antwoord)
        {
            if (antwoord == null) return true;

            switch (SoortVraag)
            {
                case SoortVraag.JaNeevraag:
                    return antwoord.ToLower() == "ja" || antwoord.ToLower() == "nee";
                case SoortVraag.Meerkeuzevraag:
                    string[] antwoorden = MeerkeuzeAntwoorden.Split(';');
                    return antwoorden.Contains(antwoord);
            }
            return true;
        }

        protected bool Equals(Evaluatievraag other)
        {
            return string.Equals(Vraag, other.Vraag) && SoortVraag == other.SoortVraag
                && Stagebezoek == other.Stagebezoek && string.Equals(Voor, other.Voor) && Volgorde == other.Volgorde;
        }

        public override int GetHashCode()
        {
            int hashCode = (Vraag != null ? Vraag.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (int)SoortVraag;
            hashCode = (hashCode * 397) ^ Stagebezoek;
            hashCode = (hashCode * 397) ^ (Voor != null ? Voor.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ Volgorde;
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Evaluatievraag)obj);
        }

    }

    public enum SoortVraag
    {
        JaNeevraag, Openvraag, Meningvraag, Meerkeuzevraag
    }
}
