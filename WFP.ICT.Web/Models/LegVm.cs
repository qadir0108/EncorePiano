using System;
using System.ComponentModel.DataAnnotations;

namespace WFP.ICT.Web.Models
{
    public class LegVm
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }

        [Required(ErrorMessage = "Leg Number required")]
        public int LegNumber { get; set; }

        [Required(ErrorMessage = "Leg Type is required")]
        public string LegTypeId { get; set; }

        [Required(ErrorMessage = "From location is required")]
        public string FromLocationId { get; set; }
        public string FromLocation { get; set; }

        [Required(ErrorMessage = "To location is required")]
        public string ToLocationId { get; set; }
        public string ToLocation { get; set; }

        [Required(ErrorMessage ="Driver is required")]
        public string DriverId { get; set; }

        [Required(ErrorMessage = "Leg date is required")]
        public DateTime? LegDate { get; set; }
    }
}