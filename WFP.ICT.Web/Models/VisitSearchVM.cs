namespace WFP.ICT.Web.Models
{
    public class VisitSearchVM
    {
        public string sortOrder { get; set; }
        public int? page { get; set; }

        public string searchType { get; set; }
        public string searchString { get; set; }

        public string registryNumberFrom { get; set; }
        public string registryNumberTo { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
    }
}