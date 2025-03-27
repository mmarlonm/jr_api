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
    .GroupJoin(
        _context.EstatusCotizacion,
        c => c.Estatus,
        e => e.Id,
        (c, estatusGroup) => new { c, estatusGroup }
    )
    .SelectMany(
        x => x.estatusGroup.DefaultIfEmpty(), // LEFT JOIN con EstatusCotizacion
        (x, e) => new { x.c, Estatus = e != null ? e.Nombre : "Sin Estatus" }
    )
    .GroupJoin(
        _context.Clientes,  // Tabla de clientes
        c => c.c.Cliente,   // Clave foránea en Cotizaciones
        cl => cl.ClienteId, // Clave primaria en Clientes
        (c, clienteGroup) => new { c, clienteGroup }
    )
    .SelectMany(
        x => x.clienteGroup.DefaultIfEmpty(), // LEFT JOIN con Clientes
        (x, cl) => new
        {
            x.c.c.CotizacionId,
            x.c.c.Cliente,
            NombreCliente = cl != null ? cl.Nombre : "Sin Cliente", // Nombre del Cliente
            x.c.c.Prospecto,
            x.c.c.UsuarioCreadorId,
            x.c.c.Necesidad,
            x.c.c.Direccion,
            x.c.c.NombreContacto,
            x.c.c.Telefono,
            x.c.c.Empresa,
            x.c.c.Cotizacion,
            x.c.c.OrdenCompra,
            x.c.c.Contrato,
            x.c.c.Proveedor,
            x.c.c.Vendedor,
            x.c.c.FechaEntrega,
            x.c.c.RutaCritica,
            x.c.c.Factura,
            x.c.c.Pago,
            x.c.c.UtilidadProgramada,
            x.c.c.UtilidadReal,
            x.c.c.Financiamiento,
            x.c.c.FechaRegistro,
            x.c.Estatus
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
    [HttpDelete("eliminar-cotizacion/{id}")]
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