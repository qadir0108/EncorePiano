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
        public int ReceivingStatus { get; set; } // PODStatusEnum
        public string Notes { get; set; }

        public Guid? PianoConsignmentId { get; set; }
        public virtual PianoConsignment PianoConsignment { get; set; }

        public Guid? PianoId { get; set; }
        public virtual Piano Piano { get; set; }

        public virtual ICollection<PianoPicture> Pictures { get; set; }

        public PianoPOD()
        {
            Pictures = new HashSet<PianoPicture>();
        }
    }
}
