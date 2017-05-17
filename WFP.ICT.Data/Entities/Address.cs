﻿namespace WFP.ICT.Data.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Address : BaseEntity, iBaseEntity
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string PhoneNumber { get; set; }
        
        public Guid? AddressTypeId { get; set; }
        public virtual AddressType AddressType { get; set; }

        public Guid? CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}