using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StageBeheersTool.Models.Domain
{
    public class Begeleider : Persoon
    {
        public string HogentEmail { get; set; }
    }
}