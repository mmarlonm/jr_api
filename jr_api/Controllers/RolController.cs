using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using jr_api.Models;  // Asegúrate de importar el contexto de tu base de datos
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;
using jr_api.IServices;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[ApiController]
[Route("api/[controller]")]
public class RolController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IRolService _RolService;

    public RolController(ApplicationDbContext context, IRolService rolService)
    {
        _context = context;
        _RolService = rolService;

    }

    [HttpGet("get-roles")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _RolService.GetRoles();
        return Ok(roles);
    }

    [HttpGet("permisos")]
    public async Task<IActionResult> GetPermisos()
    {
        var permisos = await _RolService.GetPermisos();
        return Ok(permisos);

    }

    [HttpPost("guardar-rol")]
    public async Task<IActionResult> GuardarRol([FromBody] GuardarRolRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.NombreRol) || request.Vistas == null)
        {
            return BadRequest("Datos inválidos.");
        }

        var permisos = await _RolService.GuardarRol(request);

        if(permisos == 0)
        {
            return BadRequest("Usuario no existe");
        }
        return Ok(permisos);


    }
}


public class GuardarRolRequest
{
    public int RolId { get; set; }  // Si es 0, se insertará un nuevo rol; si es >0, se actualizará.
    public string NombreRol { get; set; }
    public List<VistaPermisoDto> Vistas { get; set; }
}

public class VistaPermisoDto
{
    public string VistaId { get; set; }  // ID de la vista (Ej: "dashboards.analytics")
    public List<int> Permisos { get; set; }  // Lista de IDs de permisos asignados
}
