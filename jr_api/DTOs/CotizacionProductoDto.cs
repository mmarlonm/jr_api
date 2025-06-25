public class CotizacionProductoDto
{
    public int CotizacionProductosId { get; set; }

    public int ClienteId { get; set; }

    public int UnidadDeNegocioId { get; set; }

    public int UsuarioId { get; set; }

    public bool RequisitosEspeciales { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    // Nuevos campos de cliente
    public string? NombreCliente { get; set; }

    public string? NombreEmpresa { get; set; }

    public string? Correo { get; set; }

    public string? Telefono { get; set; }

    public string? RFC { get; set; }

    public string? DireccionCompleta { get; set; }

    public string? Estado { get; set; }
}