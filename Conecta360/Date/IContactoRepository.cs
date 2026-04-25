using Conect360.Models;

namespace Conect360.Data
{
    public interface IContactoRepository
    {
        // Consultar
        Task<List<Contacto>> ObtenerTodosAsync();
        Task<Contacto?> ObtenerPorIdAsync(int id);
        Task<List<Contacto>> BuscarAsync(string? buscar, string? categoria);

        // Crear
        Task<Contacto> CrearAsync(Contacto contacto);

        // Actualizar
        Task<bool> ActualizarAsync(Contacto contacto);

        // Eliminar
        Task<bool> EliminarAsync(int id);

        // Stats
        Task<int> ContarAsync();
        Task<int> ContarCategoriasAsync();
    }
}
