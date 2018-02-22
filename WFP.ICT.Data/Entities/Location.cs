using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace WFP.ICT.Data.Entities
{
    public partial class Location : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public Guid? ClientId { get; set; }

        public Guid? AddressId { get; set; }
        public Address Address { get; set; }

        public Location()
        {
        }
    }
}
