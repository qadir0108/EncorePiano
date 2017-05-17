namespace WFP.ICT.Web.Models
{
    public class StationerySearchVM
    {
        public string sortOrder { get; set; }
        public int? page { get; set; }

        public string searchType { get; set; }
        public string searchString { get; set; }
        public string inTypeId { get; set; }
        public string inModeId { get; set; }
        public string subject { get; set; }
        public string from { get; set; }
        public string UnitID { get; set; }
        public string attentionTo { get; set; }
        public string sender { get; set; }
    }
}