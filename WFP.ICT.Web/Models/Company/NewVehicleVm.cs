using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WFP.ICT.Web.Models
{
    public class NewVehicleVm
    {
        [Required(ErrorMessage = "Code is required")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        public string Description { get; set; }
        public bool Active { get; set; }

        [Required(ErrorMessage = "VehicleType is required")]
        public string VehicleTypeId { get; set; }

        public Guid? Id { get; set; }

        public NewVehicleVm()
        {
        }
    }
}