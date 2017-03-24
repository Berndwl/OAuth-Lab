using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TFiN.OAuthLab
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Auth",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Auth", action = "Authorize", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "RegisterClient",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "RegisterClient", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
