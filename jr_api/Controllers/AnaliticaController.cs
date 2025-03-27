using jr_api.IServices;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AnaliticaController : ControllerBase
{
    private readonly IAnaliticaService _analiticaService;

    public AnaliticaController(IAnaliticaService analiticaService)
    {
        _analiticaService = analiticaService;
    }

    [HttpGet("resumen-estatus")]
    public async Task<IActionResult> GetResumenEstatus()
    {
        var resultado = await _analiticaService.ObtenerResumenAnaliticoPorEstatusAsync();
        return Ok(resultado);
    }
}