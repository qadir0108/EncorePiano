using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class PianoPOD : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string ReceivedBy { get; set; }
        public string Signature { get; set; }
        public DateTime? ReceivingTime { get; set; }
        public int PodStatus { get; set; } // PODStatusEnum
        public string Notes { get; set; }

        public string AssignmentForm { get; set; }
        public DateTime? SignedOn { get; set; }
        public string AcknowledgeTo { get; set; } // Email addresses

        public Guid? PianoAssignmentId { get; set; }
        //public virtual PianoAssignment PianoAssignment { get; set; }

        public Guid? PianoId { get; set; }
        public virtual Piano Piano { get; set; }

        public virtual ICollection<PianoPicture> Pictures { get; set; }

        public PianoPOD()
        {
            Pictures = new HashSet<PianoPicture>();
        }
    }
}
