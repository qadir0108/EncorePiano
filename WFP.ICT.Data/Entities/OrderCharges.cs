﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class OrderCharges : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public long Amount { get; set; }
        public int ServiceStatus { get; set; } // ServiceStatusEnum

        public Guid? PianoChargesId { get; set; }
        public virtual PianoCharges PianoCharges { get; set; }

        public Guid? OrderId { get; set; }
        public virtual Order Order { get; set; }

        public OrderCharges()
        {

        }
    }
}