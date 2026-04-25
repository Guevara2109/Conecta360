using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Conect360.Data;

namespace Conect360.Controllers
{
    public class HomeController : Controller
    {
        private readonly IContactoRepository _repo;

        public HomeController(IContactoRepository repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            var contactos = await _repo.ObtenerTodosAsync();
            ViewBag.TotalContactos = contactos.Count();
            ViewBag.TotalCategorias = contactos
                                        .Select(c => c.Categoria)
                                        .Distinct()
                                        .Count();

            return View();
        }
    }
}