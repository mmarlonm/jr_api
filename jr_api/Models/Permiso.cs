using System;
namespace jr_api.Models
{
    public class Permiso
    {
        public int PermisoId { get; set; }
        public string DescripcionPermiso { get; set; }
        public ICollection<RolPermiso> RolPermisos { get; set; }
    }
}

