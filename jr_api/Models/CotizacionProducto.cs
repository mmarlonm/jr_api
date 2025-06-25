using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using jr_api.Models;

public class CotizacionProducto
{
    [Key]
    public int CotizacionProductosId { get; set; }

    [Required]
    public int ClienteId { get; set; }

    [Required]
    public int UnidadDeNegocioId { get; set; }

    [Required]
    public int UsuarioId { get; set; }

    [Required]
    public bool RequisitosEspeciales { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public DateTime? UpdatedDate { get; set; }

    // 🔽 Nuevas columnas
    [MaxLength(200)]
    public string? NombreCliente { get; set; }

    [MaxLength(200)]
    public string? NombreEmpresa { get; set; }

    [MaxLength(200)]
    public string? Correo { get; set; }

    [MaxLength(50)]
    public string? Telefono { get; set; }

    [MaxLength(50)]
    public string? RFC { get; set; }

    [MaxLength(300)]
    public string? DireccionCompleta { get; set; }

    [MaxLength(100)]
    public string? Estado { get; set; }

    // Relaciones de navegación (opcional)
    public Cliente Cliente { get; set; }
    public UnidadDeNegocio UnidadDeNegocio { get; set; }
    public Usuario Usuario { get; set; }
}