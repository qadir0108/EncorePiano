using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class PianoCharges : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public int ServiceCode { get; set; }
        public int ServiceType { get; set; } // ServiceTypeEnum
        public int ServiceStatus { get; set; } // ServiceStatusEnum
        public string ServiceDetails { get; set; }
        public long ServiceCharges { get; set; }

        public Guid? PianoOrderId { get; set; }
        public virtual PianoOrder PianoOrder { get; set; }

        public PianoCharges()
        {
            
        }
    }
}
