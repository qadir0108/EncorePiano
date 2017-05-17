using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFP.ICT.Data.Entities
{
    public class AspNetRoleClaims : BaseEntity, iBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid ClaimID { get; set; }
        public virtual AspNetClaims Claim { get; set; }

        public string RoleID { get; set; }
        public virtual WFPRole Role { get; set; }

        public AspNetRoleClaims()
        {
        }
    }
}
