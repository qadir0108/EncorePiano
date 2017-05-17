using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace WFP.ICT.Data.Entities
{
    public class WFPRole : IdentityRole
    {
        public WFPRole() : base() { }
        public WFPRole(string name) : base(name) { }
        public WFPRole(string name, bool isDeletable, bool isEditable, string description = "") : base(name) { Description = description; IsDeletable = isDeletable; IsEditable = isEditable; }

        public string Description { get; set; }
        public bool IsEditable { get; set; }
        public bool IsDeletable { get; set; }

        public virtual ICollection<AspNetRoleClaims> RoleClaims { get; set; }

    }
}
