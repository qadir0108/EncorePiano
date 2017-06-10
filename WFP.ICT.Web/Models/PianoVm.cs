using System;
using System.Collections.Generic;
using WFP.ICT.Enum;

namespace WFP.ICT.Web.Models
{
    public class PianoVm
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }

        public string PianoTypeId { get; set; }
        public string PianoType { get; set; }
        public string PianoName { get; set; }
        public string PianoColor { get; set; }
        public string PianoModel { get; set; }
        public string PianoMake { get; set; }
        public string SerialNumber { get; set; }
        public string Notes { get; set; }
        public bool IsBench { get; set; } // W/B OR N/B
        public bool IsBoxed { get; set; }
        public bool IsStairs { get; set; }
        
    }
}