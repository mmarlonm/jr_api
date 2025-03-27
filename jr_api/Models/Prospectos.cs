using jr_api.Models;

public class Prospecto
{
    public int ProspectoId { get; set; }

    public string Empresa { get; set; }
    public string Contacto { get; set; }
    public string Telefono { get; set; }
    public string Puesto { get; set; }
    public string GiroEmpresa { get; set; }
    public string Email { get; set; }
    public string AreaInteres { get; set; }
    public string TipoEmpresa { get; set; }

    public int UsuarioId { get; set; }           // Usuario que creó el prospecto
    public DateTime FechaRegistro { get; set; }

    // 🔗 Propiedades de navegación
    public Usuario? Usuario { get; set; }         // FK hacia Usuario
    public ICollection<SeguimientoProspecto>? Seguimientos { get; set; }
}