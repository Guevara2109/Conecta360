using System.Text.RegularExpressions;
using Conect360.Data;
using Conect360.Models;

namespace Conect360.Services
{
    public class ContactoService : IContactoService
    {
        private readonly IContactoRepository _repo;

        // Categorías válidas definidas como regla de negocio
        private static readonly HashSet<string> CategoriasValidas =
            new() { "General", "Familia", "Trabajo", "Amigos", "Favoritos" };

        public ContactoService(IContactoRepository repo)
        {
            _repo = repo;
        }

        // ── CONSULTAS (sin lógica extra, delegan directo) ────────────────
        public Task<List<Contacto>> ObtenerTodosAsync() => _repo.ObtenerTodosAsync();
        public Task<Contacto?> ObtenerPorIdAsync(int id) => _repo.ObtenerPorIdAsync(id);
        public Task<List<Contacto>> BuscarAsync(string? buscar, string? categoria)
            => _repo.BuscarAsync(buscar, categoria);
        public Task<int> ContarAsync() => _repo.ContarAsync();
        public Task<int> ContarCategoriasAsync() => _repo.ContarCategoriasAsync();

        // ── CREAR ────────────────────────────────────────────────────────
        public async Task<ServiceResult> CrearAsync(Contacto contacto)
        {
            // 1. Validar campos obligatorios vacíos
            var vacios = ValidarCamposObligatorios(contacto);
            if (vacios != null) return vacios;

            // 2. Validar longitudes
            var longitudes = ValidarLongitudes(contacto);
            if (longitudes != null) return longitudes;

            // 3. Validar formato de teléfono
            var telefono = ValidarTelefono(contacto.Telefono);
            if (telefono != null) return telefono;

            // 4. Validar formato de correo (si se proporcionó)
            if (!string.IsNullOrWhiteSpace(contacto.Email))
            {
                var email = ValidarEmail(contacto.Email);
                if (email != null) return email;
            }

            // 5. Validar categoría permitida
            var cat = ValidarCategoria(contacto.Categoria);
            if (cat != null) return cat;

            // 6. Evitar duplicados: mismo nombre + apellido + teléfono
            var duplicado = await VerificarDuplicadoAsync(contacto, null);
            if (duplicado != null) return duplicado;

            // 7. Notas no deben superar 500 caracteres
            if (!string.IsNullOrEmpty(contacto.Notas) && contacto.Notas.Length > 500)
                return ServiceResult.Error("Las notas no pueden superar los 500 caracteres.");

            // Todo OK → guardar
            contacto.FechaRegistro = DateTime.Now;
            await _repo.CrearAsync(contacto);
            return ServiceResult.Ok("Contacto registrado correctamente.");
        }

        // ── ACTUALIZAR ───────────────────────────────────────────────────
        public async Task<ServiceResult> ActualizarAsync(int id, Contacto contacto)
        {
            // Verificar que el contacto exista
            var existente = await _repo.ObtenerPorIdAsync(id);
            if (existente == null)
                return ServiceResult.Error("El contacto que intentas editar no existe.");

            // Mismas validaciones que al crear
            var vacios = ValidarCamposObligatorios(contacto);
            if (vacios != null) return vacios;

            var longitudes = ValidarLongitudes(contacto);
            if (longitudes != null) return longitudes;

            var telefono = ValidarTelefono(contacto.Telefono);
            if (telefono != null) return telefono;

            if (!string.IsNullOrWhiteSpace(contacto.Email))
            {
                var email = ValidarEmail(contacto.Email);
                if (email != null) return email;
            }

            var cat = ValidarCategoria(contacto.Categoria);
            if (cat != null) return cat;

            // Duplicado excluyendo el registro actual
            var duplicado = await VerificarDuplicadoAsync(contacto, id);
            if (duplicado != null) return duplicado;

            if (!string.IsNullOrEmpty(contacto.Notas) && contacto.Notas.Length > 500)
                return ServiceResult.Error("Las notas no pueden superar los 500 caracteres.");

            contacto.Id = id;
            await _repo.ActualizarAsync(contacto);
            return ServiceResult.Ok("Contacto actualizado correctamente.");
        }

