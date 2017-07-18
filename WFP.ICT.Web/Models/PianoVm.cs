using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WFP.ICT.Enum;

namespace WFP.ICT.Web.Models
{
    public class PianoVm
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }

        public string PianoTypeId { get; set; }
        [Required(ErrorMessage = "Piano type is required")]
        public string PianoType { get; set; }
        public string PianoName { get; set; }

        [Required(ErrorMessage ="Piano category is required")]
        public string PianoCategoryType { get; set; }

        [Required(ErrorMessage = "Piano size is required")]
        public string PianoSize { get; set; }
        public string PianoColor { get; set; }
        [Required(ErrorMessage = "Piano model is required")]
        public string PianoModel { get; set; }
        [Required(ErrorMessage = "Piano make is required")]
        public string PianoMake { get; set; }

        public string SerialNumber { get; set; }

        public string WarehouseSerialNumber { get; set; }
        public string Notes { get; set; }
        public bool IsBench { get; set; } // W/B OR N/B
        public bool IsBoxed { get; set; }

        public bool IsPlayer { get; set; }
        public bool IsStairs { get; set; }
        
    }
}