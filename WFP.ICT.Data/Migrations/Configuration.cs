using WFP.ICT.Common;
using WFP.ICT.Enums;

namespace WFP.ICT.Data.Migrations
{
    using Entities;
    using Enum;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<WFPICTContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;

        }

        protected override void Seed(WFPICTContext context)
        {
            // Look for any students.
            if (context.Users.Any())
            {
                return; // DB has been seeded
            }

            string companyName = "Encore Piano Moving";
            var alreayCompany =
                context.Companys.FirstOrDefault(
                    x => x.Name.Equals(companyName, StringComparison.InvariantCultureIgnoreCase));
            if (alreayCompany == null)
            {
                // Company Settings
                var companyID = Guid.NewGuid();
                var company = new Company()
                {
                    Id = companyID,
                    CreatedAt = DateTime.Now,
                    Name = companyName,
                    Details = "Encore Piano Moving",
                    Logo = "logo.jpg",
                    WebSite = "www.movemypiano.com",
                    //Theme = (int) ThemesEnum.Default
                };
                context.Companys.Add(company);
                context.SaveChanges();

                SecurityDataSeeder.Seed(context);

                TestDataSeeder.Seed(context);
            }
        }

    }
}
