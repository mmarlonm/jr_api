using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Cotizaciones
{
    [Key] // 🔹 Define CotizacionId como clave primaria
    public int CotizacionId { get; set; }

    public int? Cliente { get; set; }
    public int? Prospecto { get; set; }
    public int? UsuarioCreadorId { get; set; }

    [MaxLength(500)]
    public string? Necesidad { get; set; }

    [MaxLength(255)]
    public string? Direccion { get; set; }

    [MaxLength(255)]
    public string? NombreContacto { get; set; }

    [MaxLength(50)]
    public string? Telefono { get; set; }

    [MaxLength(255)]
    public string? Empresa { get; set; }

    public string? Cotizacion { get; set; }

    [MaxLength(255)]
    public string? OrdenCompra { get; set; }

    [MaxLength(255)]
    public string? Contrato { get; set; }

    [MaxLength(255)]
    public string? Proveedor { get; set; }

    [MaxLength(255)]
    public string? Vendedor { get; set; }

    public DateTime? FechaEntrega { get; set; }

    [MaxLength(500)]
    public string? RutaCritica { get; set; }

    [MaxLength(255)]
    public string? Factura { get; set; }

    public decimal? Pago { get; set; }
    public decimal? UtilidadProgramada { get; set; }
    public decimal? UtilidadReal { get; set; }
    public decimal? Financiamiento { get; set; }

    public DateTime? FechaRegistro { get; set; } = DateTime.UtcNow;

    
    public int Estatus { get; set; }

    [MaxLength(255)]
    public string? FormaPago { get; set; }  // Método de pago

    [MaxLength(255)]
    public string? TiempoEntrega { get; set; }  // Plazo de entrega

    public decimal? MontoTotal { get; set; }  // Monto total de la cotización

    [MaxLength(255)]
    public string? AjustesCostos { get; set; }  // Número(s) de ajustes de costos

    public string? Comentarios { get; set; }  // Comentarios o notas adicionales
    
    [ForeignKey("Estatus")]
    public EstatusCotizacion EstatusCotizacion { get; set; }
    public bool Active { get; set; }

        
    //relacion historial de estatus
    public ICollection<CotizacionesEstatusHistorial> CotizacionesEstatusHistorial { get; set; }
    public ICollection<CotizacionArchivo> Archivos { get; set; }
}