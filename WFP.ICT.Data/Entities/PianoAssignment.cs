namespace WFP.ICT.Data.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class PianoAssignment : BaseEntity, iBaseEntity
    {
        public Guid Id { get; set; }
        public string AssignmentNumber { get; set; }

        public Guid? WarehouseStartId { get; set; }
        public virtual Warehouse WarehouseStart { get; set; }

        public Guid? VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }

        public Guid? DriverId { get; set; }
        public virtual Driver Driver { get; set; }

        public Guid? PianoOrderId { get; set; }
        //public virtual PianoOrder PianoOrder { get; set; }
        
        public Guid? PianoPodId { get; set; }
        public virtual PianoPOD PianoPod { get; set; }

        public string PickupTicket { get; set; }
        public DateTime? PickupTicketGenerationTime { get; set; }

        public long EstimatedTime { get; set; } // In Minutes
        public bool isStarted { get; set; }
        public DateTime? StartTime { get; set; }
        public int MinutesAway { get; set; } //  truck is on its way 15/30/45/60 minutes away

        public virtual ICollection<PianoAssignmentRoute> Route { get; set; }

        public PianoAssignment()
        {
            Route = new HashSet<PianoAssignmentRoute>();
        }
    }
}
