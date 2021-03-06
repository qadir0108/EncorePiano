﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class Piano : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public int PianoCategoryType { get; set; } // PianoCategoryTypeEnum
        public string SerialNumber { get; set; }
        public string Model { get; set; }
        public bool IsBench { get; set; } // W/B OR N/B
        public bool IsBoxed { get; set; }
        public bool IsPlayer { get; set; }
        
        public string Notes { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? ShippedDate { get; set; }

        public Guid? PianoTypeId { get; set; }
        public PianoType PianoType { get; set; }
        public Guid? PianoMakeId { get; set; }
        public PianoMake PianoMake { get; set; }
        public Guid? PianoSizeId { get; set; }
        public PianoSize PianoSize { get; set; }
        public Guid? PianoFinishId { get; set; }
        public PianoFinish PianoFinish { get; set; }

        // Warehouse
        public string DrawingOfPiano { get; set; }
        public int WareHouseStatus { get; set; } // WareHouseStatusEnum
        public bool IsLocated { get; set; }
        public bool IsLocatedInThirdParty { get; set; }
        public Guid? WarehouseId { get; set; } // if in Warehouse, if not in warehouse np
        public virtual Warehouse Warehouse { get; set; }

        public Guid? ClientId { get; set; } // if in Warehouse, if not in warehouse np
        public virtual Client Client { get; set; }
        public Guid? OrderId { get; set; }
        public virtual Order Order { get; set; }

        public virtual ICollection<PianoPicture> Pictures { get; set; }
        public virtual ICollection<PianoStatus> Statuses { get; set; }

        public Piano()
        {
            Pictures = new HashSet<PianoPicture>();
            Statuses = new HashSet<PianoStatus>();
        }
    }
}
