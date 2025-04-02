using jr_api.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class NotaProspecto
{
    [Key]
    public int NotaId { get; set; }

    [Required]
    public int ProspectoId { get; set; }

    public int? UsuarioId { get; set; } // Nullable para permitir ON DELETE SET NULL

    [Required]
    [MaxLength(255)]
    public string Titulo { get; set; }

    [Required]
    public string Contenido { get; set; }

    [Required]
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    // Relaciones
    [ForeignKey("ProspectoId")]
    public virtual Prospecto Prospecto { get; set; }

    [ForeignKey("UsuarioId")]
    public virtual Usuario Usuario { get; set; }
}