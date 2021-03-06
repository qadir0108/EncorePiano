﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class Order : BaseEntity, iBaseEntity
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public int OrderType { get; set; } // OrderTypeEnum
        public string CallerFirstName { get; set; }
        public string CallerLastName { get; set; }
        public string CallerPhoneNumber { get; set; }
        public string CallerEmail { get; set; }
        public string CallerAlternatePhone{ get; set; }

        public int PaymentOption { get; set; } // PaymentOptionEnum

        public string SalesOrderNumber { get; set; } // For Corporate client
        
        public DateTime? PickupDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public Guid? PickupAddressId { get; set; }
        public virtual Address PickupAddress { get; set; }

        public Guid? DeliveryAddressId { get; set; }
        public virtual Address DeliveryAddress { get; set; }

        public Guid? ClientId { get; set; }
        public virtual Client Client { get; set; }

        public Guid? OrderBillingId { get; set; } // when it will be billed to client
        public virtual OrderBilling Billing { get; set; }

        public string DeliveryForm { get; set; }
        public double CodAmount { get; set; }
        public string OfficeStaff { get; set; }
        public string OnlinePayment { get; set; }

        public string PickUpNotes { get; set; }
        public string DeliveryNotes { get; set; }

        public string CarriedBy { get; set; }

        public bool BillToDifferent { get; set; }
        public Guid? InvoiceClientId { get; set; }
        public virtual Client InvoiceClient { get; set; }
        public Guid? InvoiceBillingPartyId { get; set; }
        public virtual Client InvoiceBillingParty { get; set; }

        public virtual ICollection<Piano> Pianos { get; set; }
        public virtual ICollection<Leg> Legs { get; set; }
        public virtual ICollection<OrderCharges> OrderCharges { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }

        public Order()
        {
            Pianos = new HashSet<Piano>();
            Legs = new HashSet<Leg>();
            OrderCharges = new HashSet<OrderCharges>();
            Assignments = new HashSet<Assignment>();
        }
    }
}
