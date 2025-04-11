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
            var cotizaciones = await _context.Cotizaciones
        .Where(c=> c.Active)
        .GroupJoin(
            _context.EstatusCotizacion,
            c => c.Estatus,
            e => e.Id,
            (c, estatusGroup) => new { c, estatusGroup }
        )
        .SelectMany(
            x => x.estatusGroup.DefaultIfEmpty(), // LEFT JOIN con EstatusCotizacion
            (x, e) => new { x.c, Estatus = e != null ? e.Nombre : "Sin Estatus" }
        )
        .GroupJoin(
            _context.Clientes,  // Tabla de clientes
            c => c.c.Cliente,   // Clave foránea en Cotizaciones
            cl => cl.ClienteId, // Clave primaria en Clientes
            (c, clienteGroup) => new { c, clienteGroup }
        )
        .SelectMany(
            x => x.clienteGroup.DefaultIfEmpty(), // LEFT JOIN con Clientes
            (x, cl) => new
            {
                x.c.c.CotizacionId,
                x.c.c.Cliente,
                NombreCliente = cl != null ? cl.Nombre : "Sin Cliente", // Nombre del Cliente
                x.c.c.Prospecto,
                x.c.c.UsuarioCreadorId,
                x.c.c.Necesidad,
                x.c.c.Direccion,
                x.c.c.NombreContacto,
                x.c.c.Telefono,
                x.c.c.Empresa,
                x.c.c.Cotizacion,
                x.c.c.OrdenCompra,
                x.c.c.Contrato,
                x.c.c.Proveedor,
                x.c.c.Vendedor,
                x.c.c.FechaEntrega,
                x.c.c.RutaCritica,
                x.c.c.Factura,
                x.c.c.Pago,
                x.c.c.UtilidadProgramada,
                x.c.c.UtilidadReal,
                x.c.c.Financiamiento,
                x.c.c.FechaRegistro,
                x.c.Estatus
            }
        )
        .ToListAsync();

            return cotizaciones;
        }
        public async Task<Object> GetCotizacionById(int id)
        {
            var cotizacion = await _context.Cotizaciones.FindAsync(id);
            if (cotizacion == null)
                return null;

            return cotizacion;
        }
        public async Task<Object> SaveCotizacion(CotizacionesDTO cotizacionDto)
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
                    Comentarios = cotizacionDto.Comentarios
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
                    return null;

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
                    Comentarios = cotizacionDto.Comentarios
                };

                _context.Cotizaciones.Update(cotizacion);
            }

            await _context.SaveChangesAsync();
            return  cotizacion;
        }
        public async Task<Object> DeleteCotizacion(int id)
        {
            var cotizacion = await _context.Cotizaciones.FindAsync(id);
            if (cotizacion == null)
                return null;
            cotizacion.Active = false;
            _context.Cotizaciones.Update(cotizacion);
            await _context.SaveChangesAsync();
            return cotizacion;
        }
        public async Task<IEnumerable<EstatusCotizacion>> GetEstatusCotizaciones()
        {
            var estatus = await _context.EstatusCotizacion.ToListAsync();
            return estatus;
        }
        public async Task<Object> GetHistorialEstatus(int id)
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

            return historial;
        }






    }
}
