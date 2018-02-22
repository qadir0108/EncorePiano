using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class AssignmentRoute : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string Lat { get; set; }
        public string Lng { get; set; }
        public int Order { get; set; }

        public Guid? AssignmentId { get; set; }
        public virtual Assignment Assignment { get; set; }

        public AssignmentRoute()
        {
        }
    }
}
