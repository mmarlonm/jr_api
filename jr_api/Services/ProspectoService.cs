using Azure.Core;
using jr_api.DTOs;
using jr_api.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
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
        public async Task<object> GetAllProspectos()
        {
            ResponseDTO res = new ResponseDTO();
            var prospectos = await _context.Prospectos
                .Where(P => P.Active)
                .ToListAsync();
            try
            {
                if (prospectos == null)
                {
                    res.Code = 500;
                    res.Message = "Prospecto no encontrado";
                    return res;
                }
                res.Code = 200;
                res.Message = "";
                res.data = prospectos;
                return res;
            }
            catch (Exception ex)
            {
                res.Code = 500;
                res.Message = "Error al encontrar prospecto " + ex;
                res.data = "";
                return res;

            }
            
        }

        public async Task<object> GetProspectoById(int id)
        {
            ResponseDTO res = new ResponseDTO();
            try
            {
                var prospecto = await _context.Prospectos
                    .Include(p => p.Telefonos)
                    .Include(p => p.Emails)
                    .FirstOrDefaultAsync(p => p.ProspectoId == id);

                if (prospecto == null)
                {
                    res.Code = 500;
                    res.Message = "Prospecto no encontrado";
                    return res;
                }

                // Armado de la respuesta con las listas anidadas
                res.Code = 200;
                res.Message = "";
                res.data = new
                {
                    prospecto.ProspectoId,
                    prospecto.Empresa,
                    prospecto.Contacto,
                    prospecto.Telefono,
                    prospecto.Puesto,
                    prospecto.GiroEmpresa,
                    prospecto.Email,
                    prospecto.AreaInteres,
                    prospecto.TipoEmpresa,
                    prospecto.UsuarioId,
                    prospecto.ComoSeObtuvo,
                    prospecto.Otros,
                    prospecto.PersonalSeguimiento,
                    prospecto.Active,
                    prospecto.Latitud,
                    prospecto.Longitud,
                    prospecto.RelacionComercial,
                    prospecto.Descripcion,
                    prospecto.Seguimiento,
                    prospecto.Llamada,
                    prospecto.Observaciones,
                    prospecto.FechaAccion,
                    prospecto.CanalMedio,
                    emails = prospecto.Emails?.Select(e => new
                    {
                        id = e.Id,
                        email = e.Email,
                        descripcion = e.Descripcion
                    }).ToList(),
                    telefonos = prospecto.Telefonos?.Select(t => new
                    {
                        id = t.Id,
                        telefono = t.Telefono,
                        descripcion = t.Descripcion
                    }).ToList()
                };

                return res;
            }
            catch (Exception ex)
            {
                res.Code = 500;
                res.Message = "Error al encontrar prospecto: " + ex.Message;
                res.data = "";
                return res;
            }
        }

        public async Task<object> SaveProspecto(Prospecto request)
        {
            ResponseDTO res = new ResponseDTO();
            try
            {
                if (request.ProspectoId == 0)
                {
                    request.FechaRegistro = DateTime.UtcNow;
                    request.Active = true;

                    // Asegura que los teléfonos/emails tengan el ProspectoId en 0 (para evitar conflictos)
                    if (request.Telefonos != null)
                    {
                        foreach (var tel in request.Telefonos)
                            tel.ProspectoId = 0;
                    }

                    if (request.Emails != null)
                    {
                        foreach (var email in request.Emails)
                            email.ProspectoId = 0;
                    }

                    _context.Prospectos.Add(request);
                }
                else
                {
                    var existing = await _context.Prospectos
                        .Include(p => p.Telefonos)
                        .Include(p => p.Emails)
                        .FirstOrDefaultAsync(p => p.ProspectoId == request.ProspectoId);

                    if (existing == null)
                    {
                        res.Code = 500;
                        res.Message = "Prospecto no encontrado";
                        return res;
                    }

                    // Actualiza campos base
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
                    existing.Longitud = request.Longitud;
                    existing.Latitud = request.Latitud;
                    existing.RelacionComercial = request.RelacionComercial;
                    existing.Descripcion = request.Descripcion;
                    existing.Seguimiento = request.Seguimiento;
                    existing.Llamada = request.Llamada;
                    existing.Observaciones = request.Observaciones;
                    existing.FechaAccion = request.FechaAccion;
                    existing.CanalMedio = request.CanalMedio;

                    // 🔄 Reemplaza teléfonos
                    _context.ProspectoTelefonos.RemoveRange(existing.Telefonos);
                    if (request.Telefonos != null && request.Telefonos.Any())
                    {
                        existing.Telefonos = request.Telefonos.Select(t => new ProspectoTelefono
                        {
                            Telefono = t.Telefono,
                            Descripcion = t.Descripcion,
                            ProspectoId = existing.ProspectoId
                        }).ToList();
                    }

                    // 🔄 Reemplaza emails
                    _context.ProspectoEmails.RemoveRange(existing.Emails);
                    if (request.Emails != null && request.Emails.Any())
                    {
                        existing.Emails = request.Emails.Select(e => new ProspectoEmail
                        {
                            Email = e.Email,
                            Descripcion = e.Descripcion,
                            ProspectoId = existing.ProspectoId
                        }).ToList();
                    }

                    _context.Prospectos.Update(existing);
                }

                await _context.SaveChangesAsync();
                res.Code = 200;
                res.Message = "Prospecto guardado exitosamente";
                res.data = request.ProspectoId;
                return res;
            }
            catch (Exception e)
            {
                res.Code = 500;
                res.Message = "Error al guardar prospecto: " + e.Message;
                return res;
            }
        }


        public async Task<Object> DeleteProspecto(int id)
        {
            ResponseDTO res = new ResponseDTO();
            var prospecto = await _context.Prospectos.FindAsync(id);
            try
            {
                if (prospecto == null) 
                 {
                    res.Code = 500;
                    res.Message = "Prospecto no encontrado";
                    return res;
                }
                
                prospecto.Active = false;
                _context.Prospectos.Update(prospecto);

                await _context.SaveChangesAsync();

                res.Code = 200;
                res.Message = "";
                res.data = prospecto;
                return res;
                }
            catch(Exception ex)
            {
                res.Code = 500;
                res.Message = "Error al encontrar prospecto " + ex;
                res.data = "";
                return res;
            }
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

