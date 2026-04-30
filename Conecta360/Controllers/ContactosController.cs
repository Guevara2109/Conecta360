using Microsoft.AspNetCore.Mvc;
using Conect360.Models;
using Conect360.Services;

namespace Conect360.Controllers
{
    public class ContactosController : Controller
    {
        private readonly IContactoService _service;

        public ContactosController(IContactoService service)
        {
            _service = service;
        }

        // ── GET /Contactos ──────────────────────────────────────────────
        public async Task<IActionResult> Index(string? buscar, string? categoria)
        {
            ViewBag.TotalContactos = await _service.ContarAsync();
            ViewBag.TotalCategorias = await _service.ContarCategoriasAsync();
            ViewBag.Buscar = buscar;
            ViewBag.Categoria = categoria;

            var lista = await _service.BuscarAsync(buscar, categoria);
            return View(lista);
        }

        // ── GET /Contactos/Create ───────────────────────────────────────
        public async Task<IActionResult> Create()
        {
            ViewBag.TotalContactos = await _service.ContarAsync();
            ViewBag.TotalCategorias = await _service.ContarCategoriasAsync();
            return View();
        }

        // ── POST /Contactos/Create ──────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contacto contacto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.TotalContactos = await _service.ContarAsync();
                ViewBag.TotalCategorias = await _service.ContarCategoriasAsync();
                return View(contacto);
            }

            var resultado = await _service.CrearAsync(contacto);

            if (!resultado.Exito)
            {
                ModelState.AddModelError(string.Empty, resultado.Mensaje);
                ViewBag.TotalContactos = await _service.ContarAsync();
                ViewBag.TotalCategorias = await _service.ContarCategoriasAsync();
                return View(contacto);
            }

            TempData["Mensaje"] = resultado.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        // ── GET /Contactos/Edit/5 ───────────────────────────────────────
        public async Task<IActionResult> Edit(int id)
        {
            var contacto = await _service.ObtenerPorIdAsync(id);
            if (contacto == null) return NotFound();

            ViewBag.TotalContactos = await _service.ContarAsync();
            ViewBag.TotalCategorias = await _service.ContarCategoriasAsync();
            return View(contacto);
        }

        // ── POST /Contactos/Edit/5 ──────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contacto contacto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.TotalContactos = await _service.ContarAsync();
                ViewBag.TotalCategorias = await _service.ContarCategoriasAsync();
                return View(contacto);
            }

            var resultado = await _service.ActualizarAsync(id, contacto);

            if (!resultado.Exito)
            {
                ModelState.AddModelError(string.Empty, resultado.Mensaje);
                ViewBag.TotalContactos = await _service.ContarAsync();
                ViewBag.TotalCategorias = await _service.ContarCategoriasAsync();
                return View(contacto);
            }

            TempData["Mensaje"] = resultado.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        // ── POST /Contactos/Delete/5 ────────────────────────────────────
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var resultado = await _service.EliminarAsync(id);
            TempData["Mensaje"] = resultado.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        // ── GET /Contactos/Categorias ───────────────────────────────────
        public async Task<IActionResult> Categorias(string? filtro)
        {
            ViewBag.TotalContactos = await _service.ContarAsync();
            ViewBag.TotalCategorias = await _service.ContarCategoriasAsync();
            ViewBag.Filtro = filtro;

            var lista = await _service.BuscarAsync(null, filtro);
            var grupos = lista.GroupBy(c => c.Categoria);
            return View(grupos);
        }
    }
}