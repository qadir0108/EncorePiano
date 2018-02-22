using System.Collections.Generic;
using System.Web.Mvc;

namespace WFP.ICT.Web.Models
{
    public class CompanyUsersGroupedVM
    {
        public string OfficeName { get; set; }
        public string UnitName { get; set; }

        public List<CompanyUsersVm> Users { get; set; }
        
    }
}