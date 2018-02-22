using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class PianoQuote : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string QuoteNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public bool IsStairs { get; set; }
        public PianoType PianoType { get; set; }
        public string Notes { get; set; }

        public Guid? PickupAddressId { get; set; }
        public virtual Address PickupAddress { get; set; }

        public Guid? DeliveryAddressId { get; set; }
        public virtual Address DeliveryAddress { get; set; }

        public Guid? ClientId { get; set; } // If it is dealer/manfcaturer
        public virtual Client Client { get; set; }

        public Guid? OrderBillingId { get; set; } // when it will be billed to client
        public virtual OrderBilling Billing { get; set; }

        public virtual ICollection<Piano> Items { get; set; }

        public PianoQuote()
        {
            Items = new HashSet<Piano>();
        }
    }
}
