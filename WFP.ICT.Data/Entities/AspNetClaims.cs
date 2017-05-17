using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class AspNetClaims : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        public virtual ICollection<AspNetRoleClaims> RoleClaims { get; set; }

        public AspNetClaims()
        {
        }
    }
}
