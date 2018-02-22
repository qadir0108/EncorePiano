namespace WFP.ICT.Data.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Assignment : BaseEntity, iBaseEntity
    {
        public Guid Id { get; set; }
        public string AssignmentNumber { get; set; }
        
        public Guid? VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }

        public string PickupTicket { get; set; }
        public DateTime? PickupTicketGenerationTime { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EstimatedTime { get; set; } // In Minutes
        public int MinutesAway { get; set; } //  truck is on its way 15/30/45/60 minutes away

        public Guid? OrderId { get; set; }
        public virtual Order Order { get; set; }

        public Guid? LegId { get; set; }
        public virtual Leg Leg { get; set; }

        public virtual ICollection<AssignmentRoute> Route { get; set; }
        public virtual ICollection<Driver> Drivers { get; set; }
        public virtual ICollection<Proof> Proofs { get; set; }
        public virtual ICollection<TripStatus> Statuses { get; set; }

        public Assignment()
        {
            Route = new HashSet<AssignmentRoute>();
            Drivers = new HashSet<Driver>();
            Proofs = new HashSet<Proof>();
            Statuses = new HashSet<TripStatus>();
        }
    }
}
