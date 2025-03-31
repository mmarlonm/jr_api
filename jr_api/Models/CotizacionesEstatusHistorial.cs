using System.ComponentModel.DataAnnotations.Schema;

public class CotizacionesEstatusHistorial
{
    public int Id { get; set; }
    public int CotizacionId { get; set; }
    public int EstatusAnteriorId { get; set; }
    public int EstatusNuevoId { get; set; }
    public DateTime FechaCambio { get; set; }
    public string Comentarios { get; set; }
    public string UsuarioId { get; set; } // opcional

    [ForeignKey("CotizacionId")]
    public Cotizaciones Cotizacion { get; set; }  // 🔹 Relación correctamente definida
    // Relaciones
    public EstatusCotizacion EstatusAnterior { get; set; }
    public EstatusCotizacion EstatusNuevo { get; set; }
}