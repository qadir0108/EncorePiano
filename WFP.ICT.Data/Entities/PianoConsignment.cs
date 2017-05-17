namespace WFP.ICT.Data.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class PianoConsignment : BaseEntity, iBaseEntity
    {
        public Guid Id { get; set; }
        public string ConsignmentNumber { get; set; }

        public Guid? VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }

        public Guid? DriverId { get; set; }
        public virtual Driver Driver { get; set; }

        public Guid? PianoOrderId { get; set; }
        //public virtual PianoOrder PianoOrder { get; set; }

        public Guid? PianoPODId { get; set; }
        public virtual PianoPOD PianoPOD { get; set; }

        public Guid? PianoConsignmentFormId { get; set; }
        public PianoConsignmentForm PianoConsignmentForm { get; set; }

        public virtual ICollection<PianoConsignmentRoute> Route { get; set; }

        public long EstimatedTime { get; set; } // In Minutes
        public bool isStarted { get; set; }
        public DateTime? StartTime { get; set; }
        public int MinutesAway { get; set; } //  truck is on its way 15/30/45/60 minutes away

        public PianoConsignment()
        {
            Route = new HashSet<PianoConsignmentRoute>();
        }
    }
}
