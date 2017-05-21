using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WFP.ICT.Common;
using WFP.ICT.Data.Entities;

namespace WFP.ICT.Data
{
    public class SecurityDataSeeder
    {
        public static void Seed(WFPICTContext context)
        {
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

            if (!RoleManager.RoleExists(SecurityConstants.RoleEmployee))
            {
                RoleManager.Create(new WFPRole(SecurityConstants.RoleEmployee, false, false, "Employee Role"));
                AddClaims(context, RoleManager, SecurityConstants.RoleEmployee, SecurityConstants.ClaimsAdmin);
            }

            if (!RoleManager.RoleExists(SecurityConstants.RolePrivate))
            {
                RoleManager.Create(new WFPRole(SecurityConstants.RolePrivate, false, false, "Private Role"));
                AddClaims(context, RoleManager, SecurityConstants.RolePrivate, SecurityConstants.ClaimsDefault);
            }

            if (!RoleManager.RoleExists(SecurityConstants.RoleDealer))
            {
                RoleManager.Create(new WFPRole(SecurityConstants.RoleDealer, false, false, "Dealer Role"));
                AddClaims(context, RoleManager, SecurityConstants.RoleDealer, SecurityConstants.ClaimsDefault);
            }

            if (!RoleManager.RoleExists(SecurityConstants.RoleManufacturer))
            {
                RoleManager.Create(new WFPRole(SecurityConstants.RoleManufacturer, false, false, "Manufacturer Role"));
                AddClaims(context, RoleManager, SecurityConstants.RoleManufacturer, SecurityConstants.ClaimsDefault);
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

            var alreadyTestUser = UserManager.FindByName(SecurityConstants.TestEmployeeUser);
            if (alreadyTestUser == null)
            {
                var user = new WFPUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = SecurityConstants.TestEmployeeUser,
                    Email = "qadir0108@gmail.com",
                    FirstName = "Test",
                    LastName = "User",
                    CreatedAt = DateTime.Now
                };
                var result = UserManager.CreateAsync(user, SecurityConstants.UserAdminPassword);
                if (result.Result == IdentityResult.Success)
                {
                    var result1 = UserManager.AddToRole(user.Id, SecurityConstants.RolePrivate);
                }
            }
        }

        private static void AddClaims(WFPICTContext ctx, RoleManager<WFPRole> RoleManager, string Role, string[] permissions)
        {
            var role = RoleManager.FindByName(Role);
            foreach (var permisson in permissions)
            {
                var claim = ctx.Claims.FirstOrDefault(x => x.ClaimValue == permisson);
                AddClaim(ctx, role.Id, claim.Id);
            }
        }

        public static void AddClaim(WFPICTContext ctx, string roleId, Guid claimID)
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
