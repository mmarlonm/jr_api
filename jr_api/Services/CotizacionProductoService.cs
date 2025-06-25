using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using jr_api.DTOs;
using jr_api.IServices;
using Microsoft.EntityFrameworkCore;

namespace jr_api.Services
{
    public class CotizacionProductoService : ICotizacionProductoService
    {
        private readonly ApplicationDbContext _context;

        public CotizacionProductoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseDTO> GetAllAsync()
        {
            ResponseDTO res = new ResponseDTO();

            List<CotizacionProducto> cotizacion = await _context.CotizacionProductos
                .Include(c => c.Cliente)
                .Include(c => c.UnidadDeNegocio)
                .Include(c => c.Usuario)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();

            res.Code = 200;
            res.Message = "";
            res.data = cotizacion;
            return res;
        }

        public async Task<ResponseDTO> GetByIdAsync(int id)
        {
            ResponseDTO res = new ResponseDTO();

            CotizacionProducto cotizacion = await _context.CotizacionProductos
                .Include(c => c.Cliente)
                .Include(c => c.UnidadDeNegocio)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.CotizacionProductosId == id);

            res.Code = 200;
            res.Message = "";
            res.data = cotizacion;
            return res;
        }

        public async Task<ResponseDTO> SaveAsync(CotizacionProductoDto dto)
        {
            CotizacionProducto cotizacion;
            ResponseDTO res = new ResponseDTO();

            if (dto.CotizacionProductosId == 0)
            {
                // Crear nuevo
                cotizacion = new CotizacionProducto
                {
                    ClienteId = dto.ClienteId,
                    UnidadDeNegocioId = dto.UnidadDeNegocioId,
                    UsuarioId = dto.UsuarioId,
                    RequisitosEspeciales = dto.RequisitosEspeciales,
                    CreatedDate = DateTime.UtcNow,

                    // Datos del cliente (desnormalización)
                    NombreCliente = dto.NombreCliente,
                    NombreEmpresa = dto.NombreEmpresa,
                    Correo = dto.Correo,
                    Telefono = dto.Telefono,
                    RFC = dto.RFC,
                    DireccionCompleta = dto.DireccionCompleta,
                    Estado = dto.Estado
                };

                _context.CotizacionProductos.Add(cotizacion);
            }
            else
            {
                // Actualizar existente
                cotizacion = await _context.CotizacionProductos
                    .FirstOrDefaultAsync(c => c.CotizacionProductosId == dto.CotizacionProductosId);

                if (cotizacion == null) return null;

                cotizacion.ClienteId = dto.ClienteId;
                cotizacion.UnidadDeNegocioId = dto.UnidadDeNegocioId;
                cotizacion.UsuarioId = dto.UsuarioId;
                cotizacion.RequisitosEspeciales = dto.RequisitosEspeciales;
                cotizacion.UpdatedDate = DateTime.UtcNow;

                // Actualizar datos del cliente si cambió
                cotizacion.NombreCliente = dto.NombreCliente;
                cotizacion.NombreEmpresa = dto.NombreEmpresa;
                cotizacion.Correo = dto.Correo;
                cotizacion.Telefono = dto.Telefono;
                cotizacion.RFC = dto.RFC;
                cotizacion.DireccionCompleta = dto.DireccionCompleta;
                cotizacion.Estado = dto.Estado;
            }

            await _context.SaveChangesAsync();
            res.Code = 200;
            res.Message = "";
            res.data = cotizacion;
            return res;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cotizacion = await _context.CotizacionProductos.FindAsync(id);
            if (cotizacion == null) return false;

            _context.CotizacionProductos.Remove(cotizacion);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}