namespace Conect360.Services
{
    public class ServiceResult
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;

        public static ServiceResult Ok(string mensaje = "Operación exitosa.")
            => new ServiceResult { Exito = true, Mensaje = mensaje };

        public static ServiceResult Error(string mensaje)
            => new ServiceResult { Exito = false, Mensaje = mensaje };
    }
}