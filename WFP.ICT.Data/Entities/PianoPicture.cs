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
        
        /// <summary>
        /// For Wharehouse it will be USED
        /// </summary>
        public Guid? PianoId { get; set; }
        public virtual Piano Piano { get; set; }

        /// <summary>
        /// For Assignment Proof it will be USED
        /// </summary>
        public Guid? ProofId { get; set; }
        public virtual Proof Proof { get; set; }
        
        public PianoPicture()
        {
        }
    }
}
