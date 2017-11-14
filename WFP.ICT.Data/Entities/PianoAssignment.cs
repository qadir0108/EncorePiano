namespace WFP.ICT.Data.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class PianoAssignment : BaseEntity, iBaseEntity
    {
        public Guid Id { get; set; }
        public string AssignmentNumber { get; set; }
        
        public Guid? VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }

        public Guid? PianoOrderId { get; set; }
        public virtual PianoOrder PianoOrder { get; set; }
        
        public string PickupTicket { get; set; }
        public DateTime? PickupTicketGenerationTime { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EstimatedTime { get; set; } // In Minutes
        public int MinutesAway { get; set; } //  truck is on its way 15/30/45/60 minutes away

        public virtual ICollection<PianoAssignmentRoute> Route { get; set; }
        public virtual ICollection<Driver> Drivers { get; set; }
        public virtual ICollection<PianoPOD> PODs { get; set; }
        public virtual ICollection<TripStatus> Statuses { get; set; }

        public PianoAssignment()
        {
            Route = new HashSet<PianoAssignmentRoute>();
            Drivers = new HashSet<Driver>();
            PODs = new HashSet<PianoPOD>();
            Statuses = new HashSet<TripStatus>();
        }
    }
}
