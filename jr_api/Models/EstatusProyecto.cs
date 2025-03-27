using System.ComponentModel.DataAnnotations;

public class EstatusProyecto
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Nombre { get; set; }

    [MaxLength(255)]
    public string Descripcion { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public bool Activo { get; set; } = true;
}