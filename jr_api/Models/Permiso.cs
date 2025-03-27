public class Permiso
{
    public int PermisoId { get; set; }
    public string DescripcionPermiso { get; set; }
    public string Codigo { get; set; }

    // Relaciones
    public ICollection<RolPermiso> RolPermisos { get; set; }
}