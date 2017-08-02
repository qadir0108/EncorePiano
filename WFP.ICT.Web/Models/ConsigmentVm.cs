using System;
using System.Collections.Generic;
using WFP.ICT.Enum;

namespace WFP.ICT.Web.Models
{
    public class ConsigmentVm
    {
        public Guid Id { get; set; }
        public string OrderId { get; set; }
        public string CreatedAt { get; set; }
        public string ConsignmentNumber { get; set; }
        public string OrderNumber { get; set; }

        public string StartWarehouseName { get; set; }
        public string VehicleName { get; set; }
        public string DriverName { get; set; }

        public Guid? WarehouseId { get; set; }
        public Guid? VehicleId { get; set; }
        public Guid? DriverId { get; set; }
        public Guid? Warehouses { get; set; }
        public Guid? Vehicles { get; set; }
        public List<Guid?> Drivers { get; set; }
        public Guid? Orders { get; set; }
        public string Paths { get; set; }
    }
}