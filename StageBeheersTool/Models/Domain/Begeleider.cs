﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{
    public class Begeleider : HoGentPersoon
    {

        #region Properties
        /// <summary>
        /// stages begeleid door deze begeleider
        /// </summary>
        public virtual ICollection<Stageopdracht> Stages { get; set; }
        public virtual ICollection<StageBegeleidAanvraag> StageAanvragen { get; set; }
        #endregion

        #region Public Constructors
        public Begeleider()
            : base()
        {
            Stages = new List<Stageopdracht>();
            StageAanvragen = new List<StageBegeleidAanvraag>();
        }

        #endregion

        #region Public methods
        public Stageopdracht FindStage(int id)
        {
            return Stages.SingleOrDefault(so => so.Id == id);
        }
        #endregion


        public void AddAanvraag(Stageopdracht stageopdracht)
        {
            if (!HeeftStageBegeleidingAangevraagd(stageopdracht.Id))
            {
                StageBegeleidAanvraag aanvraag = new StageBegeleidAanvraag()
                {
                    Stageopdracht = stageopdracht,
                    Begeleider = this
                };
                StageAanvragen.Add(aanvraag);
            }
        }
        /// <param name="id">Id Stageopdracht</param>
        public bool HeeftStageBegeleidingAangevraagd(int id)
        {
            return StageAanvragen.Any(sa => sa.Stageopdracht.Id == id);
        }

        public void AanvraagAnnuleren(Stageopdracht stageopdracht)
        {
            StageAanvragen.Remove(StageAanvragen.SingleOrDefault(sa => sa.Stageopdracht.Id == stageopdracht.Id));
        }

        public StageBegeleidAanvraag FindAanvraag(Stageopdracht stageopdracht)
        {
            return StageAanvragen.SingleOrDefault(sa => sa.Stageopdracht == stageopdracht);
        }

        public bool BegeleidStage(Stageopdracht stageopdracht)
        {
            return Stages.Any(so => so.Id == stageopdracht.Id);
        }
    }
}

