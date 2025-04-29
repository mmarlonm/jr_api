using jr_api.DTOs;
using jr_api.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace jr_api.Services
{
    public class CotizacionService : ICotizacionService
    {
        private readonly ApplicationDbContext _context;

        public CotizacionService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Object> GetCotizaciones()
        {
        ResponseDTO res = new ResponseDTO();
         try
        {

                var cotizaciones = await (
            from c in _context.Cotizaciones
            where c.Active

            join e in _context.EstatusCotizacion
                on c.Estatus equals e.Id into estatusJoin
            from e in estatusJoin.DefaultIfEmpty() // LEFT JOIN EstatusCotizacion

            join cl in _context.Clientes
                on c.Cliente equals cl.ClienteId into clienteJoin
            from cl in clienteJoin.DefaultIfEmpty() // LEFT JOIN Clientes

            select new
            {
                c.CotizacionId,
                c.Cliente,
                NombreCliente = cl != null ? cl.Nombre : "Sin Cliente",
                c.Prospecto,
                c.UsuarioCreadorId,
                c.Necesidad,
                c.Direccion,
                c.NombreContacto,
                c.Telefono,
                c.Empresa,
                c.Cotizacion,
                c.OrdenCompra,
                c.Contrato,
                c.Proveedor,
                c.Vendedor,
                c.FechaEntrega,
                c.RutaCritica,
                c.Factura,
                c.Pago,
                c.UtilidadProgramada,
                c.UtilidadReal,
                c.Financiamiento,
                c.FechaRegistro,
                Estatus = e != null ? e.Nombre : "Sin Estatus"
            }
        ).ToListAsync();
                res.Code = 200;
                res.Message = "";
                res.data = cotizaciones;


         }
            catch (Exception ex)
            {
                res.Code = 500;
                res.Message = "Error al obtener cotizacion:" + ex;
                res.data = "";
                return res;
            }
            return res;
        }
        public async Task<Object> GetCotizacionById(int id)
        {
            ResponseDTO res = new ResponseDTO();
            try
            {
                var cotizacion = await _context.Cotizaciones.FindAsync(id);
                if (cotizacion == null)
                {
                    res.Code = 500;
                    res.Message = "Cotizacion no existe";
                    res.data = "";
                    return res;
                }
                    

                res.Code = 200;
                res.Message = "";
                res.data = cotizacion;

            }
            catch(Exception ex)
            {
                res.Code = 500;
                res.Message = "Error al obtener cotizacion:" + ex;
                res.data = "";
                return res;
            }


           
            return res;
        }
        public async Task<Object> SaveCotizacion(CotizacionesDTO cotizacionDto)
        {
            ResponseDTO res = new ResponseDTO();
            try
            {
                Cotizaciones cotizacion;

                if (cotizacionDto.CotizacionId == 0)
                {
                    // Nueva cotización
                    cotizacion = new Cotizaciones
                    {
                        Cliente = cotizacionDto.Cliente,
                        Prospecto = cotizacionDto.Prospecto,
                        UsuarioCreadorId = cotizacionDto.UsuarioCreadorId,
                        Necesidad = cotizacionDto.Necesidad,
                        Direccion = cotizacionDto.Direccion,
                        NombreContacto = cotizacionDto.NombreContacto,
                        Telefono = cotizacionDto.Telefono,
                        Empresa = cotizacionDto.Empresa,
                        Cotizacion = cotizacionDto.Cotizacion,
                        OrdenCompra = cotizacionDto.OrdenCompra,
                        Contrato = cotizacionDto.Contrato,
                        Proveedor = cotizacionDto.Proveedor,
                        Vendedor = cotizacionDto.Vendedor,
                        FechaEntrega = cotizacionDto.FechaEntrega,
                        RutaCritica = cotizacionDto.RutaCritica,
                        Factura = cotizacionDto.Factura,
                        Pago = cotizacionDto.Pago,
                        UtilidadProgramada = cotizacionDto.UtilidadProgramada,
                        UtilidadReal = cotizacionDto.UtilidadReal,
                        Financiamiento = cotizacionDto.Financiamiento,
                        FechaRegistro = DateTime.UtcNow,
                        Estatus = cotizacionDto.Estatus,
                        FormaPago = cotizacionDto.FormaPago,
                        TiempoEntrega = cotizacionDto.TiempoEntrega,
                        MontoTotal = cotizacionDto.MontoTotal,
                        AjustesCostos = cotizacionDto.AjustesCostos,
                        Comentarios = cotizacionDto.Comentarios,
                        Active =true
                    };

                    _context.Cotizaciones.Add(cotizacion);
                }
                else
                {
                    // Actualización de cotización existente
                    cotizacion = await _context.Cotizaciones
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.CotizacionId == cotizacionDto.CotizacionId);

                    if (cotizacion == null)
                    {
                        res.Code = 500;
                        res.Message = "Cotizacion no existe";
                        res.data = "";
                        return res;
                    }
                    var estatusAnterior = cotizacion.Estatus;
                    var estatusNuevo = cotizacionDto.Estatus;

                    // ⚠️ SOLO SI CAMBIA EL ESTATUS: Registrar en historial
                    if (estatusAnterior != estatusNuevo)
                    {
                        var historial = new CotizacionesEstatusHistorial
                        {
                            CotizacionId = cotizacionDto.CotizacionId,
                            EstatusAnteriorId = estatusAnterior,
                            EstatusNuevoId = estatusNuevo,
                            Comentarios = cotizacionDto.Comentarios ?? "Cambio de estatus",
                            FechaCambio = DateTime.UtcNow,
                            UsuarioId = cotizacionDto.UsuarioCreadorId?.ToString() ?? "Sistema"
                        };

                        _context.CotizacionesEstatusHistorial.Add(historial);
                    }

                    // Actualizar la entidad
                    cotizacion = new Cotizaciones
                    {
                        CotizacionId = cotizacionDto.CotizacionId,
                        Cliente = cotizacionDto.Cliente,
                        Prospecto = cotizacionDto.Prospecto,
                        UsuarioCreadorId = cotizacionDto.UsuarioCreadorId,
                        Necesidad = cotizacionDto.Necesidad,
                        Direccion = cotizacionDto.Direccion,
                        NombreContacto = cotizacionDto.NombreContacto,
                        Telefono = cotizacionDto.Telefono,
                        Empresa = cotizacionDto.Empresa,
                        Cotizacion = cotizacionDto.Cotizacion,
                        OrdenCompra = cotizacionDto.OrdenCompra,
                        Contrato = cotizacionDto.Contrato,
                        Proveedor = cotizacionDto.Proveedor,
                        Vendedor = cotizacionDto.Vendedor,
                        FechaEntrega = cotizacionDto.FechaEntrega,
                        RutaCritica = cotizacionDto.RutaCritica,
                        Factura = cotizacionDto.Factura,
                        Pago = cotizacionDto.Pago,
                        UtilidadProgramada = cotizacionDto.UtilidadProgramada,
                        UtilidadReal = cotizacionDto.UtilidadReal,
                        Financiamiento = cotizacionDto.Financiamiento,
                        FechaRegistro = cotizacionDto.FechaRegistro,
                        Estatus = cotizacionDto.Estatus,
                        FormaPago = cotizacionDto.FormaPago,
                        TiempoEntrega = cotizacionDto.TiempoEntrega,
                        MontoTotal = cotizacionDto.MontoTotal,
                        AjustesCostos = cotizacionDto.AjustesCostos,
                        Comentarios = cotizacionDto.Comentarios,
                        Active = true
                    };

                    _context.Cotizaciones.Update(cotizacion);
                }

                await _context.SaveChangesAsync();

                res.Code = 200;
                res.Message = "";
                res.data = cotizacion;
                
            }
            catch (Exception ex)
            {
                res.Code = 500;
                res.Message = "Error al guardar cotizacion:" + ex;
                res.data = "";
                return res;
            }
            return res;
        }

        public async Task<Object> DeleteCotizacion(int id)
        {

            ResponseDTO res = new ResponseDTO();
            try
            {
                var cotizacion = await _context.Cotizaciones.FindAsync(id);
                if (cotizacion == null) { 
                res.Code = 500;
                res.Message = "Cotizacion no existe";
                res.data = "";
                return res;
            }
                cotizacion.Active = false;
                _context.Cotizaciones.Update(cotizacion);
                await _context.SaveChangesAsync();
               
                res.Code = 200;
                res.Message = "";
                res.data = cotizacion;
            }
            catch (Exception ex)
            {
                res.Code = 500;
                res.Message = "Error al borrar cotizacion:" + ex;
                res.data = "";
                return res;

            }
            return res;
        }
        public async Task<ResponseDTO> GetEstatusCotizaciones()
        {
            ResponseDTO res = new ResponseDTO();
            try
            {
                var estatus = await _context.EstatusCotizacion.ToListAsync();
                res.Code = 200;
                res.Message = "";
                res.data = estatus;

               
            }
            catch(Exception ex)
            {
                res.Code = 500;
                res.Message = "Error al obtener cotizacion:" + ex;
                res.data = "";
                return res;

            }
            return res;
        }

        public async Task<Object> GetHistorialEstatus(int id)
        {
            ResponseDTO res = new ResponseDTO();
            try
            {
                var historial = await _context.CotizacionesEstatusHistorial
                    .Where(h => h.CotizacionId == id)
                    .OrderByDescending(h => h.FechaCambio)
                    .Select(h => new CotizacionesEstatusHistorialDTO
                    {
                        EstatusAnterior = h.EstatusAnterior.Nombre,
                        EstatusNuevo = h.EstatusNuevo.Nombre,
                        FechaCambio = h.FechaCambio,
                        Comentarios = h.Comentarios
                    })
                 
                    .ToListAsync();
                res.Code = 200;
                res.Message = "";
                res.data = historial;
                
            }
            catch(Exception ex)
            {
                res.Code = 500;
                res.Message = "Error al obtener cotizacion:" + ex;
                res.data = "";
                return res;
            }
            return res;
            }






    }
}
