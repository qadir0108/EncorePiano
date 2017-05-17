using System.Collections.Generic;
using System.Web.Mvc;

namespace WFP.ICT.Web.Models
{
    public class RegistryDashboardVM
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
        
        public IEnumerable<SelectListItem> Years { get; set; }
        public IEnumerable<SelectListItem> Months { get; set; }
    }
}