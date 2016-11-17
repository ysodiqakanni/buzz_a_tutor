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
                name: "landing",
                url: "",
                defaults: new { controller = "Home", action = "Landing" }
            );

            routes.MapRoute(
                name: "home",
                url: "home",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapRoute(
                name: "login",
                url: "login",
                defaults: new { controller = "Home", action = "Login" }
            );

            routes.MapRoute(
                name: "forgotpassword",
                url: "forgotpassword",
                defaults: new { controller = "Home", action = "ForgotPassword" }
            );

            routes.MapRoute(
                name: "resetpassword",
                url: "resetpassword",
                defaults: new { controller = "Home", action = "ResetPassword" }
            );

            routes.MapRoute(
                name: "loginsignup",
                url: "loginsignup",
                defaults: new { controller = "Home", action = "LoginSignup" }
            );

            routes.MapRoute(
                name: "swap",
                url: "swap/{id}",
                defaults: new { controller = "Home", action = "Swap" }
            );

            routes.MapRoute(
                name: "subjects",
                url: "subjects",
                defaults: new { controller = "Home", action = "SelectSubject" }
            );

            routes.MapRoute(
                name: "teachers",
                url: "teachers",
                defaults: new { controller = "Home", action = "ListTeachers" }
            );

            routes.MapRoute(
                name: "teacher",
                url: "teacher/{id}",
                defaults: new { controller = "Home", action = "ListTeacherLessons" }
            );

            routes.MapRoute(
                name: "list-lessons",
                url: "list-lessons/{subject}",
                defaults: new { controller = "Home", action = "ListLessons" }
            );

            routes.MapRoute(
                name: "logout",
                url: "logout",
                defaults: new { controller = "Home", action = "Logout" }
            );

            routes.MapRoute(
                name: "about",
                url: "about",
                defaults: new { controller = "Home", action = "About" }
            );

            routes.MapRoute(
                name: "terms",
                url: "terms",
                defaults: new { controller = "Home", action = "Terms" }
            );

            routes.MapRoute(
                name: "privacy",
                url: "privacy",
                defaults: new { controller = "Home", action = "Privacy" }
            );
        }
    }
}