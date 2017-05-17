using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class CustomerInvoice : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public System.Guid Id { get; set; }

        public DateTime? SentOn { get; set; }
        public long Amount { get; set; }
        public int Status { get; set; } // InvoiceStatusEnum

        public Guid? CustomerId { get; set; }
        //public virtual Customer Customer { get; set; }

        public virtual ICollection<PianoOrder> Orders { get; set; }

        public CustomerInvoice()
        {
            Orders = new HashSet<PianoOrder>();
        }
    }
}
