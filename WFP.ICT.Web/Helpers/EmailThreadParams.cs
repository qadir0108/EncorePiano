using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WFP.ICT.Data.Entities;

namespace WFP.ICT.Web.Helpers
{
    public class EmailThreadParams
    {
        public Guid? id { get; set; }
        public WFPUser user { get; set; }

        public string UploadPath { set; get; }
    }
}