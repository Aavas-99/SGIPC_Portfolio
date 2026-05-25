using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SGIPC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "PortfolioCommittee",
                url: "Committee",
                defaults: new { controller = "Portfolio", action = "Committee" }
            );

            routes.MapRoute(
                name: "PortfolioSignin",
                url: "Signin",
                defaults: new { controller = "Portfolio", action = "Signin" }
            );

            routes.MapRoute(
                name: "PortfolioSignup",
                url: "Signup",
                defaults: new { controller = "Portfolio", action = "Signup" }
            );

            routes.MapRoute(
                name: "PortfolioForm",
                url: "Form",
                defaults: new { controller = "Portfolio", action = "Form" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Portfolio", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
