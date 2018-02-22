using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class Leg : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public int LegNumber { get; set; }
        public int LegType { get; set; } // LegTypeEnum

        public Guid? FromLocationId { get; set; }
        public virtual Location FromLocation { get; set; }
        public Guid? ToLocationId { get; set; }
        public virtual Location ToLocation { get; set; }
        public Guid? DriverId { get; set; }
        public virtual Driver Driver { get; set; }
        public DateTime? LegDate { get; set; }

        public Guid? OrderId { get; set; }
        public virtual Order Order { get; set; }

        public Leg()
        {
        }
    }
}
