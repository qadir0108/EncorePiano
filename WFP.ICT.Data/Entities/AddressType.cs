﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WFP.ICT.Data.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class AddressType : BaseEntity, iBaseEntity
    {
        public AddressType()
        {
            this.Addresses = new HashSet<Address>();
        }
    
        public System.Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<Address> Addresses { get; set; }
    }
}
