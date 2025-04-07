using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using jr_api.Models;
using static ProyectoController;
using jr_api.IServices;

[ApiController]
[Route("api/[controller]")]
public class ProspectoController : ControllerBase

{
    private readonly ApplicationDbContext _context;
    private readonly IProspectoService _ProspectoService;


    public ProspectoController(ApplicationDbContext context, IProspectoService prospectoService)
    {
        _context = context;
        _ProspectoService = prospectoService;

    }

    // -------------------- PROSPECTOS --------------------

    // 🔸 Obtener todos los prospectos
    [HttpGet("getAll-prospectos")]
    public async Task<IActionResult> GetAllProspectos()
    {
        var prospectos = await _ProspectoService.GetAllProspectos();

        return Ok(prospectos);
    }

    // 🔸 Obtener un prospecto por ID
    [HttpGet("get-prospecto/{id}")]
    public async Task<IActionResult> GetProspectoById(int id)
    {
        var prospecto = await _ProspectoService.GetProspectoById(id);
       if (prospecto == null)
        {
            return NotFound("No existe prospecto");
        }
        return Ok(prospecto);
    }

    // 🔸 Crear o actualizar un prospecto
    [HttpPost("save-prospecto")]
    public async Task<IActionResult> SaveProspecto([FromBody] Prospecto request)
    {
        if (request == null)
            return BadRequest("Datos inválidos.");
        var prospecto = await _ProspectoService.SaveProspecto(request);
        if (prospecto == null)
            return BadRequest("El prospecto no existe");


        return Ok(new { message = "Prospecto guardado correctamente." });
    }

    // 🔸 Eliminar prospecto (eliminación física, puedes hacer lógica si prefieres)
    [HttpDelete("delete-prospecto/{id}")]
    public async Task<IActionResult> DeleteProspecto(int id)
    {
        var prospecto = await _ProspectoService.DeleteProspecto(id);
        if (prospecto == null)
        {
            return NotFound("Prospecto no encontrado.");

        }
        return Ok(prospecto);
    }

    // -------------------- SEGUIMIENTOS (COMENTARIOS) --------------------

    // 🔸 Obtener todos los seguimientos (comentarios) de un prospecto
    [HttpGet("get-seguimientos/{prospectoId}")]
    public async Task<IActionResult> GetSeguimientos(int prospectoId)
    {
        var seguimientos = await _ProspectoService.GetSeguimientos(prospectoId);       
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

    /// <summary>
    /// Obtiene todas las notas de un prospecto específico.
    /// </summary>
    [HttpGet("notas/{prospectoId}")]
    public async Task<IActionResult> GetNotasByProspecto(int prospectoId)
    {
        var notas = await _ProspectoService.GetNotasByProspecto(prospectoId);

        return Ok(notas);
    }

    /// <summary>
    /// Crea o actualiza una nota de prospecto.
    /// </summary>
    [HttpPost]
    [HttpPost("save-notes")]
    public async Task<IActionResult> SaveNota([FromBody] NotaProspectoDto notaDto)
    {
        if (notaDto == null)
            return BadRequest("Datos inválidos.");

        var notaExistente = await _ProspectoService.SaveNota(notaDto);
            if (notaExistente == null)
                return NotFound("Nota no encontrada.");

            return Ok(new { message = "Nota actualizada exitosamente" });
        
    }

    /// <summary>
    /// Elimina una nota de prospecto por su ID.
    /// </summary>
    [HttpDelete("delete-nota/{notaId}")]
    public async Task<IActionResult> DeleteNota(int notaId)
    {
        var nota = await _ProspectoService.DeleteNota (notaId);
        if (nota == null)
        {
            return NotFound(new { message = "Nota no encontrada." });
        }
        return Ok(new { message = "Nota eliminada exitosamente." });
    }
}