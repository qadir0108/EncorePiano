using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class ClientPayment : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public int PaymentType { get; set; } // PaymentTypeEnum
        public string CheckNumber { get; set; }
        public DateTime PaymentDate { get; set; } // TransactionDate
        public long Amount { get; set; }

        // For Credit Card payment
        public string TransactionNumber { get; set; }

        public Guid? ClientId { get; set; } //
        public virtual Client Client { get; set; }

        public Guid? ClientInvoiceId { get; set; }
        public Guid? OrderId { get; set; }

        public ClientPayment()
        {
        
        }
    }
}
