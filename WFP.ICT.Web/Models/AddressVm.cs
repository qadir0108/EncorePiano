using System;
using System.ComponentModel.DataAnnotations;

namespace WFP.ICT.Web.Models
{
    public class AddressVm
    {

        //[Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        //[Required(ErrorMessage = "Address is required")]
        public string Address1 { get; set; }
   

        //[Required(ErrorMessage = "State is required")]
        public string State { get; set; }

        //[Required(ErrorMessage = "Zip code is required")]
        public string PostCode { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }
        public int? Stairs{ get; set; }
        public int? Turns { get; set; }

        //[Required(ErrorMessage = "Pickup date required")]
        public DateTime? PickUpDate { get; set; }

        //[Required(ErrorMessage = "Warehouse location is required")]
        public string Warehouse{ get; set; }

        //[Required(ErrorMessage = "Contact is required")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string PhoneNumber { get; set; }

        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string AlternatePhone { get; set; }
        public string AlternateContact { get; set; }

        public string Notes { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }

        public new string ToString
        {
            get
            {
                if (Name == "" || Address1 == "") return "";
                var pickupAddress = $"{Name}, <br /> {Address1} {City}, <br /> {State}, {PostCode} {PhoneNumber} "
                    .Trim("<br />".ToCharArray());
                return "<br />" + pickupAddress;
            }
        }

        public new string ToStringWithStairTurns
        {
            get
            {
                if (Name == "" || Address1 == "") return "";
                var pickupAddress = $"{Name}, <br /> {Address1} {City}, <br /> {State}, {PostCode} {PhoneNumber} <br /> Stairs {Stairs}, Turns {Turns}, Notes {Notes} "
                    .Trim("<br />".ToCharArray());
                return "<br />" + pickupAddress;
            }
        }

        public string ToStringWithoutBreak
        {
            get
            {
                if (Name == "" || Address1 == "") return "";
                return string.Format("{0}, {1} {2}, {3}, {4}", Name, Address1, City, State, PostCode);
            }
        }

        public string ToStringWithOutPhone
        {
            get
            {
                if (Name == "" || Address1 == "") return "";
                var pickupAddress = string.Format("{0}, <br /> {1} {2}, <br /> {3}, {4}",
                    Name, Address1, 
                    City, State,
                    PostCode).Trim("<br />".ToCharArray());
                return "<br />" + pickupAddress;
            }
        }

        public AddressVm()
        {

        }
    }
}