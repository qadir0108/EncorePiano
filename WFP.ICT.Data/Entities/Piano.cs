using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class Piano : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string SerialNumber { get; set; }
        public string Name { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public bool IsBench { get; set; } // W/B OR N/B
        public bool IsBoxed { get; set; }

        public string Notes { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? ShippedDate { get; set; }

        public Guid? PianoTypeId { get; set; }
        public PianoType PianoType { get; set; }

        public Guid? PianoStatusId { get; set; }
        public virtual PianoStatus PianoStatus { get; set; }

        public bool IsLocatedInThirdParty { get; set; }
        public Guid? WarehouseId { get; set; } // if in Warehouse, if not in warehouse np
        public virtual Warehouse Warehouse { get; set; }

        public Guid? OrderId { get; set; }
        public virtual PianoOrder Order { get; set; }

        public virtual ICollection<PianoPicture> Pictures { get; set; }

        public Piano()
        {
            Pictures = new HashSet<PianoPicture>();
        }
    }
}
