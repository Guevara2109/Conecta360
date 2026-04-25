using Microsoft.AspNetCore.Mvc;
using Conect360.Data;
using Conect360.Models;

namespace Conect360.Controllers
{
    public class ContactosController : Controller
    {
        private readonly IContactoRepository _repo;

        public ContactosController(IContactoRepository repo)
        {
            _repo = repo;
        }

        // ── GET /Contactos ──────────────────────────────────────────────
        public async Task<IActionResult> Index(string? buscar, string? categoria)
        {
            ViewBag.TotalContactos  = await _repo.ContarAsync();
            ViewBag.TotalCategorias = await _repo.ContarCategoriasAsync();
            ViewBag.Buscar          = buscar;
            ViewBag.Categoria       = categoria;

            var lista = await _repo.BuscarAsync(buscar, categoria);
            return View(lista);
        }

        // ── GET /Contactos/Create ───────────────────────────────────────
        public async Task<IActionResult> Create()
        {
            ViewBag.TotalContactos  = await _repo.ContarAsync();
            ViewBag.TotalCategorias = await _repo.ContarCategoriasAsync();
            return View();
        }

        // ── POST /Contactos/Create ──────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contacto contacto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.TotalContactos  = await _repo.ContarAsync();
                ViewBag.TotalCategorias = await _repo.ContarCategoriasAsync();
                return View(contacto);
            }

            await _repo.CrearAsync(contacto);
            TempData["Mensaje"] = "Contacto registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ── GET /Contactos/Edit/5 ───────────────────────────────────────
        public async Task<IActionResult> Edit(int id)
        {
            var contacto = await _repo.ObtenerPorIdAsync(id);
            if (contacto == null) return NotFound();

            ViewBag.TotalContactos  = await _repo.ContarAsync();
            ViewBag.TotalCategorias = await _repo.ContarCategoriasAsync();
            return View(contacto);
        }

        // ── POST /Contactos/Edit/5 ──────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contacto contacto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.TotalContactos  = await _repo.ContarAsync();
                ViewBag.TotalCategorias = await _repo.ContarCategoriasAsync();
                return View(contacto);
            }

            contacto.Id = id;
            var actualizado = await _repo.ActualizarAsync(contacto);
            if (!actualizado) return NotFound();

            TempData["Mensaje"] = "Contacto actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ── POST /Contactos/Delete/5 ────────────────────────────────────
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.EliminarAsync(id);
            TempData["Mensaje"] = "Contacto eliminado.";
            return RedirectToAction(nameof(Index));
        }

        // ── GET /Contactos/Categorias ───────────────────────────────────
        public async Task<IActionResult> Categorias(string? filtro)
        {
            ViewBag.TotalContactos  = await _repo.ContarAsync();
            ViewBag.TotalCategorias = await _repo.ContarCategoriasAsync();
            ViewBag.Filtro          = filtro;

            var lista = await _repo.BuscarAsync(null, filtro);
            var grupos = lista.GroupBy(c => c.Categoria);
            return View(grupos);
        }

        internal static IEnumerable<object> ObtenerContactos()
        {
            throw new NotImplementedException();
        }
    }
}
