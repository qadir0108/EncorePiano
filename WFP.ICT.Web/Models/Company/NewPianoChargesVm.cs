using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WFP.ICT.Web.Models
{
    public class NewPianoChargesVm
    {
        [Required(ErrorMessage = "Code is required")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Details is required")]
        public string Details { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        public long Amount { get; set; }

        public Guid? Id { get; set; }
        public NewPianoChargesVm()
        {
        }
    }
}