using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using jr_api.Models;
using static ProyectoController;

[ApiController]
[Route("api/[controller]")]
public class ProspectoController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProspectoController(ApplicationDbContext context)
    {
        _context = context;
    }

    // -------------------- PROSPECTOS --------------------

    // 🔸 Obtener todos los prospectos
    [HttpGet("getAll-prospectos")]
    public async Task<IActionResult> GetAllProspectos()
    {
        var prospectos = await _context.Prospectos
            .ToListAsync();

        return Ok(prospectos);
    }

    // 🔸 Obtener un prospecto por ID
    [HttpGet("get-prospecto/{id}")]
    public async Task<IActionResult> GetProspectoById(int id)
    {
        var prospecto = await _context.Prospectos
            .FirstOrDefaultAsync(p => p.ProspectoId == id);

        if (prospecto == null)
            return NotFound("Prospecto no encontrado.");

        return Ok(prospecto);
    }

    // 🔸 Crear o actualizar un prospecto
    [HttpPost("save-prospecto")]
    public async Task<IActionResult> SaveProspecto([FromBody] Prospecto request)
    {
        if (request == null)
            return BadRequest("Datos inválidos.");

        if (request.ProspectoId == 0)
        {
            request.FechaRegistro = DateTime.UtcNow;
            _context.Prospectos.Add(request);
        }
        else
        {
            var existing = await _context.Prospectos.FindAsync(request.ProspectoId);
            if (existing == null)
                return NotFound("Prospecto no encontrado.");

            existing.Empresa = request.Empresa;
            existing.Contacto = request.Contacto;
            existing.Telefono = request.Telefono;
            existing.Puesto = request.Puesto;
            existing.GiroEmpresa = request.GiroEmpresa;
            existing.Email = request.Email;
            existing.AreaInteres = request.AreaInteres;
            existing.TipoEmpresa = request.TipoEmpresa;
            existing.UsuarioId = request.UsuarioId;
            existing.ComoSeObtuvo = request.ComoSeObtuvo;
            existing.Otros = request.Otros;
            existing.PersonalSeguimiento = request.PersonalSeguimiento;


            _context.Prospectos.Update(existing);
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Prospecto guardado correctamente." });
    }

    // 🔸 Eliminar prospecto (eliminación física, puedes hacer lógica si prefieres)
    [HttpDelete("delete-prospecto/{id}")]
    public async Task<IActionResult> DeleteProspecto(int id)
    {
        var prospecto = await _context.Prospectos.FindAsync(id);
        if (prospecto == null)
            return NotFound("Prospecto no encontrado.");

        _context.Prospectos.Remove(prospecto);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Prospecto eliminado correctamente." });
    }

    // -------------------- SEGUIMIENTOS (COMENTARIOS) --------------------

    // 🔸 Obtener todos los seguimientos (comentarios) de un prospecto
    [HttpGet("get-seguimientos/{prospectoId}")]
    public async Task<IActionResult> GetSeguimientos(int prospectoId)
    {
        var seguimientos = await _context.SeguimientoProspectos
            .Where(s => s.ProspectoId == prospectoId)
            .ToListAsync();

        return Ok(seguimientos);
    }

    // 🔸 Crear un nuevo seguimiento para un prospecto
    [HttpPost("add-seguimiento")]
    public async Task<IActionResult> AddSeguimiento([FromBody] SeguimientoProspecto request)
    {
        if (request == null)
            return BadRequest("Datos inválidos.");

        request.FechaRegistro = DateTime.UtcNow;
        _context.SeguimientoProspectos.Add(request);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Seguimiento agregado correctamente." });
    }

    // 🔸 Eliminar un seguimiento
    [HttpDelete("delete-seguimiento/{id}")]
    public async Task<IActionResult> DeleteSeguimiento(int id)
    {
        var seguimiento = await _context.SeguimientoProspectos.FindAsync(id);
        if (seguimiento == null)
            return NotFound("Seguimiento no encontrado.");

        _context.SeguimientoProspectos.Remove(seguimiento);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Seguimiento eliminado correctamente." });
    }
}