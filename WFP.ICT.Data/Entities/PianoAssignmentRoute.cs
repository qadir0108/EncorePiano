using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class PianoAssignmentRoute : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string Lat { get; set; }
        public string Lng { get; set; }
        public int Order { get; set; }

        public Guid? PianoAssignmentId { get; set; }
        public virtual PianoAssignment PianoAssignment { get; set; }

        public PianoAssignmentRoute()
        {
        }
    }
}
