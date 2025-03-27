using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Cotizaciones
{
    [Key] // 🔹 Define CotizacionId como clave primaria
    public int CotizacionId { get; set; }

    public int? Cliente { get; set; }
    public int? Prospecto { get; set; }
    public int UsuarioCreadorId { get; set; }

    [MaxLength(500)]
    public string Necesidad { get; set; }

    [MaxLength(255)]
    public string Direccion { get; set; }

    [MaxLength(255)]
    public string NombreContacto { get; set; }

    [MaxLength(50)]
    public string Telefono { get; set; }

    [MaxLength(255)]
    public string Empresa { get; set; }

    public string Cotizacion { get; set; }

    [MaxLength(255)]
    public string OrdenCompra { get; set; }

    [MaxLength(255)]
    public string Contrato { get; set; }

    [MaxLength(255)]
    public string Proveedor { get; set; }

    [MaxLength(255)]
    public string Vendedor { get; set; }

    public DateTime? FechaEntrega { get; set; }

    [MaxLength(500)]
    public string RutaCritica { get; set; }

    [MaxLength(255)]
    public string Factura { get; set; }

    public decimal? Pago { get; set; }
    public decimal? UtilidadProgramada { get; set; }
    public decimal? UtilidadReal { get; set; }
    public decimal? Financiamiento { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    
    public int Estatus { get; set; }
}