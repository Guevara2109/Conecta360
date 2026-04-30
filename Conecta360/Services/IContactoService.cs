using Conect360.Models;

namespace Conect360.Services
{
    public interface IContactoService
    {
        Task<List<Contacto>> ObtenerTodosAsync();
        Task<Contacto?> ObtenerPorIdAsync(int id);
        Task<List<Contacto>> BuscarAsync(string? buscar, string? categoria);

        Task<ServiceResult> CrearAsync(Contacto contacto);
        Task<ServiceResult> ActualizarAsync(int id, Contacto contacto);
        Task<ServiceResult> EliminarAsync(int id);

        Task<int> ContarAsync();
        Task<int> ContarCategoriasAsync();
    }
}