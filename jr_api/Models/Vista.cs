using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using jr_api.Models;

public class Vista
{
    [Key]
    public int VistaId { get; set; }

    [Required]
    [MaxLength(255)]
    public string NombreVista { get; set; }

    [Required]
    [MaxLength(255)]
    public string Ruta { get; set; }

    // Relaciones
    public ICollection<RolVista> RolVistas { get; set; }
    public ICollection<RolPermiso> RolPermisos { get; set; }
}