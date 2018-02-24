using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WFP.ICT.Web.Models
{
    public class NewWarehouseVm
    {
        [Required(ErrorMessage = "Code is required")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }

        public string AlternateContact { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternatePhone { get; set; }
        public string Address1 { get; set; }
        public string PostCode { get; set; }
        public string Notes { get; set; }

        public Guid? Id { get; set; }

        public NewWarehouseVm()
        {
        }
    }
}