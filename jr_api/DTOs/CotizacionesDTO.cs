using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class CotizacionesDTO
{
    public int CotizacionId { get; set; }

    public int? Cliente { get; set; }
    public int? Prospecto { get; set; }
    public int? UsuarioCreadorId { get; set; }
    public string? Necesidad { get; set; }
    public string? Direccion { get; set; }
    public string NombreContacto { get; set; }
    public string? Telefono { get; set; }
    public string? Empresa { get; set; }

    public string? Cotizacion { get; set; }
    public string? OrdenCompra { get; set; }

    public string? Contrato { get; set; }

    public string? Proveedor { get; set; }

    public string? Vendedor { get; set; }

    public DateTime? FechaEntrega { get; set; }

    public string? RutaCritica { get; set; }

    public string? Factura { get; set; }

    public decimal? Pago { get; set; }
    public decimal? UtilidadProgramada { get; set; }
    public decimal? UtilidadReal { get; set; }
    public decimal? Financiamiento { get; set; }

    public DateTime? FechaRegistro { get; set; } = DateTime.UtcNow;


    public int Estatus { get; set; }

    public string? FormaPago { get; set; }  // Método de pago

    public string? TiempoEntrega { get; set; }  // Plazo de entrega

    public decimal? MontoTotal { get; set; }  // Monto total de la cotización

    public string? AjustesCostos { get; set; }  // Número(s) de ajustes de costos

    public string? Comentarios { get; set; }  // Comentarios o notas adicionales
}