using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class TripStatus : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public int Status { get; set; } // TripStatusEnum
        public DateTime? StatusTime { get; set; }
        public string StatusBy { get; set; }
        public string Comments { get; set; }

        public Guid? AssignmentId { get; set; }
        public virtual Assignment Assignment { get; set; }

        public TripStatus()
        {
            
        }
    }
}
