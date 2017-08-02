using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class PianoFinish : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public String Code { get; set; }

        public string Name { get; set; }

        public PianoFinish()
        {
        }
    }
}