using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication2
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Edit",
                url: "Edit/{id}",
                defaults: new { controller = "Home", action = "Edit" });

            routes.MapRoute(
                name: "Download",
                url: "Save/{path}",
                defaults: new { controller = "Home", action = "Save" });

            routes.MapRoute(
                name: "Down",
                url: "Download/{link}",
                defaults: new { controller = "Home", action = "Download" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            
        }
    }
}
