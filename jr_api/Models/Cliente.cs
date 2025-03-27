using System;
using System.ComponentModel.DataAnnotations;

public class Cliente
{
    [Key]
    public int ClienteId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Nombre { get; set; }

    public string? Codigo { get; set; }

    [MaxLength(255)]
    public string?  Direccion { get; set; }

    [MaxLength(200)]
    public string?  Ciudad { get; set; }

    [MaxLength(200)]
    public string? Colonia { get; set; }

    [MaxLength(100)]
    public string? Estado { get; set; }

    [MaxLength(100)]
    public string? Pais { get; set; }

    [MaxLength(20)]
    public string? CodigoPostal { get; set; }

    [MaxLength(50)]
    public string? Telefono { get; set; }
    public string? Telefono2 { get; set; }

    [MaxLength(255)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(255)]
    public string? Empresa { get; set; }

    [MaxLength(50)]
    public string? RFC { get; set; } // Registro Fiscal

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public bool Activo { get; set; } = true;
}