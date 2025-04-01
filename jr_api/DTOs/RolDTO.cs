using jr_api.Models;

public class RolDTO
{
    public int RolId { get; set; }
    public string NombreRol { get; set; }
    public List<RolVista> Vista { get; set; }
    public List<RolPermiso> Permisos { get; set; }

}