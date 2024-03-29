﻿using System;
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
                url: "lessons/new/{date}",
                defaults: new { controller = "Lessons", action = "New", date = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "lessons/join",
                url: "lessons/join/{id}",
                defaults: new { controller = "Lessons", action = "Join" }
            );

            routes.MapRoute(
                name: "lessons/leave",
                url: "lessons/leave/{lessonid}/{studentid}",
                defaults: new { controller = "Lessons", action = "Leave" }
            );

            routes.MapRoute(
                name: "lessons/upload",
                url: "lessons/upload",
                defaults: new { controller = "Lessons", action = "Upload" }
            );

            routes.MapRoute(
                name: "lessons",
                url: "lessons/{id}",
                defaults: new { controller = "Lessons", action = "Index" }
            );

            routes.MapRoute(
                name: "lessons/edit",
                url: "lessons/edit/{id}",
                defaults: new { controller = "Lessons", action = "Edit" }
            );
        }
    }
}