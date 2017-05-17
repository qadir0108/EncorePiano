using System.Collections.Generic;
using System.Data;

namespace WFP.ICT.Web.Reports
{
    public sealed class ReportModel
    {
        public DataSet DataSet { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        public ReportModel()
        {
            Parameters = new Dictionary<string, object>();
        }
    }
}