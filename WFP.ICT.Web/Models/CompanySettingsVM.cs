using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WFP.ICT.Enum;

namespace WFP.ICT.Web.Models
{
    public class CompanySettingsVM
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public string WebSite { get; set; }

        // UI Settings
        public string Logo { get; set; }

        // AD Settings
        public string ActiveDiretoryDomain { get; set; }
        public string ActiveDiretoryUserName { get; set; }
        public string ActiveDiretoryPassword { get; set; }

        // Email Settings
        public string EmailServer { get; set; }
        public string EmailUserName { get; set; }
        public string EmailPassword { get; set; }

        public IEnumerable<SelectListItem> Offices { get; set; }
    }
}