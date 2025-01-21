using System;
namespace jr_api.Models
{
        public class Usuario
        {
            public int UsuarioId { get; set; }
            public string NombreUsuario { get; set; }
            public string Email { get; set; }
            public string ContraseñaHash { get; set; }
            public string ContraseñaSalt { get; set; }
            public byte[] Avatar { get; set; }
            public ICollection<UsuarioRol> UsuarioRoles { get; set; }
        }
}

