using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{
    public class Student : HoGentPersoon
    {
       
        #region Properties
        public virtual Keuzepakket Keuzepakket { get; set; }
        //public virtual Begeleider Begeleider { get; set; }
        #endregion

        #region Constructors
        public Student() : base()
        {

        }
        #endregion

        #region Public Methods
      
        #endregion
    }
}