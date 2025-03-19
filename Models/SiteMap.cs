using Microsoft.AspNetCore.Mvc.Controllers;


namespace ProductionManagement.Models
{
    public class SiteMap
    {
        public IEnumerable<RouteInfo> Routes { get; set; }
    }

    public class RouteInfo
    {
        public string Controller { get; set; }
        public string Action { get; set; }
    }

    public static class RouteHelper
    {
        public static IEnumerable<RouteInfo> GetRoutes(IEndpointRouteBuilder endpoints)
        {
            var routes = new List<RouteInfo>();
            foreach (var route in endpoints.DataSources)
            {
                foreach (var endpoint in route.Endpoints)
                {
                    var controller = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>()?.ControllerName;
                    var action = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>()?.ActionName;

                    if (controller != null && action != null)
                    {
                        var routeInfo = new RouteInfo
                        {
                            Controller = controller,
                            Action = action,
                        };
                        routes.Add(routeInfo);
                    }
                }
            }
            return routes;
        }
    }
}
