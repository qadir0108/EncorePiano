namespace WFP.ICT.Web.Models
{
    public class UserProfileVM
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CCEmails { get; set; }
        public string Phone { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public string DateCreated { get; set; }

        public bool IsUsesAPI { get; set; }
        public string APIKey { get; set; }
        public bool IsTestsCreatives { get; set; }

        public string CompanyLogo { get; set; }
    }
}