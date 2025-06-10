using Microsoft.AspNetCore.Mvc;
using System;

[ApiController]
[Route("api/[controller]")]
public class NotificacionController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public NotificacionController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("hoy")]
    public IActionResult ObtenerNotificacionesDeHoy()
    {
        var zona = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"); // Puedes usar "America/Mexico_City"
        var hoy = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zona).Date;

        var notificaciones = _context.Prospectos
            .AsEnumerable() // Ejecutar en memoria para usar DateTime.Parse
            .Where(p =>
            {
                if (string.IsNullOrEmpty(p.FechaAccion))
                    return false;

                if (DateTime.TryParse(p.FechaAccion, out var fecha))
                {
                    // Asegurar zona horaria correcta
                    var fechaLocal = TimeZoneInfo.ConvertTimeFromUtc(fecha.ToUniversalTime(), zona).Date;
                    return fechaLocal == hoy;
                }

                return false;
            })
            .Select(p => new NotificacionDto
            {
                ProspectoId = p.ProspectoId,
                Mensaje = $"Acción pendiente para prospecto: {p.Contacto}",
                FechaAccion = DateTime.Parse(p.FechaAccion)
            })
            .ToList();

        return Ok(notificaciones);
    }
}