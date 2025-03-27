using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using jr_api.Models;

public class Proyecto
{
    [Key]
    public int? ProyectoId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Nombre { get; set; }

    [Required]
    public int CategoriaId { get; set; }

    [ForeignKey("CategoriaId")]
    public Categoria Categoria { get; set; }

    [Required]
    [MaxLength(255)]
    public string? Lugar { get; set; }

    [Required]
    public int UnidadId { get; set; }

    [ForeignKey("UnidadId")]
    public UnidadDeNegocio UnidadDeNegocio { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    [MaxLength(50)]
    public string Estado { get; set; } = "Pendiente";

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    // Nuevas propiedades
    [MaxLength(255)]
    public int? Cliente { get; set; }

    [MaxLength(255)]
    public string? Necesidad { get; set; }

    [MaxLength(255)]
    public string? Direccion { get; set; }

    [MaxLength(255)]
    public string? NombreContacto { get; set; }

    [MaxLength(50)]
    public string? Telefono { get; set; }

    [MaxLength(255)]
    public string? Empresa { get; set; }

    public string? Levantamiento { get; set; }
    public string? PlanoArquitectonico { get; set; }
    public string? DiagramaIsometrico { get; set; }
    public string? DiagramaUnifilar { get; set; }

    public string? MaterialesCatalogo { get; set; }
    public string? MaterialesPresupuestados { get; set; }
    public string? InventarioFinal { get; set; }
    public string? CuadroComparativo { get; set; }

    [MaxLength(255)]
    public string? Proveedor { get; set; }

    public string? ManoDeObra { get; set; }
    public string? PersonasParticipantes { get; set; }
    public string? Equipos { get; set; }
    public string? Herramientas { get; set; }

    public decimal? IndirectosCostos { get; set; }
    public decimal? Fianzas { get; set; }
    public decimal? Anticipo { get; set; }
    public decimal? Cotizacion { get; set; }

    [MaxLength(255)]
    public string? OrdenDeCompra { get; set; }

    [MaxLength(255)]
    public string? Contrato { get; set; }

    public string? ProgramaDeTrabajo { get; set; }
    public string? AvancesReportes { get; set; }
    public string? Comentarios { get; set; }
    public string? Hallazgos { get; set; }
    public string? Dosier { get; set; }
    public string? RutaCritica { get; set; }

    [MaxLength(255)]
    public string? Factura { get; set; }

    public decimal? Pago { get; set; }
    public decimal? UtilidadProgramada { get; set; }
    public decimal? UtilidadReal { get; set; }
    public decimal? Financiamiento { get; set; }

    public int? LiderProyectoId { get; set; }
    public string? Entregables { get; set; }
    public string? Cronograma { get; set; }

    public string? CierreProyectoActaEntrega { get; set; }

    // Clave foránea para EstatusProyecto
    public int Estatus { get; set; }

    // Propiedad de navegación para EstatusProyecto
    [ForeignKey("Estatus")]
    public EstatusProyecto EstatusProyecto { get; set; }

    // Clave foránea hacia Usuario
    public int UsuarioId { get; set; }

    // Relación inversa al Usuario
    public Usuario UsuarioCreador { get; set; }

    //relacion historial de estatus
    public ICollection<ProyectoEstatusHistorial> EstatusHistorial { get; set; }
}