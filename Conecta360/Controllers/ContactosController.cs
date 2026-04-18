using Microsoft.AspNetCore.Mvc;
using Conect360.Models;

namespace Conect360.Controllers
{
    public class ContactosController : Controller
    {
        // ── ALMACENAMIENTO TEMPORAL EN MEMORIA ──
        // Se expone como público para que HomeController pueda leer las stats
        private static List<Contacto> _contactos = new();
        private static int _nextId = 1;

        // Método público para que otros controladores lean la lista
        public static List<Contacto> ObtenerContactos() => _contactos;

        // ── GET /Contactos ──────────────────────────────────────────────
        public IActionResult Index(string? buscar, string? categoria)
        {
            ViewBag.TotalContactos = _contactos.Count;
            ViewBag.TotalCategorias = _contactos.Select(c => c.Categoria).Distinct().Count();

            var lista = _contactos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
                lista = lista.Where(c =>
                    c.Nombre.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                    c.Apellido.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                    c.Telefono.Contains(buscar, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(categoria))
                lista = lista.Where(c => c.Categoria == categoria);

            ViewBag.Buscar = buscar;
            ViewBag.Categoria = categoria;

            return View(lista.ToList());
        }

        // ── GET /Contactos/Create ───────────────────────────────────────
        public IActionResult Create()
        {
            ViewBag.TotalContactos = _contactos.Count;
            ViewBag.TotalCategorias = _contactos.Select(c => c.Categoria).Distinct().Count();
            return View();
        }

        // ── POST /Contactos/Create ──────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Contacto contacto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.TotalContactos = _contactos.Count;
                ViewBag.TotalCategorias = _contactos.Select(c => c.Categoria).Distinct().Count();
                return View(contacto);
            }

            contacto.Id = _nextId++;
            contacto.FechaRegistro = DateTime.Now;
            _contactos.Add(contacto);

            TempData["Mensaje"] = "Contacto registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ── GET /Contactos/Edit/5 ───────────────────────────────────────
        public IActionResult Edit(int id)
        {
            var contacto = _contactos.FirstOrDefault(c => c.Id == id);
            if (contacto == null) return NotFound();

            ViewBag.TotalContactos = _contactos.Count;
            ViewBag.TotalCategorias = _contactos.Select(c => c.Categoria).Distinct().Count();
            return View(contacto);
        }

        // ── POST /Contactos/Edit/5 ──────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Contacto contacto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.TotalContactos = _contactos.Count;
                ViewBag.TotalCategorias = _contactos.Select(c => c.Categoria).Distinct().Count();
                return View(contacto);
            }

            var existente = _contactos.FirstOrDefault(c => c.Id == id);
            if (existente == null) return NotFound();

            existente.Nombre = contacto.Nombre;
            existente.Apellido = contacto.Apellido;
            existente.Telefono = contacto.Telefono;
            existente.Email = contacto.Email;
            existente.Categoria = contacto.Categoria;
            existente.Notas = contacto.Notas;

            TempData["Mensaje"] = "Contacto actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ── POST /Contactos/Delete/5 ────────────────────────────────────
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var contacto = _contactos.FirstOrDefault(c => c.Id == id);
            if (contacto != null) _contactos.Remove(contacto);

            TempData["Mensaje"] = "Contacto eliminado.";
            return RedirectToAction(nameof(Index));
        }

        // ── GET /Contactos/Categorias ───────────────────────────────────
        public IActionResult Categorias(string? filtro)
        {
            ViewBag.TotalContactos = _contactos.Count;
            ViewBag.TotalCategorias = _contactos.Select(c => c.Categoria).Distinct().Count();
            ViewBag.Filtro = filtro;

            var lista = _contactos.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filtro))
                lista = lista.Where(c => c.Categoria == filtro);

            var grupos = lista.GroupBy(c => c.Categoria);
            return View(grupos);
        }
    }
}
