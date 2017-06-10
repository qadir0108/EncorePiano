namespace WFP.ICT.Data.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class APIRequestLog : BaseEntity, iBaseEntity
    {
        public Guid Id { get; set; }
        public int RequestType { get; set; } // RequestTypeEnum
        public string Request { get; set; }

    }
}
