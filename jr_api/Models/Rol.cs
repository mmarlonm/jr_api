using System;
namespace jr_api.Models
{
    public class Rol
    {
        public int RolId { get; set; }
        public string NombreRol { get; set; }
        public ICollection<UsuarioRol> UsuarioRoles { get; set; }
        public ICollection<RolPermiso> RolPermisos { get; set; }
    }
}

