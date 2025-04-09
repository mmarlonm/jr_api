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


    public CotizacionController(ApplicationDbContext context, ICotizacionService cotizacionService)
    {
        _context = context;
        _CotizacionService = cotizacionService;
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

        if (cotizacion == null)
                return NotFound("Cotización no encontrada.");

        return Ok(new { Message = "Cotización guardada correctamente", CotizacionId = cotizacion });
    }

    // Eliminar una cotización
    [HttpDelete("eliminar-cotizacion/{id}")]
    public async Task<IActionResult> DeleteCotizacion(int id)
    {
        var cotizacion = await _CotizacionService.DeleteCotizacion(id);
        if (cotizacion == null)
            return NotFound("Cotización no encontrada.");
        return Ok(new { Message = "Cotización eliminada correctamente" });
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

}