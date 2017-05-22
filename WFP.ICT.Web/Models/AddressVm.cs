using System;

namespace WFP.ICT.Web.Models
{
    public class AddressVm
    {
        public WFP.ICT.Data.Entities.Address _address { get; set; }

        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }

        public AddressVm()
        {
            
        }

        public AddressVm(WFP.ICT.Data.Entities.Address address)
        {
            _address = address;
        }

        public string AddressToString
        {
            get
            {
                if (_address == null) return "";
                var pickupAddress = string.Format("{0}, <br /> {1} {2}, <br /> {3}, {4} {5} <br />{6}",
                    _address.Name, _address.Address1, _address.Address2,
                    _address.Suburb, _address.State,
                    _address.PostCode, _address.PhoneNumber).Trim("<br />".ToCharArray());
                return "<br />" + pickupAddress;
            }
        }

        public string AddressToStringWithOutPhone
        {
            get
            {
                if (_address == null) return "";
                var pickupAddress = string.Format("{0}, <br /> {1} {2}, <br /> {3}, {4} {5}",
                    _address.Name, _address.Address1, _address.Address2,
                    _address.Suburb, _address.State,
                    _address.PostCode).Trim("<br />".ToCharArray());
                return "<br />" + pickupAddress;
            }
        }
    }
}