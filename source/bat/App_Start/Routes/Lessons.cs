using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace bat.Routes
{
    public class Lessons
    {
        public Lessons(RouteCollection routes)
        {
            routes.MapRoute(
                name: "lessons/new",
                url: "lessons/new",
                defaults: new { controller = "Lessons", action = "New" }
            );
        }
    }
}