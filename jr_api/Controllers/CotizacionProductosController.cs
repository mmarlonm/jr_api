using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using jr_api.IServices;

[ApiController]
[Route("api/[controller]")]
public class CotizacionProductosController : ControllerBase
{
    private readonly ICotizacionProductoService _cotizacionProductoService;

    public CotizacionProductosController(ICotizacionProductoService cotizacionProductoService)
    {
        _cotizacionProductoService = cotizacionProductoService;
    }

    // GET: api/CotizacionProductos
    [HttpGet]
    public async Task<ActionResult<List<CotizacionProducto>>> GetAll()
    {
        var result = await _cotizacionProductoService.GetAllAsync();
        return Ok(result);
    }

    // GET: api/CotizacionProductos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CotizacionProducto>> GetById(int id)
    {
        var result = await _cotizacionProductoService.GetByIdAsync(id);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    // POST: api/CotizacionProductos
    [HttpPost]
    public async Task<ActionResult<CotizacionProducto>> Save(CotizacionProductoDto dto)
    {
        var result = await _cotizacionProductoService.SaveAsync(dto);
        if (result == null)
            return NotFound("Cotización no encontrada para actualizar.");

        return Ok(result);
    }

    // DELETE: api/CotizacionProductos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _cotizacionProductoService.DeleteAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}