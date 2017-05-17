using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class PianoOrder : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }

        public int OrderMedium { get; set; } // OrderMediumEnum
        public int PaymentOption { get; set; } // PaymentOptionEnum

        public string SalesOrderNumber { get; set; } // For Corporate client

        public bool IsStairs { get; set; }
        public string Notes { get; set; }

        //public DateTime? OrderDate { get; set; } // CreatedAt

        public DateTime? PickupDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public Guid? PickupAddressId { get; set; }
        public virtual Address PickupAddress { get; set; }

        public Guid? DeliveryAddressId { get; set; }
        public virtual Address DeliveryAddress { get; set; }

        public Guid? CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public Guid? PianoOrderBillingId { get; set; } // when it will be billed to customer
        public virtual PianoOrderBilling Billing { get; set; }

        public Guid? PianoConsignmentId { get; set; } // When it is send to driver's vehicle
        public virtual PianoConsignment PianoConsignment { get; set; }

        public virtual ICollection<Piano> Items { get; set; }
        public virtual ICollection<PianoOrderStatus> Statuses { get; set; }
        public virtual ICollection<PianoService> Services { get; set; }

        public PianoOrder()
        {
            Items = new HashSet<Piano>();
            Statuses = new HashSet<PianoOrderStatus>();
        }
    }
}
