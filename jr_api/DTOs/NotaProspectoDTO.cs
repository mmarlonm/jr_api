public class NotaProspectoDto
{
    public int IdNote { get; set; }
    public int ProspectoId { get; set; }
    public string? NombreProspecto { get; set; }
    public int? UsuarioId { get; set; }
    public string? NombreUsuario { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime FechaCreacion { get; set; }
}