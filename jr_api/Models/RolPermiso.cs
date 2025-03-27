using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class RolPermiso
{
    [Key]
    public int RolPermisoId { get; set; }  // ✅ Clave primaria única

    [Required]
    public int RolId { get; set; }

    [Required]
    public int PermisoId { get; set; }

    public int? VistaId { get; set; }  // Puede ser NULL si no hay vista asociada

    // Relaciones con otras tablas
    [ForeignKey("RolId")]
    public Rol Rol { get; set; }

    [ForeignKey("PermisoId")]
    public Permiso Permiso { get; set; }

    [ForeignKey("VistaId")]
    public Vista? Vista { get; set; }
}