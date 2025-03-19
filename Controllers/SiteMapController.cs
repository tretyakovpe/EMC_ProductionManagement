using Microsoft.AspNetCore.Mvc;
using ProductionManagement.Models;

namespace ProductionManagement.Controllers
{
    public class SiteMapController(IEndpointRouteBuilder endpoints) : Controller
    {
        private readonly IEndpointRouteBuilder _endpoints = endpoints;

        // GET: /SiteMap
        public IActionResult Index()
        {
            var siteMap = new SiteMap
            {
                Routes = RouteHelper.GetRoutes(_endpoints)
            };

            return View(siteMap);
        }
    }
}