using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WFP.ICT.Web.Models
{
    public class NewAddressVm
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Contact is required")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }

        [Required(ErrorMessage = "Zip code is required")]
        public string PostCode { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }


        [Required(ErrorMessage = "Please select marker")]
        public string lng { get; set; }

        public string lat { get; set; }

        public NewClientVm Client { get; set; }

        public Guid? Id { get; set; }
        public NewAddressVm()
        {
            Client = new NewClientVm();
        }
    }

    public class NewClientVm
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Contact is required")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please select type")]
        public int Type { get; set; }

        [Required(ErrorMessage = "Code is required")]
        public string AccountCode { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please Enter Valid Email")]
        public string Email { get; set; }
        public String Notes { get; set; }

        public Guid? Id { get; set; }

    }
}