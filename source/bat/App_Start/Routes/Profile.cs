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
                name: "password",
                url: "profile/password",
                defaults: new { controller = "Profile", action = "EditPassword" }
            );

            routes.MapRoute(
                name: "newMember",
                url: "profile/family/new",
                defaults: new { controller = "Profile", action = "NewMember" }
            );

            routes.MapRoute(
                name: "editMember",
                url: "profile/family/edit/{id}",
                defaults: new { controller = "Profile", action = "EditMember" }
            );

            routes.MapRoute(
                name: "deleteMember",
                url: "profile/family/delete/{id}",
                defaults: new { controller = "Profile", action = "DeleteMember" }
            );

            routes.MapRoute(
                name: "profile",
                url: "profile/",
                defaults: new { controller = "Profile", action = "Index" }
            );
        }
    }
}