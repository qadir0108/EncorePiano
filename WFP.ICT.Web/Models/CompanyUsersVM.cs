using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WFP.ICT.Enum;

namespace WFP.ICT.Web.Models
{
    public class CompanyUsersVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string RetypePassword { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }

        public string ActionToTake { get; set; }

        public List<SelectItemPair> Roles { get; set; }
        public string Role { get; set; }

    }
}