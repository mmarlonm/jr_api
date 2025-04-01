using jr_api.IServices;
using jr_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System.Reflection.Metadata.Ecma335;

namespace jr_api.Services
{
    public class RolService : IRolService
    {
        private readonly ApplicationDbContext _context;

        public RolService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<object>> GetRoles()
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
            return roles;


        }
        public async Task<IEnumerable<object>> GetPermisos()
        {
            var permisos = await _context.Permisos
                .Select(p => new
                {
                    p.PermisoId,
                    p.DescripcionPermiso
                })
                .ToListAsync();

            return permisos;
        }
        public async Task<int> GuardarRol(GuardarRolRequest request)
        {
            

            // 1️⃣ Verificar si el rol ya existe
            var rol = await _context.Roles.FirstOrDefaultAsync(r => r.RolId == request.RolId);

            if (rol == null && request.RolId > 0)
            {
                return 0;
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

            return rol.RolId;
        }



    }
}
