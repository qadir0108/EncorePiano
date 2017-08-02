using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFP.ICT.Web.Models
{
    public class BatchLoadVm
    {
        public string Type { get; set; }
        public string Size { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Serial { get; set; }
        public string Bench { get; set; }
        public string Player { get; set; }
        public string Box { get; set; }
        public string Owner { get; set; }

    }
}