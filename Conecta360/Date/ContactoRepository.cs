using Microsoft.EntityFrameworkCore;
using Conect360.Models;

namespace Conect360.Data
{
    public class ContactoRepository : IContactoRepository
    {
        private readonly Conecta360DbContext _context;

        public ContactoRepository(Conecta360DbContext context)
        {
            _context = context;
        }

        // ── CONSULTAR TODOS ─────────────────────────────────────────────
        public async Task<List<Contacto>> ObtenerTodosAsync()
        {
            return await _context.Contactos
                                 .OrderBy(c => c.Apellido)
                                 .ThenBy(c => c.Nombre)
                                 .ToListAsync();
        }

        // ── CONSULTAR POR ID ────────────────────────────────────────────
        public async Task<Contacto?> ObtenerPorIdAsync(int id)
        {
            return await _context.Contactos.FindAsync(id);
        }

        // ── BUSCAR / FILTRAR ────────────────────────────────────────────
        public async Task<List<Contacto>> BuscarAsync(string? buscar, string? categoria)
        {
            var query = _context.Contactos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                string termino = buscar.ToLower();
                query = query.Where(c =>
                    c.Nombre.ToLower().Contains(termino) ||
                    c.Apellido.ToLower().Contains(termino) ||
                    c.Telefono.Contains(termino));
            }

            if (!string.IsNullOrWhiteSpace(categoria))
            {
                query = query.Where(c => c.Categoria == categoria);
            }

            return await query
                         .OrderBy(c => c.Apellido)
                         .ThenBy(c => c.Nombre)
                         .ToListAsync();
        }

        // ── CREAR ───────────────────────────────────────────────────────
        public async Task<Contacto> CrearAsync(Contacto contacto)
        {
            contacto.FechaRegistro = DateTime.Now;
            _context.Contactos.Add(contacto);
            await _context.SaveChangesAsync();
            return contacto;
        }

        // ── ACTUALIZAR ──────────────────────────────────────────────────
        public async Task<bool> ActualizarAsync(Contacto contacto)
        {
            var existente = await _context.Contactos.FindAsync(contacto.Id);
            if (existente == null) return false;

            existente.Nombre    = contacto.Nombre;
            existente.Apellido  = contacto.Apellido;
            existente.Telefono  = contacto.Telefono;
            existente.Email     = contacto.Email;
            existente.Categoria = contacto.Categoria;
            existente.Notas     = contacto.Notas;

            await _context.SaveChangesAsync();
            return true;
        }

        // ── ELIMINAR ────────────────────────────────────────────────────
        public async Task<bool> EliminarAsync(int id)
        {
            var contacto = await _context.Contactos.FindAsync(id);
            if (contacto == null) return false;

            _context.Contactos.Remove(contacto);
            await _context.SaveChangesAsync();
            return true;
        }

        // ── ESTADÍSTICAS ────────────────────────────────────────────────
        public async Task<int> ContarAsync()
        {
            return await _context.Contactos.CountAsync();
        }

        public async Task<int> ContarCategoriasAsync()
        {
            return await _context.Contactos
                                 .Select(c => c.Categoria)
                                 .Distinct()
                                 .CountAsync();
        }
    }
}
