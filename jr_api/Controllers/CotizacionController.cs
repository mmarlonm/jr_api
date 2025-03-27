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

    public CotizacionController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Obtener todas las cotizaciones
    [HttpGet("cotizaciones")]
public async Task<IActionResult> GetCotizaciones()
{
    var cotizaciones = await _context.Cotizaciones
        .Join(
            _context.EstatusCotizacion,
            c => c.Estatus,
            e => e.Id,
            (c, e) => new
            {
                c.CotizacionId,
                c.Cliente,
                c.Prospecto,
                c.UsuarioCreadorId,
                c.Necesidad,
                c.Direccion,
                c.NombreContacto,
                c.Telefono,
                c.Empresa,
                c.Cotizacion,
                c.OrdenCompra,
                c.Contrato,
                c.Proveedor,
                c.Vendedor,
                c.FechaEntrega,
                c.RutaCritica,
                c.Factura,
                c.Pago,
                c.UtilidadProgramada,
                c.UtilidadReal,
                c.Financiamiento,
                c.FechaRegistro,
                Estatus = e.Nombre // 🔹 Agrega el nombre del estatus en lugar del ID
            }
        )
        .ToListAsync();

    return Ok(cotizaciones);
}

    // Obtener una cotización por ID
    [HttpGet("cotizacion/{id}")]
    public async Task<IActionResult> GetCotizacionById(int id)
    {
        var cotizacion = await _context.Cotizaciones.FindAsync(id);
        if (cotizacion == null)
            return NotFound("Cotización no encontrada.");

        return Ok(cotizacion);
    }

    // Crear o actualizar una cotización
    [HttpPost("guardar-cotizacion")]
    public async Task<IActionResult> SaveCotizacion([FromBody] Cotizaciones cotizacion)
    {
        if (cotizacion == null)
            return BadRequest("Datos inválidos.");

        if (cotizacion.CotizacionId == 0)
        {
            _context.Cotizaciones.Add(cotizacion);
        }
        else
        {
            _context.Cotizaciones.Update(cotizacion);
        }

        await _context.SaveChangesAsync();
        return Ok(new { Message = "Cotización guardada correctamente", CotizacionId = cotizacion.CotizacionId });
    }

    // Eliminar una cotización
    [HttpDelete("eliminar-cotizaciones/{id}")]
    public async Task<IActionResult> DeleteCotizacion(int id)
    {
        var cotizacion = await _context.Cotizaciones.FindAsync(id);
        if (cotizacion == null)
            return NotFound("Cotización no encontrada.");

        _context.Cotizaciones.Remove(cotizacion);
        await _context.SaveChangesAsync();
        return Ok(new { Message = "Cotización eliminada correctamente" });
    }
    [HttpGet("estatus")]
    public async Task<ActionResult<IEnumerable<EstatusCotizacion>>> GetEstatusCotizaciones()
    {
        var estatus = await _context.EstatusCotizacion.ToListAsync();
        return Ok(estatus);
    }
}