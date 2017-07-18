using System;
using System.ComponentModel.DataAnnotations;

namespace WFP.ICT.Web.Models
{
    public class PianoServiceVm
    {
        public WFP.ICT.Data.Entities.PianoCharges _service { get; set; }

        public string Id { get; set; }
        [Required(ErrorMessage = "Serice code is required")]
        public string ServiceCode { get; set; }
        public string ServiceType { get; set; }
        public string ServiceDetails { get; set; }
        [Required(ErrorMessage = "Service charges are required")]
        public string ServiceCharges { get; set; }

        public PianoServiceVm()
        {
            
        }

        public PianoServiceVm(WFP.ICT.Data.Entities.PianoCharges service)
        {
            _service = service;
        }
    }
   
    }