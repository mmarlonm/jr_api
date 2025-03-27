using System;
using System.ComponentModel.DataAnnotations;

public class Categoria
{
    [Key]
    public int CategoriaId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Nombre { get; set; }

    [MaxLength(500)]
    public string Descripcion { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}