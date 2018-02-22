using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WFP.ICT.Enum;

namespace WFP.ICT.Web.Models
{
    public class CompanySettingsVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public string WebSite { get; set; }

        // UI Settings
        public string Logo { get; set; }

        public IEnumerable<SelectListItem> Offices { get; set; }
    }
}