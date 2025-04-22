using jr_api.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class Venta
{
    public int VentaId { get; set; }
    public DateTime Fecha { get; set; }
    public string? Serie { get; set; }
    public string? Folio { get; set; }
    public decimal? Total { get; set; }
    public decimal? Pendiente { get; set; }
    public Guid? UUID { get; set; }

    public int? FacturadorExternoId { get; set; }
    public string? Observaciones { get; set; }

    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; }

    public int AgenteId { get; set; }
    public Usuario Agente { get; set; }

    // Usar atributo Column si el nombre en la base de datos es diferente
    [Column("FormaPagoId")]
    public int FormaPagoId { get; set; }

    public FormaDePago FormaPago { get; set; }

    public int UnidadDeNegocioId { get; set; }
    public UnidadDeNegocio UnidadDeNegocio { get; set; }
    public DateTime FechaRegistro { get; set; }
    public bool Active { get; set; }
    public int? proyectoId { get; set; }

}