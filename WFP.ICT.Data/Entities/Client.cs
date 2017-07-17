using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    using System;
    using System.Collections.Generic;
    using Enum;

    public partial class Client : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public System.Guid Id { get; set; }

        public string AccountCode { get; set; }
        public string CompanyLogo { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Comment { get; set; }

        public Guid? CustomerInvoiceId { get; set; }
        public virtual ICollection<CustomerInvoice> Invoice { get; set; }

        public Guid? CustomerPaymentId { get; set; }
        public virtual ICollection<CustomerPayment> Payment { get; set; }

        public int CustomerType { get; set; }

        public virtual Address Addresses { get; set; }
        public virtual ICollection<PianoOrder> Orders { get; set; }

        public Client()
        {
            Payment = new HashSet<CustomerPayment>();
            Invoice = new HashSet<CustomerInvoice>();
            Orders = new HashSet<PianoOrder>();
        }
    }
}
