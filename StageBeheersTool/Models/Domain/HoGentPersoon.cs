using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{

    public abstract class HoGentPersoon : Persoon
    {
        #region Private Fields
        private string _fotoUrl;
        #endregion

        #region Properties
        public string HogentEmail { get; set; }
        public string FotoUrl
        {
            get
            {
                return _fotoUrl ?? "~/Images/profiel.jpg";
            }
            set
            {
                _fotoUrl = value;
            }
        }
        #endregion



    }
}