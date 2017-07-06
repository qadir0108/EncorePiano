using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WFP.ICT.Web.ViewModels
{
    public class AddressEditorViewModel
    {
        public Guid? Id { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string City { get; set; }
    }
}