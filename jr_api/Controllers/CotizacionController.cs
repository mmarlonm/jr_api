using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class CotizacionController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ICotizacionService _CotizacionService;
    private readonly IConfiguration _configuration;


    public CotizacionController(ApplicationDbContext context, ICotizacionService cotizacionService, IConfiguration configuration)
    {
        _context = context;
        _CotizacionService = cotizacionService;
        _configuration = configuration;
    }

    // Obtener todas las cotizaciones
    [HttpGet("cotizaciones")]
    public async Task<IActionResult> GetCotizaciones()
    {
        var cotizaciones = await _CotizacionService.GetCotizaciones();
        
        return Ok(cotizaciones);
    }

    // Obtener una cotización por ID
    [HttpGet("cotizacion/{id}")]
    public async Task<IActionResult> GetCotizacionById(int id)
    {
        var cotizacion = await _CotizacionService.GetCotizacionById(id);
        if (cotizacion == null)
            return NotFound("Cotización no encontrada.");

        return Ok(cotizacion);
    }

    // Crear o actualizar una cotización
    [HttpPost("guardar-cotizacion")]
    public async Task<IActionResult> SaveCotizacion([FromBody] CotizacionesDTO cotizacionDto)
    {
        if (cotizacionDto == null)
            return BadRequest("Datos inválidos.");
        var cotizacion = await _CotizacionService.SaveCotizacion(cotizacionDto);

        return Ok(cotizacion);
    }

    // Eliminar una cotización
    [HttpDelete("eliminar-cotizacion/{id}")]
    public async Task<IActionResult> DeleteCotizacion(int id)
    {
        var cotizacion = await _CotizacionService.DeleteCotizacion(id);
        return Ok(cotizacion);
    }
    [HttpGet("estatus")]
    public async Task<ActionResult<IEnumerable<EstatusCotizacion>>> GetEstatusCotizaciones()
    {
        var estatus = await _CotizacionService.GetEstatusCotizaciones();
        return Ok(estatus);
    }

    [HttpGet("historial-estatus/{id}")]
    public async Task<IActionResult> GetHistorialEstatus(int id)
    {
        var historial = await _CotizacionService.GetHistorialEstatus(id);
        return Ok(historial);
    }

    [HttpPost("SubirArchivoCotizacion")]
    public async Task<IActionResult> SubirArchivoCotizacion([FromForm] int cotizacionId, [FromForm] string categoria, [FromForm] IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return BadRequest("Archivo no proporcionado.");

        var cotizacion = await _context.Cotizaciones.FindAsync(cotizacionId);
        if (cotizacion == null)
            return NotFound("Cotización no encontrada.");

        try
        {
            var rutaBase = _configuration["RutaArchivos"]; // Reutiliza la misma clave
            var folderPath = Path.Combine(rutaBase, "Cotizaciones", cotizacionId.ToString(), categoria);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = Path.GetFileName(archivo.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            var rutaRelativa = Path.Combine("Cotizaciones", cotizacionId.ToString(), categoria, fileName).Replace("\\", "/");

            var archivoCotizacion = new CotizacionArchivo
            {
                CotizacionId = cotizacionId,
                Categoria = categoria,
                NombreArchivo = fileName,
                RutaArchivo = rutaRelativa,
                FechaSubida = DateTime.Now
            };

            _context.CotizacionArchivo.Add(archivoCotizacion);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Archivo subido exitosamente", ruta = rutaRelativa });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al subir archivo: {ex.Message}");
        }
    }

    [HttpGet("ObtenerArchivosCotizacion/{cotizacionId}")]
    public async Task<IActionResult> ObtenerArchivosCotizacion(int cotizacionId)
    {
        var archivos = await _context.CotizacionArchivo
            .Where(a => a.CotizacionId == cotizacionId)
            .OrderByDescending(a => a.FechaSubida)
            .ToListAsync();

        return Ok(archivos); // Devuelve array vacío si no hay archivos
    }

    [HttpGet("DescargarArchivoCotizacion/{cotizacionId}/{categoria}/{nombreArchivo}")]
    public IActionResult DescargarArchivoCotizacion(int cotizacionId, string categoria, string nombreArchivo)
    {
        try
        {
            var rutaBase = _configuration["RutaArchivos"];
            var rutaRelativa = Path.Combine("Cotizaciones", cotizacionId.ToString(), categoria, nombreArchivo).Replace("\\", "/");
            var filePath = Path.Combine(rutaBase, rutaRelativa);

            if (!System.IO.File.Exists(filePath))
                return NotFound("Archivo no encontrado.");

            var archivoBytes = System.IO.File.ReadAllBytes(filePath);
            return File(archivoBytes, "application/octet-stream", nombreArchivo);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al descargar archivo: {ex.Message}");
        }
    }

    [HttpDelete("EliminarArchivoCotizacion/{cotizacionId}/{categoria}/{nombreArchivo}")]
    public async Task<IActionResult> EliminarArchivoCotizacion(int cotizacionId, string categoria, string nombreArchivo)
    {
        try
        {
            var rutaBase = _configuration["RutaArchivos"];
            var rutaRelativa = Path.Combine("Cotizaciones", cotizacionId.ToString(), categoria, nombreArchivo).Replace("\\", "/");
            var filePath = Path.Combine(rutaBase, rutaRelativa);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            var archivoBD = await _context.CotizacionArchivo
                .FirstOrDefaultAsync(a =>
                    a.CotizacionId == cotizacionId &&
                    a.Categoria == categoria &&
                    a.NombreArchivo == nombreArchivo);

            if (archivoBD != null)
            {
                _context.CotizacionArchivo.Remove(archivoBD);
                await _context.SaveChangesAsync();
            }

            return Ok("Archivo eliminado correctamente.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al eliminar archivo: {ex.Message}");
        }
    }

}