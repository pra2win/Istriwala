using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Linq;

namespace Istriwala.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Enable CORS
            string origins = ConfigurationManager.AppSettings["Origins"];
            var enableCorsAttribute = new EnableCorsAttribute("*",
                                                   "Origin, Content-Type, Accept, Authorization",
                                                   "GET, POST, OPTIONS, TOKEN");
            config.EnableCors(enableCorsAttribute);

            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Custom global exception logging
            //config.Services.Add(typeof(IExceptionLogger), new GlobalExceptionLogger(new BPA.Services.Repositories.BpaApiExceptionRepository()));

            // Custom filters
            //config.Filters.Add(new BPA.API.Web.Filters.CustomExceptionFilter());

            // Force JSON date serialization to local time-zone
            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local;

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
