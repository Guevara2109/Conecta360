using Microsoft.AspNetCore.Mvc;

namespace Conect360.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
