using System.Collections.Generic;
using System.Web.Mvc;

namespace WFP.ICT.Web.Models
{
    public class CompanyOfficeVM
    {
        public string Id { get; set; }
        public int Code { get; set; }
        public string OfficeName { get; set; }
        public bool IsEditable { get; set; }
        public bool IsDeletable { get; set; }
        
    }
}