using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using WFP.ICT.Data.Entities;
using WFP.ICT.Enum;

namespace WFP.ICT.Web
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<WFPUser>
    {
        public ApplicationUserManager(IUserStore<WFPUser> store) : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<WFPUser>(context.Get<WFPICTContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<WFPUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                //RequiredLength = 6,
                //RequireNonLetterOrDigit = true,
                //RequireDigit = true,
                //RequireLowercase = true,
                //RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<WFPUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<WFPUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<WFPUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    public class ApplicationRoleManager : RoleManager<WFPRole>
    {
        public ApplicationRoleManager(IRoleStore<WFPRole, string> roleStore): base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<WFPRole>(context.Get<WFPICTContext>()));
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

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<WFPUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(WFPUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }

        public override Task SignInAsync(WFPUser user, bool isPersistent, bool rememberBrowser)
        {
            return base.SignInAsync(user, isPersistent, rememberBrowser);
        }

        public override Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser)
        {
            return base.TwoFactorSignInAsync(provider, code, isPersistent, rememberBrowser);
        }

        //public override Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        //{
        //    var resultFromLocal = base.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);
        //    if (resultFromLocal.Result == SignInStatus.Success)
        //    {
        //        var user = UserManager.FindByName(userName);
        //        if (user.Status == (int)UserStatusEnum.Locked)
        //            return Task.FromResult(SignInStatus.LockedOut);

        //        return Task.FromResult(SignInStatus.Success);
        //    } else if (resultFromLocal.Result == SignInStatus.LockedOut)
        //    {
        //        return Task.FromResult(SignInStatus.LockedOut);
        //    }
        //    else
        //    {
        //        var company = HttpContext.Current.Session["company"] as WFP.ICT.Data.Entities.Company;
        //        bool IsAuthenticated = DirectoryManager.Authenticate(company.ActiveDiretoryDomain, userName, password);
        //        if (IsAuthenticated) {

        //            // Create user
        //            var user = UserManager.FindByName(userName);
        //            if (user == null)
        //            {
        //                var newUser = new WFPUser
        //                {
        //                    UserName = userName,
        //                    Email = userName + "@wfp.org",
        //                    CreatedAt = DateTime.Now
        //                };

        //                // Temp password, so that for each time local should fail
        //                var createUserResult = UserManager.CreateAsync(newUser, SecurityConstants.UserAllUserPassword);
        //                if (createUserResult.Result == IdentityResult.Success)
        //                {
        //                }
        //                else
        //                {
        //                    throw new Exception("There is an error while creating user." + createUserResult.Result.Errors.FirstOrDefault());
        //                }
        //            }

        //            return Task.FromResult(SignInStatus.RequiresVerification);
        //        } else
        //        {
        //            return Task.FromResult(SignInStatus.Failure);
        //        }
        //    }
        //}
    }
}
