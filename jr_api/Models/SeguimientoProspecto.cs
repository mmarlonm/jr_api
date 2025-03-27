using jr_api.Models;

public class SeguimientoProspecto
{
    public int SeguimientoProspectoId { get; set; } // ✅ CLAVE PRIMARIA

    public int ProspectoId { get; set; }
    public string Comentario { get; set; }
    public DateTime FechaRegistro { get; set; }

    public int UsuarioId { get; set; }

    // 🔗 Relaciones de navegación
    public Prospecto Prospecto { get; set; }
    public Usuario Usuario { get; set; }
}