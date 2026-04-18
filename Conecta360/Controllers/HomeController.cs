using Microsoft.AspNetCore.Mvc;

namespace Conect360.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Pasamos estadísticas al sidebar y al dashboard de inicio
            ViewBag.TotalContactos = ContactosController.ObtenerContactos().Count;
            ViewBag.TotalCategorias = ContactosController.ObtenerContactos()
                                        .Select(c => c.Categoria)
                                        .Distinct()
                                        .Count();
            return View();
        }
    }
}
