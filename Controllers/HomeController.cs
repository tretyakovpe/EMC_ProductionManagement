using Microsoft.AspNetCore.Mvc;

namespace ProductionManagement.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

