using System.ComponentModel.DataAnnotations;

namespace WFP.ICT.Web.Models
{
    public class PrintingProcessVM
    {
        public string OfficeName { get; set; }
        public string FileId { get; set; }

        [Required]
        [Display(Name = "FileName")]
        public string FileName { get; set; }

        [Required]
        [Display(Name = "If Firt Row is Header?")]
        public bool IsHeader { get; set; }
    }
}