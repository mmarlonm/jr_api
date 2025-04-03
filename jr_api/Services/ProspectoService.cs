using jr_api.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
namespace jr_api.Services
{
    public class ProspectoService : IProspectoService
	{
        private readonly ApplicationDbContext _context;

        public ProspectoService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<object>> GetAllProspectos()
        {
            var prospectos = await _context.Prospectos
                .ToListAsync();

            return prospectos;
        }
        public async Task<object> GetProspectoById(int id)
        {
            var prospecto = await _context.Prospectos
                .FirstOrDefaultAsync(p => p.ProspectoId == id);

            if (prospecto == null)
                return null;
              
            return prospecto;
        }
        public async Task<int?> SaveProspecto( Prospecto request)
        {

            try
            {
                if (request.ProspectoId == 0)
                {
                    request.FechaRegistro = DateTime.UtcNow;
                    _context.Prospectos.Add(request);
                }
                else
                {
                    var existing = await _context.Prospectos.FindAsync(request.ProspectoId);
                    if (existing == null)
                        return null;

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
                    await _context.SaveChangesAsync();
                }
                return request.ProspectoId;
            }catch(Exception e)
            {
                return null;
            }
        }




    }
}

