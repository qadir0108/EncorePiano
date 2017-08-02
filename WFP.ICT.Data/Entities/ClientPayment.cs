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
        public DateTime PaymentDate { get; set; }
        public long Amount { get; set; }

        public Guid? CustomerId { get; set; }
        //public virtual Customer Customer { get; set; }

        public ClientPayment()
        {
        
        }
    }
}
