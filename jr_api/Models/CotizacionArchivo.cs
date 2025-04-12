public class CotizacionArchivo
{
    public int CotizacionArchivoId { get; set; }
    public int CotizacionId { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string NombreArchivo { get; set; } = string.Empty;
    public string RutaArchivo { get; set; } = string.Empty;
    public DateTime FechaSubida { get; set; } = DateTime.Now;

    public Cotizaciones Cotizacion { get; set; }
}