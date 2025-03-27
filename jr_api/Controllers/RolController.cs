using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using jr_api.Models;  // Asegúrate de importar el contexto de tu base de datos
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[ApiController]
[Route("api/[controller]")]
public class RolController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RolController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("get-roles")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _context.Roles
            .Select(r => new
            {
                r.RolId,
                r.NombreRol,

                // Obtener las vistas asociadas a cada rol
                Vistas = _context.RolVistas
                    .Where(rv => rv.RolId == r.RolId)
                    .Select(rv => new
                    {
                        rv.Vista.VistaId,
                        rv.Vista.NombreVista,
                        rv.Vista.Ruta
                    }).ToList(),

                // Obtener los permisos asociados a cada rol y a cada vista
                Permisos = _context.RolPermisos
                    .Where(rp => rp.RolId == r.RolId)
                    .Select(rp => new
                    {
                        rp.Permiso.PermisoId,
                        rp.Permiso.DescripcionPermiso,
                        Vista = rp.Vista != null ? new
                        {
                            rp.Vista.VistaId,
                            rp.Vista.NombreVista
                        } : null // Si el permiso no está asociado a una vista
                    }).ToList()
            })
            .ToListAsync();

        return Ok(roles);
    }

    [HttpGet("permisos")]
    public async Task<IActionResult> GetPermisos()
    {
        var permisos = await _context.Permisos
            .Select(p => new
            {
                p.PermisoId,
                p.DescripcionPermiso
            })
            .ToListAsync();

        return Ok(permisos);
    }

    [HttpPost("guardar-rol")]
    public async Task<IActionResult> GuardarRol([FromBody] GuardarRolRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.NombreRol) || request.Vistas == null)
        {
            return BadRequest("Datos inválidos.");
        }

        // 1️⃣ Verificar si el rol ya existe
        var rol = await _context.Roles.FirstOrDefaultAsync(r => r.RolId == request.RolId);

        if (rol == null && request.RolId > 0)
        {
            return NotFound("El rol especificado no existe.");
        }

        if (rol == null)
        {
            // Si el rol no existe y el ID es 0, se crea un nuevo rol
            rol = new Rol { NombreRol = request.NombreRol };
            _context.Roles.Add(rol);
            await _context.SaveChangesAsync(); // Guardamos para obtener su ID
        }
        else
        {
            // Si el rol ya existe, actualizamos su nombre
            rol.NombreRol = request.NombreRol;
            _context.Roles.Update(rol);
        }

        await _context.SaveChangesAsync();

        // 2️⃣ ELIMINAR REGISTROS PREVIOS DEL ROL
        var permisosPrevios = await _context.RolPermisos.Where(rp => rp.RolId == rol.RolId).ToListAsync();
        var vistasPrevias = await _context.RolVistas.Where(rv => rv.RolId == rol.RolId).ToListAsync();

        _context.RolPermisos.RemoveRange(permisosPrevios);
        _context.RolVistas.RemoveRange(vistasPrevias);

        await _context.SaveChangesAsync();

        // 3️⃣ GUARDAR LAS NUEVAS VISTAS Y PERMISOS
        var nuevasVistas = new List<RolVista>();
        var nuevosPermisos = new List<RolPermiso>();

        foreach (var vistaDto in request.Vistas)
        {
            // Verificar si la vista ya existe en la tabla Vistas
            var vista = await _context.Vistas.FirstOrDefaultAsync(v => v.NombreVista == vistaDto.VistaId);

            if (vista == null)
            {
                // Si la vista no existe, se crea una nueva entrada en la tabla Vistas
                vista = new Vista { NombreVista = vistaDto.VistaId, Ruta = $"/{vistaDto.VistaId.Replace('.', '/')}" };
                _context.Vistas.Add(vista);
                await _context.SaveChangesAsync(); // Guardamos para obtener el VistaId
            }

            // Guardar la relación Rol-Vista
            nuevasVistas.Add(new RolVista
            {
                RolId = rol.RolId,
                VistaId = vista.VistaId
            });

            // Guardar la relación de permisos en RolPermisos
            foreach (var permisoId in vistaDto.Permisos)
            {
                nuevosPermisos.Add(new RolPermiso
                {
                    RolId = rol.RolId,
                    VistaId = vista.VistaId,
                    PermisoId = permisoId
                });
            }
        }

        _context.RolVistas.AddRange(nuevasVistas);
        _context.RolPermisos.AddRange(nuevosPermisos);
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Rol guardado correctamente", rolId = rol.RolId });
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
