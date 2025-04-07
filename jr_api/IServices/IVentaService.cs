public interface IVentaService
{
    Task<List<VentaParametrosDto>> ObtenerVentasAsync();
    Task<VentaDto?> ObtenerVentaPorIdAsync(int id);
    Task<VentaDto> GuardarVentaAsync(VentaDto dto);
    Task<bool> EliminarVentaAsync(int id);
    Task<List<FormaDePagoDto>> ObtenerFormasDePagoAsync();
}