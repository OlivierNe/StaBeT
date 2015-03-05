using System;
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
        #endregion

        #region Public Constructors
        public Begeleider()
            : base()
        {
            Stages = new List<Stageopdracht>();
        }

        #endregion

        #region Public methods
        public Stageopdracht FindStage(int id)
        {
            return Stages.SingleOrDefault(so => so.Id == id);
        }
        #endregion

    }
}

