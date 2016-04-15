using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace bat.Routes
{
    public class Profile
    {
        public Profile(RouteCollection routes)
        {
            routes.MapRoute(
                name: "edit",
                url: "profile/edit",
                defaults: new { controller = "Profile", action = "Edit" }
            );

            routes.MapRoute(
                name: "profile",
                url: "Profile/",
                defaults: new { controller = "Profile", action = "Index" }
            );
        }
    }
}