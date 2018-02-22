using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class ClientPaymentCard : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string CardDetails { get; set; }
        public Guid? ClientId { get; set; }

        public ClientPaymentCard()
        {
        
        }
    }
}
