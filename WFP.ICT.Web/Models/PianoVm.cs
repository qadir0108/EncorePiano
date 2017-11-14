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

        //[Required(ErrorMessage = "Piano type is required")]
        public string PianoTypeId { get; set; }

        public string PianoType { get; set; }
        public string PianoName { get; set; }

        //[Required(ErrorMessage ="Piano category is required")]
        public string PianoCategoryType { get; set; }

        //[Required(ErrorMessage = "Piano size is required")]
        public string PianoSize { get; set; }
        public string PianoColor { get; set; }

        //[Required(ErrorMessage = "Piano model is required")]
        public string PianoModel { get; set; }

        //[Required(ErrorMessage = "Piano make is required")]
        public string PianoMake { get; set; }

        //[Required(ErrorMessage = "Piano finish is required")]
        public string PianoFinish { get; set; }

        public string SerialNumber { get; set; }

        public string WarehouseSerialNumber { get; set; }
        public string Notes { get; set; }
        public bool IsBench { get; set; } // W/B OR N/B
        public bool IsPlayer { get; set; }
        public bool IsBoxed { get; set; }

        public string IsMainUnitLoaded { get; set; }
        public string AdditionalBenchesStatus { get; set; } // AdditionalItemStatus
        public string AdditionalCasterCupsStatus { get; set; } // AdditionalItemStatus
        public string AdditionalCoverStatus { get; set; } // AdditionalItemStatus
        public string AdditionalOwnersManualStatus { get; set; } // AdditionalItemStatus
        public string AdditionalLampStatus { get; set; } // AdditionalItemStatus
        public string LoadTimeStamp { get; set; }

    }
}