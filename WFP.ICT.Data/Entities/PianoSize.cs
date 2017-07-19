using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class PianoSize : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public double Width { get; set; }

        public Guid? PianoTypeId { get; set; }
        public virtual PianoType PianoType { get; set; }
        public PianoSize()
        {
        }
    }
}