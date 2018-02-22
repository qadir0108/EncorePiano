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
        public string FCMToken { get; set; }

        public Guid? WarehouseId { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }

        public Driver()
        {
            Assignments = new HashSet<Assignment>();
        }
    }
}
