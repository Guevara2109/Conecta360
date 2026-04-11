using Microsoft.AspNetCore.Mvc;
using Conect360.Models;

namespace Conect360.Controllers
{
    public class ContactosController : Controller
    {
        // ── ALMACENAMIENTO TEMPORAL EN MEMORIA ──
        // TODO: reemplazar con ApplicationDbContext + Entity Framework Core
        private static List<Contacto> _contactos = new();
        private static int _nextId = 1;

        // ── GET /Contactos ──────────────────────────────────────────────
        public IActionResult Index()
        {
            // TODO: return View(_context.Contactos.ToList());
            return View(_contactos);
        }

        // ── GET /Contactos/Create ───────────────────────────────────────
        public IActionResult Create()
        {
            return View();
        }

        // ── POST /Contactos/Create ──────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Contacto contacto)
        {
            if (!ModelState.IsValid)
                return View(contacto);

            // TODO: _context.Contactos.Add(contacto); await _context.SaveChangesAsync();
            contacto.Id = _nextId++;
            contacto.FechaRegistro = DateTime.Now;
            _contactos.Add(contacto);

            TempData["Mensaje"] = "Contacto registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ── GET /Contactos/Details/5 ────────────────────────────────────
        public IActionResult Details(int id)
        {
            // TODO: var contacto = await _context.Contactos.FindAsync(id);
            var contacto = _contactos.FirstOrDefault(c => c.Id == id);
            if (contacto == null) return NotFound();
            return View(contacto);
        }

        // ── GET /Contactos/Edit/5 ───────────────────────────────────────
        public IActionResult Edit(int id)
        {
            var contacto = _contactos.FirstOrDefault(c => c.Id == id);
            if (contacto == null) return NotFound();
            return View(contacto);
        }

        // ── POST /Contactos/Edit/5 ──────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Contacto contacto)
        {
            if (!ModelState.IsValid)
                return View(contacto);

            var existente = _contactos.FirstOrDefault(c => c.Id == id);
            if (existente == null) return NotFound();

            // TODO: _context.Update(contacto); await _context.SaveChangesAsync();
            existente.Nombre    = contacto.Nombre;
            existente.Apellido  = contacto.Apellido;
            existente.Telefono  = contacto.Telefono;
            existente.Email     = contacto.Email;
            existente.Categoria = contacto.Categoria;
            existente.Notas     = contacto.Notas;

            TempData["Mensaje"] = "Contacto actualizado.";
            return RedirectToAction(nameof(Index));
        }

        // ── GET /Contactos/Delete/5 ─────────────────────────────────────
        public IActionResult Delete(int id)
        {
            var contacto = _contactos.FirstOrDefault(c => c.Id == id);
            if (contacto == null) return NotFound();
            return View(contacto);
        }

        // ── POST /Contactos/Delete/5 ────────────────────────────────────
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // TODO: var contacto = await _context.Contactos.FindAsync(id);
            //       _context.Contactos.Remove(contacto); await _context.SaveChangesAsync();
            var contacto = _contactos.FirstOrDefault(c => c.Id == id);
            if (contacto != null) _contactos.Remove(contacto);

            TempData["Mensaje"] = "Contacto eliminado.";
            return RedirectToAction(nameof(Index));
        }

        // ── GET /Contactos/Categorias ───────────────────────────────────
        public IActionResult Categorias()
        {
            // TODO: agrupar desde _context.Contactos.GroupBy(c => c.Categoria)
            var grupos = _contactos.GroupBy(c => c.Categoria);
            return View(grupos);
        }
    }
}
