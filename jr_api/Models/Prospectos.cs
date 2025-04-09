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

    // 🔹 Nueva columna: Cómo se obtuvo el contacto
    public string? ComoSeObtuvo { get; set; }  // Web, redes sociales, recomendación, etc.

    // 🔹 Nueva columna: Otros (campo libre)
    public string? Otros { get; set; }

    public int? PersonalSeguimiento { get; set; }  // Web, redes sociales, recomendación, etc.

    // 🔗 Propiedades de navegación
    public Usuario? Usuario { get; set; }         // FK hacia Usuario
    public bool Active { get; set; }

    public ICollection<SeguimientoProspecto>? Seguimientos { get; set; }
}