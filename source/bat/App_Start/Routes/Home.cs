using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace bat.Routes
{
    public class Home
    {
        public Home(RouteCollection routes)
        {
            routes.MapRoute(
                name: "home",
                url: "",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapRoute(
                name: "login",
                url: "login",
                defaults: new { controller = "Home", action = "Login" }
            );

            routes.MapRoute(
                name: "swap",
                url: "swap/{id}",
                defaults: new { controller = "Home", action = "Swap" }
            );

            routes.MapRoute(
                name: "subject",
                url: "subject/{subject}",
                defaults: new { controller = "Home", action = "SubjectList" }
            );

            routes.MapRoute(
                name: "logout",
                url: "logout",
                defaults: new { controller = "Home", action = "Logout" }
            );
        }
    }
}