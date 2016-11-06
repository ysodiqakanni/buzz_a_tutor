using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace bat
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            config.Routes.MapHttpRoute(
                name: "api/lessons/upload",
                routeTemplate: "api/lessons/upload",
                defaults: new { controller = "LessonsApi", action = "Upload" }
            );

            config.Routes.MapHttpRoute(
                name: "api/lessons/uploadtocloud",
                routeTemplate: "api/lessons/uploadtocloud",
                defaults: new { controller = "LessonsApi", action = "UploadtoCloud" }
            );

            config.Routes.MapHttpRoute(
                name: "api/lessons/downloadfromcloud",
                routeTemplate: "api/lessons/downloadfromcloud",
                defaults: new { controller = "LessonsApi", action = "DownloadFromCloud" }
            );

            config.Routes.MapHttpRoute(
                name: "api/lessons/getattachment",
                routeTemplate: "api/lessons/getattachment",
                defaults: new { controller = "LessonsApi", action = "Getattachment" }
            );

            config.Routes.MapHttpRoute(
                name: "api/lessons/getbblist",
                routeTemplate: "api/lessons/getbblist",
                defaults: new { controller = "LessonsApi", action = "GetBlackboardImages" }
            );

            config.Routes.MapHttpRoute(
                name: "api/dashboard/getevents",
                routeTemplate: "api/dashboard/getevents",
                defaults: new { controller = "DashBoardApi", action = "Getevents" }
            );

            config.Routes.MapHttpRoute(
                name: "api/dashboard/getday",
                routeTemplate: "api/dashboard/getday",
                defaults: new { controller = "DashBoardApi", action = "Getday" }
            );

            config.Routes.MapHttpRoute(
                name: "api/chat/gethistory",
                routeTemplate: "api/chat/gethistory",
                defaults: new { controller = "ChatApi", action = "GetChatHistory" }
            );
        }
    }
}
