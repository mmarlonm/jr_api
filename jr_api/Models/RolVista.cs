using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jr_api.Models
{
    public class RolVista
    {
        [Key]
        public int RolVistaId { get; set; }

        [Required]
        public int RolId { get; set; }

        [Required]
        public int VistaId { get; set; }

        public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;

        // Relaciones
        [ForeignKey("RolId")]
        public Rol Rol { get; set; }

        [ForeignKey("VistaId")]
        public Vista Vista { get; set; }
    }
}