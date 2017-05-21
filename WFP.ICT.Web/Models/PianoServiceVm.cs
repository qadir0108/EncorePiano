using System;

namespace WFP.ICT.Web.Models
{
    public class PianoServiceVm
    {
        public WFP.ICT.Data.Entities.PianoService _service { get; set; }

        public string Id { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceType { get; set; }
        public string ServiceDetails { get; set; }
        public string ServiceCharges { get; set; }
        public bool IsSelected { get; set; }

        public PianoServiceVm()
        {
            
        }

        public PianoServiceVm(WFP.ICT.Data.Entities.PianoService service)
        {
            _service = service;
        }
    }
}