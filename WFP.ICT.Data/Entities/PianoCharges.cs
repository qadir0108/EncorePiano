using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class PianoCharges : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public int ChargesCode { get; set; }
        public int ChargesType { get; set; } // ChargesTypeEnum
        public string ChargesDetails { get; set; }
        public long Amount { get; set; }

        public PianoCharges()
        {
            
        }
    }
}
