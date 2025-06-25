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

    [HttpGet("mapa")]
    public async Task<IActionResult> GetMapa(string tipo)
    {
        try
        {
            var resultado = await _analiticaService.ObtenerMapaAsync(tipo);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            // Puedes loguear o retornar el error explícitamente mientras desarrollas
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }
}