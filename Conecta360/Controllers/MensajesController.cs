using Microsoft.AspNetCore.Mvc;

namespace Conect360.Controllers
{
    public class MensajesController : Controller
    {
        // ── GET /Mensajes ───────────────────────────────────────────────
        public IActionResult Index()
        {
            // TODO: cargar mensajes recientes desde base de datos
            return View();
        }

        // ── POST /Mensajes/Enviar ───────────────────────────────────────
        [HttpPost]
        public IActionResult Enviar(string numero, string texto)
        {
            if (string.IsNullOrWhiteSpace(numero))
                return BadRequest("Número requerido.");

            // TODO: integrar con servicio de mensajería (SMS, WhatsApp API, etc.)
            TempData["Mensaje"] = $"Mensaje enviado a {numero}.";
            return RedirectToAction(nameof(Index));
        }

        // ── POST /Mensajes/Llamar ───────────────────────────────────────
        [HttpPost]
        public IActionResult Llamar(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                return BadRequest("Número requerido.");

            // TODO: integrar con Twilio u otro proveedor de VoIP
            TempData["Mensaje"] = $"Llamando a {numero}...";
            return RedirectToAction(nameof(Index));
        }
    }
}
