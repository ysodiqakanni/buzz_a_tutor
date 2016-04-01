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
                name: "api/lessons/getattachment",
                routeTemplate: "api/lessons/getattachment",
                defaults: new { controller = "LessonsApi", action = "Getattachment" }
            );
        }
    }
}
