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
                return;   // DB has been seeded
            }

            string companyName = "Encore Piano Moving";
            var alreayCompany = context.Companys.FirstOrDefault(x => x.Name.Equals(companyName, StringComparison.InvariantCultureIgnoreCase));
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
                    EmailUserName = "aspillan@movemypiano.com",
                    EmailPassword = "secrete",
                    Logo = "logo.jpg",
                    WebSite = "www.movemypiano.com",
                    //Theme = (int) ThemesEnum.Default
                };
                context.Companys.Add(company);
                context.SaveChanges();

                // Piano Types
                foreach (var ptype in EnumHelper.GetEnumTextValues(typeof(PianoTypeEnum)))
                {
                    string officeCode = ptype.Value;
                    var already = context.PianoTypes.FirstOrDefault(m => m.Code == officeCode);
                    if (already == null)
                    {
                        context.PianoTypes.Add(new PianoType()
                        {
                            Id = Guid.NewGuid(),
                            CreatedAt = DateTime.Now,
                            Code = ptype.Value,
                            Type = ptype.Text,
                        });
                    }
                }
                context.SaveChanges();

                // Permissions
                foreach (var permission in SecurityConstants.ClaimsAll)
                {
                    context.Claims.Add(new AspNetClaims()
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.Now,
                        ClaimType = permission.Contains(PermissionType.Order.ToLower())
                            ? PermissionType.Order
                            : PermissionType.Permission,
                        ClaimValue = permission
                    });
                }
                context.SaveChanges();

                // Roles
                var RoleManager = new RoleManager<WFPRole>(new RoleStore<WFPRole>(context));
                if (!RoleManager.RoleExists(SecurityConstants.RoleAdmin))
                {
                    RoleManager.Create(new WFPRole(SecurityConstants.RoleAdmin, false, false, "Admin Role"));
                    AddClaims(context, RoleManager, SecurityConstants.RoleAdmin, SecurityConstants.ClaimsAdmin);
                }

                if (!RoleManager.RoleExists(SecurityConstants.RolePrivateUser))
                {
                    RoleManager.Create(new WFPRole(SecurityConstants.RolePrivateUser, false, false, "Private User Role"));
                    AddClaims(context, RoleManager, SecurityConstants.RolePrivateUser, SecurityConstants.ClaimsDefault);
                }

                var UserManager = new UserManager<WFPUser>(new UserStore<WFPUser>(context));

                var alreadyAdminUser = UserManager.FindByName(SecurityConstants.UserAdmin);
                if (alreadyAdminUser == null)
                {
                    var adminUser = new WFPUser()
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = SecurityConstants.UserAdmin,
                        Email = "aspillan@movemypiano.com",
                        FirstName = "Admin",
                        LastName = "User",
                        CreatedAt = DateTime.Now
                    };
                    var result = UserManager.CreateAsync(adminUser, SecurityConstants.UserAdminPassword);
                    if (result.Result == IdentityResult.Success)
                    {
                        var result1 = UserManager.AddToRole(adminUser.Id, SecurityConstants.RoleAdmin);
                    }
                }

                var alreadyTestUser = UserManager.FindByName(SecurityConstants.TestPrivateUser);
                if (alreadyTestUser == null)
                {
                    var user = new WFPUser()
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = SecurityConstants.TestPrivateUser,
                        Email = "qadir0108@gmail.com",
                        FirstName = "Test",
                        LastName = "User",
                        CreatedAt = DateTime.Now
                    };
                    var result = UserManager.CreateAsync(user, SecurityConstants.UserAdminPassword);
                    if (result.Result == IdentityResult.Success)
                    {
                        var result1 = UserManager.AddToRole(user.Id, SecurityConstants.RolePrivateUser);
                    }
                }
            }
        }

        private void AddClaims(WFPICTContext ctx, RoleManager<WFPRole> RoleManager, string Role, string[] permissions)
        {
            var role = RoleManager.FindByName(Role);
            foreach (var permisson in permissions)
            {
                var claim = ctx.Claims.FirstOrDefault(x => x.ClaimValue == permisson);
                AddClaim(ctx, role.Id, claim.Id);
            }
        }

        public void AddClaim(WFPICTContext ctx, string roleId, Guid claimID)
        {
            ctx.RoleClaims.Add(new AspNetRoleClaims()
            {
                Id = Guid.NewGuid(),
                RoleID = roleId,
                ClaimID = claimID,
                CreatedAt = DateTime.Now
            });
            ctx.SaveChanges();
        }
    }
}
