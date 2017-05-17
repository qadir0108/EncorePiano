namespace WFP.ICT.Web.Models
{
    public class CampaignSearchVM
    {
        public string sortOrder { get; set; }
        public int? page { get; set; }

        public string searchType { get; set; }
        public string searchString { get; set; }

        public string campaignName { get; set; }
        public string IsTested { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }

        public string Status { get; set; }
        public string clearSessionId { get; set; }
    }
}