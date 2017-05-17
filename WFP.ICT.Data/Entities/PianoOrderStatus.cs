using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class PianoOrderStatus : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public int OrderStatus { get; set; } // OrderStatusEnum

        public Guid? PianoOrderId { get; set; }
        public virtual PianoOrder PianoOrder { get; set; }

        public PianoOrderStatus()
        {
            
        }
    }
}
