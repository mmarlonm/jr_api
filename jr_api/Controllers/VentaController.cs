using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class VentaController : ControllerBase
{
    private readonly IVentaService _ventaService;

    public VentaController(IVentaService ventaService)
    {
        _ventaService = ventaService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var ventas = await _ventaService.ObtenerVentasAsync();
        return Ok(ventas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var venta = await _ventaService.ObtenerVentaPorIdAsync(id);
        return venta != null ? Ok(venta) : NotFound();
    }

    [HttpPost("save-venta")]
    public async Task<IActionResult> Create([FromBody] VentaDto dto)
    {
        var venta = await _ventaService.GuardarVentaAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = venta }, venta);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _ventaService.EliminarVentaAsync(id);
        return Ok(result);
    }

    // Obtener formas de pago
    [HttpGet("formas-de-pago")]
    public async Task<IActionResult> GetFormasDePago()
    {
        var formas = await _ventaService.ObtenerFormasDePagoAsync();
        return Ok(formas);
    }
}