using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class PianoStatus : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public int Status { get; set; } // PianoStatusEnum
        public DateTime? StatusTime { get; set; }
        public string StatusBy { get; set; }
        public string Comments { get; set; }

        public string DrawingOfPiano { get; set; }

        public Guid? PianoId { get; set; }
        //public virtual Piano Piano { get; set; }

        public PianoStatus()
        {
            
        }
    }
}
