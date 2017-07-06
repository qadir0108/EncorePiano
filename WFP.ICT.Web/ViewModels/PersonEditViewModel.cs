using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WFP.ICT.Web.ViewModels
{
    public class PersonEditViewModel
    {
        public IEnumerable<AddressEditorViewModel> Addresses { get; set; }
    }
}