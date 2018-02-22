using System.Collections.Generic;
using System.Web.Mvc;

namespace WFP.ICT.Web.Models
{
    public class CompanyPermissionVm
    {
        public string Id { get; set; }
        public string ClaimValue { get; set; }
        public string ClaimType { get; set; }
        public string RolesCount { get; set; }
        public bool IsEditable { get; set; }
        public bool IsDeletable { get; set; }
    }
}