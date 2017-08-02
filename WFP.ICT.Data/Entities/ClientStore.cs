namespace WFP.ICT.Data.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class ClientStore : BaseEntity, iBaseEntity
    {

        public Guid Id { get; set; }
        public string Name { get; set; }
        public String Code { get; set; }

        public Guid? AddressId { get; set; }
        public Address Address { get; set; }
     
    }
}
