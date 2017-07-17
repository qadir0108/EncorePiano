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

        public int OrderType { get; set; } // OrderTypeEnum
        public int OrderMedium { get; set; } // OrderMediumEnum
        public string CallerFirstName { get; set; }
        public string CallerLastName { get; set; }
        public string CallerPhoneNumber { get; set; }
        public string CallerEmail { get; set; }
        
        public int PaymentOption { get; set; } // PaymentOptionEnum

        public string SalesOrderNumber { get; set; } // For Corporate client
        
        public string Notes { get; set; }

        //public DateTime? OrderDate { get; set; } // CreatedAt

        public DateTime? PreferredPickupDateTime { get; set; }

        public DateTime? PickupDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public Guid? PickupAddressId { get; set; }
        public virtual Address PickupAddress { get; set; }

        public Guid? DeliveryAddressId { get; set; }
        public virtual Address DeliveryAddress { get; set; }

        public Guid? CustomerId { get; set; }
        public virtual Client Customer { get; set; }

        public Guid? PianoOrderBillingId { get; set; } // when it will be billed to customer
        public virtual PianoOrderBilling Billing { get; set; }

        public Guid? PianoConsignmentId { get; set; } // When it is send to driver's vehicle
        public virtual PianoConsignment PianoConsignment { get; set; }

        public virtual ICollection<Piano> Pianos { get; set; }
        public virtual ICollection<PianoCharges> PianoCharges { get; set; }
        public virtual ICollection<PianoOrderStatus> Statuses { get; set; }
        public double CodAmount { get; set; }
        public string OfficeStaff { get; set; }
        public string OfficePayment { get; set; }

        public bool BillToDifferent { get; set; }
        public Guid? InvoiceClientId { get; set; }
        public virtual Client InvoiceClient { get; set; }
        public Guid? InvoiceBillingPartyId { get; set; }
        public virtual Client InvoiceBillingParty { get; set; }
        public Guid? ShuttleCompanyId { get; set; }
        public virtual Client ShuttleCompany { get; set; }
        public PianoOrder()
        {
            Pianos = new HashSet<Piano>();
            PianoCharges = new HashSet<PianoCharges>();
            Statuses = new HashSet<PianoOrderStatus>();
        }
    }
}
