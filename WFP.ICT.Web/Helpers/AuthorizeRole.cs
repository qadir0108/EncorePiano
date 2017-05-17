using System;
using System.Security.Claims;
using System.Web.Mvc;
using System.Web.Routing;
using System.Linq;

namespace WFP.ICT.Web.Helpers
{
    public class AuthorizeRole : ActionFilterAttribute
    {
        public string Roles { get; set; }

        /// <summary>
        /// Method: OnActionExecuting
        /// Comments: This action filter method is user authorized to access the page.
        /// Parameters: Required the Page Permissions enum   
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {           
            bool IfAllowed = false;

            try
            {
                // Check Claims
                var userIdentity = filterContext.HttpContext.User.Identity as ClaimsIdentity;
                var roles = userIdentity.Claims.Where(x => x.Type == ClaimTypes.Role).Select(claim =>claim.Value).ToArray();
                var rolesList = Roles.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                foreach (var requestedRole in rolesList)
                {
                    if (roles.Contains(requestedRole))
                    {
                        IfAllowed = true;
                        break;
                    }
                }
                
                if (IfAllowed)
                {
                    base.OnActionExecuting(filterContext);
                    return;
                }
            }
            catch(Exception ex)
            {
              
            }

            // Redirect to Error page
            filterContext.Result =
                    new RedirectToRouteResult(new RouteValueDictionary   
                    {  
                        { "action", "NotAuthorized" },  
                        { "controller", "Error" },
                    });
            return;
        }
    }
}