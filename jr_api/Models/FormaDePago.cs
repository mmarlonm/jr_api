public class FormaDePago
{
    public int Id { get; set; }
    public string Clave { get; set; }
    public string Descripcion { get; set; }

    public ICollection<Venta> Ventas { get; set; }
}