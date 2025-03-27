public class ProyectoEstatusHistorial
{
    public int Id { get; set; }
    public int ProyectoId { get; set; }
    public int EstatusAnteriorId { get; set; }
    public int EstatusNuevoId { get; set; }
    public DateTime FechaCambio { get; set; }
    public string Comentarios { get; set; }
    public string UsuarioCambio { get; set; } // opcional

    public Proyecto Proyecto { get; set; }
    // Relaciones
    public EstatusProyecto EstatusAnterior { get; set; }
    public EstatusProyecto EstatusNuevo { get; set; }
}