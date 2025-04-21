using Microsoft.EntityFrameworkCore;

public class VentaService : IVentaService
{
    private readonly ApplicationDbContext _context;

    public VentaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<VentaParametrosDto>> ObtenerVentasAsync()
    {

        return await _context.Ventas
             .Where(V => V.Active)

            .Include(v => v.Cliente)
            .Include(v => v.Agente)  // Cambio de Usuario a Agente
            .Include(v => v.UnidadDeNegocio)
            .Include(v => v.FormaPago)
            .Select(v => new VentaParametrosDto
            {
                VentaId = v.VentaId,
                Fecha = v.Fecha,
                Serie = v.Serie,
                Folio = v.Folio,
                ClienteId = v.ClienteId,
                ClienteNombre = v.Cliente.Nombre,  // Asegúrate de que "Cliente" tiene la propiedad "Nombre"
                Total = v.Total,
                Pendiente = v.Pendiente,
                UsuarioId = v.AgenteId,  // Cambio de UsuarioId a AgenteId
                UsuarioNombre = v.Agente.NombreUsuario,  // Cambio de Usuario a Agente
                FormaPagoId = v.FormaPagoId,
                FormaDePagoDescripcion = v.FormaPago.Descripcion,
                UUID = v.UUID,
                UnidadDeNegocioId = v.UnidadDeNegocioId,
                UnidadDeNegocioNombre = v.UnidadDeNegocio.Nombre
            })
            .ToListAsync();
    }

    public async Task<VentaDto?> ObtenerVentaPorIdAsync(int id)
    {
        return await _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Agente)  // Cambio de Usuario a Agente
            .Include(v => v.UnidadDeNegocio)
            .Include(v => v.FormaPago)
            .Where(v => v.VentaId == id)
            .Select(v => new VentaDto
            {
                VentaId = v.VentaId,
                Fecha = v.Fecha,
                Serie = v.Serie,
                Folio = v.Folio,
                ClienteId = v.ClienteId,
                Total = v.Total,
                Pendiente = v.Pendiente,
                UUID = v.UUID,
                UsuarioId = v.AgenteId,
                FormaPagoId = v.FormaPagoId,
                UnidadNegocioId = v.UnidadDeNegocioId,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<VentaDto> GuardarVentaAsync(VentaDto dto)
    {
        // Validar datos obligatorios
        if (dto.Fecha == default || string.IsNullOrEmpty(dto.Serie) || string.IsNullOrEmpty(dto.Folio))
        {
            throw new ArgumentException("Faltan valores obligatorios");
        }

        Venta venta;

        // Verificar si estamos actualizando una venta existente o creando una nueva
        if (dto.VentaId > 0)
        {
            // Actualizar
            venta = await _context.Ventas.FindAsync(dto.VentaId);

            if (venta == null)
            {
                throw new ArgumentException("Venta no encontrada para actualizar");
            }

            // Actualizar los campos de la venta
            venta.Fecha = dto.Fecha;
            venta.Serie = dto.Serie;
            venta.Folio = dto.Folio;
            venta.ClienteId = dto.ClienteId;
            venta.Total = dto.Total;
            venta.Pendiente = dto.Pendiente;
            venta.AgenteId = dto.UsuarioId; // Asegúrate de que UsuarioId está presente y es válido
            venta.FormaPagoId = dto.FormaPagoId;
            venta.UUID = dto.UUID;
            venta.UnidadDeNegocioId = dto.UnidadNegocioId;
        }
        else
        {
            // Crear nueva venta
            venta = new Venta
            {
                Fecha = dto.Fecha,
                Serie = dto.Serie,
                Folio = dto.Folio,
                ClienteId = dto.ClienteId,
                Total = dto.Total,
                Pendiente = dto.Pendiente,
                AgenteId = dto.UsuarioId, // Asegúrate de que UsuarioId está presente y es válido
                FormaPagoId = dto.FormaPagoId,
                UUID = dto.UUID,
                UnidadDeNegocioId = dto.UnidadNegocioId,
                FechaRegistro = DateTime.Now // Si falta este valor, lo puedes agregar aquí
            };

            _context.Ventas.Add(venta);
        }

        try
        {
            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log o manejo del error
            throw new Exception("Error al guardar o actualizar la venta", ex);
        }

        // Asignar el VentaId a DTO para retornar
        dto.VentaId = venta.VentaId;
        return dto;
    }

    public async Task<bool> EliminarVentaAsync(int id)
    {
        var venta = await _context.Ventas.FindAsync(id);
        if (venta == null) return false;
        venta.Active = false;
        _context.Ventas.Update(venta);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<FormaDePagoDto>> ObtenerFormasDePagoAsync()
    {
        return await _context.FormasDePago
            .Select(f => new FormaDePagoDto
            {
                Id = f.Id,
                Clave = f.Clave,
                Descripcion = f.Descripcion
            }).ToListAsync();
    }
}

public class VentaParametrosDto
{
    public int VentaId { get; set; }
    public DateTime Fecha { get; set; }
    public string? Serie { get; set; }
    public string? Folio { get; set; }
    public decimal? Total { get; set; }
    public decimal? Pendiente { get; set; }
    public Guid? UUID { get; set; }

    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } // Nombre del cliente
    public int UsuarioId { get; set; }
    public string UsuarioNombre { get; set; } // Nombre del usuario/agente
    public int FormaPagoId { get; set; }
    public string FormaDePagoDescripcion { get; set; } // Descripción de la forma de pago
    public int UnidadDeNegocioId { get; set; }
    public string UnidadDeNegocioNombre { get; set; } // Nombre de la unidad de negocio
}