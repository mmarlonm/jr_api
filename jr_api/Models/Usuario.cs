using System;
using NPOI.SS.Formula.Functions;

namespace jr_api.Models
{
        public class Usuario
        {
            public int UsuarioId { get; set; }
            public string NombreUsuario { get; set; }
            public string Email { get; set; }
            public string? Telefono { get; set; }
            public bool Activo { get; set; }
            public string ContraseñaHash { get; set; }
            public string ContraseñaSalt { get; set; }
            public byte[]? Avatar { get; set; }
            public ICollection<UsuarioRol> UsuarioRoles { get; set; }
            // 🔗 Navegación a Prospectos
            public ICollection<Prospecto?> Prospectos { get; set; }
            public ICollection<Proyecto?> Proyectos { get; set; }

    }
}

