using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class PianoCharges : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string Code { get; set; }
        public string Details { get; set; } // ChargesTypeEnum for data seed only 
        public long Amount { get; set; }

        public PianoCharges()
        {
            
        }
    }
}
