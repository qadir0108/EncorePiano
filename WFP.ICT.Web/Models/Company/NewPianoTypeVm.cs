﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WFP.ICT.Web.Models
{
    public class NewPianoTypeVm
    {
        [Required(ErrorMessage = "Code is required")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Type is required")]
        public string Type { get; set; }
        
        public Guid? Id { get; set; }
        public NewPianoTypeVm()
        {
        }
    }
}