using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class PianoPicture : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public int PictureType { get; set; } // PictureTypeEnum
        public string Picture { get; set; }
        
        public Guid? PianoId { get; set; }
        public virtual Piano Piano { get; set; }

        public Guid? PianoPodId { get; set; }
        public virtual PianoPOD PianoPod { get; set; }
        
        public PianoPicture()
        {
        }
    }
}
