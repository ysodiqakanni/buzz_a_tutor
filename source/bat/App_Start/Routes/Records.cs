using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace bat.Routes
{
    public class Records
    {
        public Records(RouteCollection routes)
        {
            routes.MapRoute(
                name: "details",
                url: "records/{id}",
                defaults: new { controller = "ChatRecords", action = "Details" }
            );

            routes.MapRoute(
                name: "records",
                url: "records/",
                defaults: new { controller = "ChatRecords", action = "Index" }
            );
        }
    }
}