        // ── ELIMINAR ─────────────────────────────────────────────────────
        public async Task<ServiceResult> EliminarAsync(int id)
        {
            var existente = await _repo.ObtenerPorIdAsync(id);
            if (existente == null)
                return ServiceResult.Error("El contacto que intentas eliminar no existe.");

            await _repo.EliminarAsync(id);
            return ServiceResult.Ok($"Contacto '{existente.Nombre} {existente.Apellido}' eliminado.");
        }

        // ════════════════════════════════════════════════════════════════
        // MÉTODOS PRIVADOS DE VALIDACIÓN
        // ════════════════════════════════════════════════════════════════

        private static ServiceResult? ValidarCamposObligatorios(Contacto c)
        {
            if (string.IsNullOrWhiteSpace(c.Nombre))
                return ServiceResult.Error("El nombre es obligatorio y no puede estar vacío.");
            if (string.IsNullOrWhiteSpace(c.Apellido))
                return ServiceResult.Error("El apellido es obligatorio y no puede estar vacío.");
            if (string.IsNullOrWhiteSpace(c.Telefono))
                return ServiceResult.Error("El teléfono es obligatorio y no puede estar vacío.");
            return null;
        }

        private static ServiceResult? ValidarLongitudes(Contacto c)
        {
            if (c.Nombre.Trim().Length < 2)
                return ServiceResult.Error("El nombre debe tener al menos 2 caracteres.");
            if (c.Nombre.Trim().Length > 100)
                return ServiceResult.Error("El nombre no puede superar los 100 caracteres.");
            if (c.Apellido.Trim().Length < 2)
                return ServiceResult.Error("El apellido debe tener al menos 2 caracteres.");
            if (c.Apellido.Trim().Length > 100)
                return ServiceResult.Error("El apellido no puede superar los 100 caracteres.");
            if (c.Telefono.Trim().Length > 20)
                return ServiceResult.Error("El teléfono no puede superar los 20 caracteres.");
            return null;
        }

        private static ServiceResult? ValidarTelefono(string telefono)
        {
            // Acepta formatos: +503 7123-4567 / 71234567 / (503)71234567
            var limpio = Regex.Replace(telefono, @"[\s\-\(\)\+]", "");
            if (!Regex.IsMatch(limpio, @"^\d{7,15}$"))
                return ServiceResult.Error(
                    "El teléfono tiene un formato inválido. Solo se permiten números, espacios, guiones y el símbolo +.");
            return null;
        }

        private static ServiceResult? ValidarEmail(string email)
        {
            // Validación estricta de formato de correo
            var patron = @"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$";
            if (!Regex.IsMatch(email.Trim(), patron, RegexOptions.IgnoreCase))
                return ServiceResult.Error(
                    "El correo electrónico no tiene un formato válido. Ejemplo: nombre@dominio.com");
            return null;
        }

        private static ServiceResult? ValidarCategoria(string categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria))
                return ServiceResult.Error("Debes seleccionar una categoría.");
            if (!CategoriasValidas.Contains(categoria))
                return ServiceResult.Error(
                    $"La categoría '{categoria}' no es válida. Categorías permitidas: {string.Join(", ", CategoriasValidas)}.");
            return null;
        }

        private async Task<ServiceResult?> VerificarDuplicadoAsync(Contacto nuevo, int? idExcluir)
        {
            var todos = await _repo.ObtenerTodosAsync();
            var existe = todos.Any(c =>
                c.Nombre.Trim().ToLower() == nuevo.Nombre.Trim().ToLower() &&
                c.Apellido.Trim().ToLower() == nuevo.Apellido.Trim().ToLower() &&
                c.Telefono.Trim() == nuevo.Telefono.Trim() &&
                c.Id != idExcluir);

            if (existe)
                return ServiceResult.Error(
                    $"Ya existe un contacto con el nombre '{nuevo.Nombre} {nuevo.Apellido}' y ese número de teléfono.");
            return null;
        }
    }
}