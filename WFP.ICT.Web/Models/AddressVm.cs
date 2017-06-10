using System;

namespace WFP.ICT.Web.Models
{
    public class AddressVm
    {
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }

        public string AddressToString
        {
            get
            {
                if (Name == "" || Address1 == "") return "";
                var pickupAddress = string.Format("{0}, <br /> {1} {2}, <br /> {3}, {4} {5} <br />{6}",
                    Name, Address1, Address2,
                    Suburb, State,
                    PostCode, PhoneNumber).Trim("<br />".ToCharArray());
                return "<br />" + pickupAddress;
            }
        }

        public string AddressToStringWithoutBreak
        {
            get
            {
                if (Name == "" || Address1 == "") return "";
                var pickupAddress = string.Format("{0}, {1} {2}, {3}, {4} {5}",
                    Name, Address1, Address2, Suburb, State, PostCode);
                return pickupAddress;
            }
        }

        public string AddressToStringWithOutPhone
        {
            get
            {
                if (Name == "" || Address1 == "") return "";
                var pickupAddress = string.Format("{0}, <br /> {1} {2}, <br /> {3}, {4} {5}",
                    Name, Address1, Address2,
                    Suburb, State,
                    PostCode).Trim("<br />".ToCharArray());
                return "<br />" + pickupAddress;
            }
        }
    }
}