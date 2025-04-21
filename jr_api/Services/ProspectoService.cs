using jr_api.DTOs;
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
                .Where(P => P.Active)
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
        public async Task<object> SaveProspecto( Prospecto request)
        {
            ResponseDTO res = new ResponseDTO();
            try
            {
                if (request.ProspectoId == 0)
                {
                    request.FechaRegistro = DateTime.UtcNow;
                    request.Active = true;
                    _context.Prospectos.Add(request);
                }
                else
                {
                    var existing = await _context.Prospectos.FindAsync(request.ProspectoId);
                    if (existing == null)
                    {
                        res.Code = 500;
                        res.Message = "Prospecto no encontrado";
                        return res;
                    }

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
                res.Code = 200;
                res.Message = "Prospecto guardado exitosamente";
                res.data = request.ProspectoId;
                return res;
            }catch(Exception e)
            {
                res.Code = 500;
                res.Message = "Error al guardar prospecto "+ e.Message;
                return res;
            }
        }
        public async Task<Object> DeleteProspecto(int id)
        {
            var prospecto = await _context.Prospectos.FindAsync(id);
            if (prospecto == null)
                return null;
            prospecto.Active = false;
            _context.Prospectos.Update(prospecto);

            await _context.SaveChangesAsync();

            return prospecto;
        }
        public async Task<Object> GetSeguimientos(int prospectoId)
        {
            var seguimientos = await _context.SeguimientoProspectos
                .Where(s => s.ProspectoId == prospectoId)
                .ToListAsync();

            return seguimientos;
        }
        public async Task<Object> GetNotasByProspecto(int prospectoId)
        {
            var notas = await _context.NotasProspecto
                .Where(n => n.ProspectoId == prospectoId)
                .OrderByDescending(n => n.FechaCreacion)
                .Select(n => new NotaProspectoDto
                {
                    IdNote = n.NotaId,
                    ProspectoId = n.ProspectoId,
                    UsuarioId = n.UsuarioId,
                    NombreUsuario = n.Usuario.NombreUsuario, // Asumiendo que tienes una relación con Usuario
                    Title = n.Titulo,
                    Content = n.Contenido,
                    FechaCreacion = n.FechaCreacion
                })
                .ToListAsync();

            return notas;
        }
        public async Task<Object> SaveNota([FromBody] NotaProspectoDto notaDto)
        {
                       if (notaDto.IdNote == 0) // Nueva nota
            {
                var nuevaNota = new NotaProspecto
                {
                    ProspectoId = notaDto.ProspectoId,
                    UsuarioId = notaDto.UsuarioId,
                    Titulo = notaDto.Title,
                    Contenido = notaDto.Content,
                    FechaCreacion = DateTime.UtcNow
                };

                _context.NotasProspecto.Add(nuevaNota);
                await _context.SaveChangesAsync();
                return nuevaNota;
            }
            else // Edición de nota existente
            {
                var notaExistente = await _context.NotasProspecto.FindAsync(notaDto.IdNote);
                if (notaExistente == null)
                    return null;

                notaExistente.Titulo = notaDto.Title;
                notaExistente.Contenido = notaDto.Content;

                _context.NotasProspecto.Update(notaExistente);
                await _context.SaveChangesAsync();
                return notaExistente;
            }
        }
        public async Task<Object> DeleteNota(int notaId)
        {
            var nota = await _context.NotasProspecto.FindAsync(notaId);
            if (nota == null)
            {
                return null;
            }

            _context.NotasProspecto.Remove(nota);
            await _context.SaveChangesAsync();

            return nota;
        }






    }
}

