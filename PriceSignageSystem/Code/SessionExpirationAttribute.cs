using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PriceSignageSystem.Code
{
    public class SessionExpirationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;

            // Check if the session is expired
            if (session != null && session.IsNewSession)
            {
                string sessionCookie = filterContext.HttpContext.Request.Headers["Cookie"];

                // Check if a new session cookie is being created
                if ((sessionCookie != null) && (sessionCookie.IndexOf("ASP.NET_SessionId") >= 0))
                {
                    // Redirect to the SessionExpired action
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
                    { "controller", "Home" },
                    { "action", "SessionExpired" }
                });
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}