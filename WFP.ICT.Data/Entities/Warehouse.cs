using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class Warehouse : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string AlternateContact { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternatePhone { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string State { get; set; }
        public string Notes { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }

        public virtual ICollection<Piano> Inventory { get; set; }

        public Warehouse()
        {
            Inventory = new HashSet<Piano>();
        }
    }
}
