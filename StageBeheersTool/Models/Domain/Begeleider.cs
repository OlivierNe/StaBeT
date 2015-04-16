using System.Collections.Generic;
using System.Linq;

namespace StageBeheersTool.Models.Domain
{
    public class Begeleider : HoGentPersoon
    {

        #region Properties
        public string Aanspreking { get; set; }
        public virtual ICollection<Stageopdracht> Stageopdrachten { get; set; }
        public virtual ICollection<StagebegeleidingAanvraag> StageAanvragen { get; set; }
        #endregion

        #region Public Constructors
        public Begeleider()
        {
            Stageopdrachten = new List<Stageopdracht>();
            StageAanvragen = new List<StagebegeleidingAanvraag>();
        }

        #endregion

        #region Public methods
        public Stageopdracht FindStage(int id)
        {
            return Stageopdrachten.SingleOrDefault(so => so.Id == id);
        }


        public void AddAanvraag(Stageopdracht stageopdracht)
        {
            if (!HeeftStageBegeleidingAangevraagd(stageopdracht))
            {
                var aanvraag = new StagebegeleidingAanvraag
                {
                    Stage = stageopdracht,
                    Begeleider = this
                };
                StageAanvragen.Add(aanvraag);
            }
        }

        public bool HeeftStageBegeleidingAangevraagd(Stageopdracht stageopdracht)
        {
            return StageAanvragen.Any(sa => sa.Stage.Id == stageopdracht.Id);
        }

        public void AanvraagAnnuleren(Stageopdracht stageopdracht)
        {
            StageAanvragen.Remove(StageAanvragen.SingleOrDefault(sa => sa.Stage.Id == stageopdracht.Id));
        }

        public StagebegeleidingAanvraag FindAanvraag(Stageopdracht stageopdracht)
        {
            return StageAanvragen.SingleOrDefault(sa => sa.Stage == stageopdracht);
        }

        public bool BegeleidStage(Stageopdracht stageopdracht)
        {
            return Stageopdrachten.Any(so => so.Id == stageopdracht.Id);
        }


        public bool MagAanvraagAnnuleren(Stageopdracht stageopdracht)
        {
            return HeeftStageBegeleidingAangevraagd(stageopdracht) &&
                   stageopdracht.IsGoedgekeurd() && stageopdracht.HeeftStageBegeleider() == false;
        }

        public bool MagAanvraagIndienen(Stageopdracht stageopdracht)
        {
            return !HeeftStageBegeleidingAangevraagd(stageopdracht) && stageopdracht.IsToegewezen()
                    && stageopdracht.HeeftStageBegeleider() == false;
        }
        
        public bool MagStageopdrachtWijzigen(Stageopdracht stageopdracht)
        {
            return stageopdracht.Stagebegeleider != null && this.Equals(stageopdracht.Stagebegeleider);
        }

        protected bool Equals(Begeleider other)
        {
            return string.Equals(other.HogentEmail, HogentEmail);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Begeleider)obj);
        }

        public override int GetHashCode()
        {
            return (HogentEmail != null ? HogentEmail.GetHashCode() : 0);
        }

        #endregion
      
    }
}

