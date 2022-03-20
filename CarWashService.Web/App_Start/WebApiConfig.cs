using CarWashService.Web.Models.AuthenticationModels;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace CarWashService.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            _ = config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Filters.Add(new BasicAuthenticationAttribute());
        }
    }
}
