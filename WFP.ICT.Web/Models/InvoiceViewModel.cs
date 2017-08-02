using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WFP.ICT.Web.Models
{
    public class InvoiceViewModel
    {
        public string Client { get; set; }
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate{ get; set; }

    }
}