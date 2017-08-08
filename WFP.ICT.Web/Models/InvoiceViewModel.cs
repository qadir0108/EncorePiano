using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WFP.ICT.Web.Models
{
    public class InvoiceViewModel
    {
        [Required]
        public string Client { get; set; }
        [Required]
        public DateTime? StartDate { get; set; }
        [Required]
        public DateTime? EndDate{ get; set; }

    }

    public class BatchInvoiceViewModel
    {
        public string OrderNumber { get; set; }
        public string Status { get; set; }
        public string RecievedBy { get; set; }
        public string Signature { get; set; }

        public string RecieveDate { get; set; }

        public string Notes { get; set; }

        public string POD { get; set; }

        public string Pictures { get; set; }

    }
}