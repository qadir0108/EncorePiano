namespace WFP.ICT.Web.API.Models
{
    public enum RequestParamsEnum
    {
        Id
    }

    public class AssignmentResponse
    {
        public string Id { get; set; }
        public string ConsignmentNumber { get; set; }
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
        public string CallerPhoneNumberAlt { get; set; }
        public string CallerEmail { get; set; }
        
        public string PickupDate { get; set; }
        public string PickupAddress { get; set; }
        public string PickupPhoneNumber { get; set; }
        public string PickupAlternateContact { get; set; }
        public string PickupAlternatePhone { get; set; }
        public int PickupNumberStairs { get; set; }
        public int PickupNumberTurns { get; set; }
        public string PickupInstructions { get; set; }

        public string DeliveryDate { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryPhoneNumber { get; set; }
        public string DeliveryAlternateContact { get; set; }
        public string DeliveryAlternatePhone { get; set; }
        public int DeliveryNumberStairs { get; set; }
        public int DeliveryNumberTurns { get; set; }
        public string DeliveryInstructions { get; set; }

        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }

        public int NumberOfItems { get; set; }
        public PianoResponse[] Pianos { get; set; }
        public RouteResponse[] Route { get; set; }
    }

    public class PianoResponse
    {
        public string Id { get; set; }
        public string ConsignmentId { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Finish { get; set; }
        public string SerialNumber { get; set; }
        public int IsBench { get; set; }
        public int IsPlayer { get; set; }
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