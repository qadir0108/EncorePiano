using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class OrderBilling : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public int PaymentOption { get; set; } // PaymentOptionEnum
        public int BillingType { get; set; } // BillingTypeEnum

        public long Amount { get; set; }

        public Guid? OrderId { get; set; }
        //public virtual Order Order { get; set; }

        public OrderBilling()
        {
            
        }
    }
}
