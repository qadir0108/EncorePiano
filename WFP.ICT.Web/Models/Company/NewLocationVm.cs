using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WFP.ICT.Web.Models
{
    public class NewLocationVm
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        
        public Guid? Id { get; set; }
        public NewLocationVm()
        {
        }
    }
}