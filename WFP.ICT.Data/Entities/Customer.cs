using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Customer : BaseEntity, iBaseEntity
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
        public virtual CustomerInvoice Invoice { get; set; }

        public Guid? CustomerPaymentId { get; set; }
        public virtual CustomerPayment Payment { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<PianoOrder> Orders { get; set; }

        public Customer()
        {
            Addresses = new HashSet<Address>();
            Orders = new HashSet<PianoOrder>();
        }
    }
}
