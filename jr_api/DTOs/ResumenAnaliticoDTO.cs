public class ResumenAnaliticoDto
{
    public List<AnaliticaResumenProyecto> Proyectos { get; set; }
    public List<AnaliticaResumenCotizacion> Cotizaciones { get; set; }
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