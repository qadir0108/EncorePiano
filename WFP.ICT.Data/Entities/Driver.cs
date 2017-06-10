using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Driver : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }
        public Guid? DefaultVehicleID { get; set; }

        public string FCMToken { get; set; }

        public virtual ICollection<PianoConsignment> Consignments { get; set; }

        public Driver()
        {
            Consignments = new HashSet<PianoConsignment>();
        }
    }
}
