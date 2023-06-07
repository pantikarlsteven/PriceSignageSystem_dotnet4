using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PriceSignageSystem
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "PreviewCrystalReport",
            //    url: "Report/PreviewCrystalReport",
            //    defaults: new { controller = "Report", action = "PreviewCrystalReport" }
            //);

            //routes.MapRoute(
            //    name: "AutoPrintOnDemandReport",
            //    url: "Report/AutoPrintOnDemandReport",
            //    defaults: new { controller = "Report", action = "AutoPrintOnDemandReport" }
            //);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}
