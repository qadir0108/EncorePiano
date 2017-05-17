namespace WFP.ICT.Web.Models
{
    public class CommonVM
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        
        public bool IsEditable { get; set; }
        public bool IsDeletable { get; set; }
        public string Comments { get; set; }
        public string Comment { get; set; }
        public string Action { get; set; }
    }
}