using jr_api.Models;

public class Rol
{
    public int RolId { get; set; }
    public string NombreRol { get; set; }

    // Relaciones
    public ICollection<UsuarioRol> UsuarioRoles { get; set; }
    public ICollection<RolPermiso> RolPermisos { get; set; }
    public ICollection<RolVista> RolVistas { get; set; }
}