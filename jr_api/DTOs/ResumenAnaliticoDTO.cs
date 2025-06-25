public class ResumenAnaliticoDto
{
    public List<AnaliticaResumenProyecto> Proyectos { get; set; }
    public List<AnaliticaResumenCotizacion> Cotizaciones { get; set; }
    public int TotalProspectos { get; set; }
    public int TotalClientes { get; set; }
    public int TotalProductos { get; set; }
}

public class AnaliticaResumenProyecto
{
    public int EstatusId { get; set; }
    public string EstatusNombre { get; set; }
    public int TotalProyectos { get; set; }
}

public class AnaliticaResumenCotizacion
{
    public int EstatusId { get; set; }
    public string EstatusNombre { get; set; }
    public int TotalCotizaciones { get; set; }
}

public class UbicacionDto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public double? Latitud { get; set; }
    public double? Longitud { get; set; }
    public string Tipo { get; set; } // "Prospecto" o "Cliente"
}