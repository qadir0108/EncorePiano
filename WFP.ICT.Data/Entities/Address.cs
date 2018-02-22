namespace WFP.ICT.Data.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Address : BaseEntity, iBaseEntity
    {

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternateContact { get; set; }
        public string AlternatePhone { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string State { get; set; }
        public int NumberStairs { get; set; }
        public int NumberTurns { get; set; }
        public string Notes { get; set; }

        public string Lat { get; set; }
        public string Lng { get; set; }
        public int AddressType { get; set; }
        public Guid? ClientId { get; set; }
        //  public virtual Client Client { get; set; }

        public Guid? WarehouseId { get; set; }
        //public virtual Warehouse Warehouse { get; set; }
    }
}
