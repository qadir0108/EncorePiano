using System;
using WFP.ICT.Data.Entities;

namespace WFP.ICT.Web.Async
{
    public class EmailThreadParams
    {
        public Guid? idFirst { get; set; }
        public Guid? idSecond { get; set; }
        public WFPUser user { get; set; }

        public string UploadPath { set; get; }
    }
}