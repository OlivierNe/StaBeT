using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{
    public class Student : Persoon
    {
        private string _fotoUrl;

        public string HogentEmail { get; set; }
        public string Keuzevak { get; set; }
        public string FotoUrl
        {
            get
            {
                return _fotoUrl ?? "~/Images/Student/profiel.jpg";
            }
            set
            {
                _fotoUrl = value;
            }
        }


    }
}