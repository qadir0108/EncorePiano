using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class PianoConsignmentRoute : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string Lat { get; set; }
        public string Lng { get; set; }
        public string Order { get; set; }

        public Guid? PianoConsignmentId { get; set; }
        public virtual PianoConsignment PianoConsignment { get; set; }

        public PianoConsignmentRoute()
        {
        }
    }
}
