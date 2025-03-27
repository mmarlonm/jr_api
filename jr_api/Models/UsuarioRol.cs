using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jr_api.Models
{
    public class UsuarioRol
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // 🔥 Deja que la DB maneje el ID
        public int UsuarioRolId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int RolId { get; set; }

        public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;

        // Relaciones
        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        [ForeignKey("RolId")]
        public Rol Rol { get; set; }
    }
}