public interface IVentaService
{
    Task<Object> ObtenerVentasAsync();
    Task<Object> ObtenerVentaPorIdAsync(int id);
    Task<Object> GuardarVentaAsync(VentaDto dto);
    Task<Object> EliminarVentaAsync(int id);
    Task<List<FormaDePagoDto>> ObtenerFormasDePagoAsync();
}