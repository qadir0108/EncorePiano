using System;
using WFP.ICT.Web.Models;

namespace WFP.ICT.Web.ViewModels
{
    public class WarehouseVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public AddressVm Address { get; set; }

        public string AddressToString
        {
            get
            {
                if (Address.Name == "" || Address.Address1 == "") return "";
                var pickupAddress = string.Format("{0}, <br /> {1} {2}, <br /> {3}, {4} {5} <br />{6}",
                    Address.Name, Address.Address1,
                    Address.City, Address.State,
                    Address.PostCode, Address.PhoneNumber).Trim("<br />".ToCharArray());
                return "<br />" + pickupAddress;
            }
        }

        public string AddressToStringWithOutPhone
        {
            get
            {
                if (Address.Name == "" || Address.Address1 == "") return "";
                var pickupAddress = string.Format("{0}, <br /> {1} {2}, <br /> {3}, {4} {5}",
                    Address.Name, Address.Address1,
                    Address.City, Address.State,
                    Address.PostCode).Trim("<br />".ToCharArray());
                return "<br />" + pickupAddress;
            }
        }
    }
}