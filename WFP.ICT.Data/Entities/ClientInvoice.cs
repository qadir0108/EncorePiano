using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class ClientInvoice : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public System.Guid Id { get; set; }

        public string InvoiceNumber { get; set; }
        public DateTime? SentOn { get; set; }
        public long Amount { get; set; }
        public int Status { get; set; } // InvoiceStatusEnum

        public string Notes { get; set; }

        public DateTime GeneratedAt { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Guid? ClientId { get; set; }
        public virtual Client Client { get; set; }

        public ClientInvoice()
        {
        }
    }
}
