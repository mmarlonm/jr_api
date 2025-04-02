public class Producto
{
    public int ProductoId { get; set; }
    public string CodigoProducto { get; set; }
    public string NombreProducto { get; set; }
    public string? Descripcion { get; set; }
    public string? Proveedor { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public string UnidadMedida { get; set; }
    public string? Categoria { get; set; }
    public DateTime? FechaCreacion { get; set; } = DateTime.UtcNow;
    public bool? Activo { get; set; } = true;
}