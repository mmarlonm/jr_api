public class ProyectoArchivo
{
    public int Id { get; set; }
    public int ProyectoId { get; set; }
    public string Categoria { get; set; }
    public string NombreArchivo { get; set; }
    public string RutaArchivo { get; set; }
    public DateTime FechaSubida { get; set; }

    public Proyecto Proyecto { get; set; }
}