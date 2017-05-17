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

        public virtual ICollection<PianoPicture> Pictures { get; set; }

        public PianoPOD()
        {
            Pictures = new HashSet<PianoPicture>();
        }
    }
}
