using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Data.Entity;

namespace WFP.ICT.Data.Entities
{
    // ID, UserName, Email, PhoneNumber and Password used from IdentityUser
    public class WFPUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<WFPUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            try
            {
                using (WFPICTContext ctx = new WFPICTContext())
                {
                    var userId = userIdentity.GetUserId();
                    var user = ctx.Users.Include(x => x.Roles).FirstOrDefault(x => x.Id == userId);

                    foreach (var role in user.Roles)
                    {
                        var roleClaims = ctx.RoleClaims.Include("Claim").Where(x => x.RoleID == role.RoleId);
                        foreach (var roleClaim in roleClaims)
                        {
                            userIdentity.AddClaim(new Claim(ClaimTypes.UserData, roleClaim.Claim.ClaimValue));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return userIdentity;
        }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailSignature { get; set; }

        public DateTime? LastLogin { get; set; }

        public int Status { get; set; } // UserStatusEnum
        public int UserType { get; set; } // UserTypeEnum

        public Guid? ClientId { get; set; } // If user is of Dealer/Manufacturer
        public virtual Client Client { get; set; }

        public DateTime CreatedAt { get; set; }
        public string CreatedByID { get; set; }

        public string APIKey { get; set; }

        public WFPUser()
        {
        }
    }
}
