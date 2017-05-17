using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class PianoConsignmentForm : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string ConsignmentForm { get; set; }
        public DateTime? SignedOn { get; set; }
        public string AcknowledgeTo { get; set; } // Email addresses

        public Guid? PianoConsignmentId { get; set; }
        //public virtual PianoConsignment PianoConsignment { get; set; }

        public PianoConsignmentForm()
        {
        }
    }
}
