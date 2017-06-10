namespace WFP.ICT.Web.API.Models
{
    public enum RequestParamsEnum
    {
        Id
    }

    public class ConsignmentResponse
    {
        public string Id { get; set; }
        public string ConsignmentNumber { get; set; }
        public string StartWarehouseName { get; set; }
        public string StartWarehouseAddress { get; set; }
        public  string VehicleCode { get; set; }
        public string VehicleName { get; set; }
        public string DriverCode { get; set; }
        public string DriverName { get; set; }

        public string OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string OrderType { get; set; }
        public string OrderedAt { get; set; }
        public string CallerName { get; set; }
        public string CallerPhoneNumber { get; set; }
        public string SpecialInstructions { get; set; }
        public int NumberOfItems { get; set; }
        public string PickupAddress { get; set; }
        public string DeliveryAddress { get; set; }
        
        public PianoResponse[] Pianos { get; set; }
        public RouteResponse[] Route { get; set; }
    }

    public class PianoResponse
    {
        public string Id { get; set; }
        public string ConsignmentId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Model { get; set; }
        public string Make { get; set; }
        public string SerialNumber { get; set; }
        public int IsStairs { get; set; }
        public int IsBench { get; set; }
        public int IsBoxed { get; set; }
    }

    public class RouteResponse
    {
        public string ConsignmentId { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public int Order { get; set; }
    }
}