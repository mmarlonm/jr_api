public class VentaDto
{
    public int VentaId { get; set; }
    public DateTime Fecha { get; set; }
    public string? Serie { get; set; }
    public string? Folio { get; set; }
    public decimal? Total { get; set; }
    public decimal? Pendiente { get; set; }
    public Guid? UUID { get; set; }

    public int ClienteId { get; set; }
    public int UsuarioId { get; set; }
    public int FormaPagoId { get; set; }  // Cambié el nombre de 'FormasDePagoId' a 'FormaPagoId'
    public int UnidadNegocioId { get; set; }
}