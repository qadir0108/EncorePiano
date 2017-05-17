using System.Collections.Generic;
using System.Web.Mvc;

namespace WFP.ICT.Web.Models
{
    public class CompanyUnitVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsEditable { get; set; }
        public bool IsDeletable { get; set; }
        public string HeadID { get; set; }
        public string FocalPersonID { get; set; }

        public string Action { get; set; }

        public IEnumerable<SelectListItem> Users { get; set; }
    }
}