using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class Warehouse : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }

        public Guid? AddressId { get; set; }
        public virtual Address Address { get; set; }

        public virtual ICollection<Piano> Inventory { get; set; }

        public Warehouse()
        {
            Inventory = new HashSet<Piano>();
        }
    }
}
