using System;
using System.ComponentModel.DataAnnotations;

namespace WFP.ICT.Web.Models
{
    public class PianoChargesVm
    {

        public string Id { get; set; }
        [Required(ErrorMessage = "Serice code is required")]
        public string ChargesCode { get; set; }
        public string ChargesType { get; set; }
        public string ChargesDetails { get; set; }
        [Required(ErrorMessage = "Service charges are required")]
        public string Amount { get; set; }

        public PianoChargesVm()
        {
            
        }
        public string ChargesTypeId { get; set; }


    }
   
    }