using System;
namespace jr_api.Models
{
    public class RolPermiso
    {
        public int RolId { get; set; }
        public Rol Rol { get; set; }
        public int PermisoId { get; set; }
        public Permiso Permiso { get; set; }
    }
}

