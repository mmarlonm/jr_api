﻿using jr_api.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ProyectoController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;
    private readonly IProyectoService _ProyectoService;


    public ProyectoController(ApplicationDbContext context, IWebHostEnvironment environment, IConfiguration configuration, IProyectoService proyecto)
    {
        _context = context;
        _environment = environment;
        _configuration = configuration;
        _ProyectoService = proyecto;
    }

    // 📌 1️⃣ Obtener todas las Unidades de Negocio
    [HttpGet("unidades-negocio")]
    public async Task<IActionResult> GetUnidadesDeNegocio()
    {
        var unidades = await _ProyectoService.GetUnidadesDeNegocio();
        return Ok(unidades);
    }

    // 📌 2️⃣ Obtener todas las Categorías
    [HttpGet("categorias")]
    public async Task<IActionResult> GetCategorias()
    {
        var categorias = await _ProyectoService.GetCategorias();
        return Ok(categorias);
    }

    // 📌 3️⃣ Obtener todos los Proyectos con sus relaciones (Categoría y Unidad de Negocio)
    [HttpGet("proyectos")]
    public async Task<IActionResult> GetProyectos()
    {
   
   
        var result = await _ProyectoService.GetProyectos();

        return Ok(result);
    }

    [HttpGet("proyecto/{id}")]
    public async Task<IActionResult> GetProyectoById(int id)
    {
        // Buscar el proyecto por su ID
        var proyecto = await _ProyectoService.GetProyectoById(id);
        // Verificar si el proyecto existe
        if (proyecto == null)
        {
            return NotFound("El proyecto no se encontró.");
        }

        return Ok(proyecto);
    }

    // 📌 4️⃣ Insertar o Actualizar un Proyecto
    [HttpPost("guardar-proyecto")]
    public async Task<IActionResult> SaveProyecto([FromBody] ProyectoDTO request)
    {
        if (request == null)
            return BadRequest("Datos inválidos.");

            // ACTUALIZACIÓN 
        var proyecto = await _ProyectoService.SaveProyecto(request);
        if (proyecto == null)
                return NotFound("Proyecto no encontrado.");
        return Ok(new { Message = "Proyecto guardado correctamente", ProyectoId = proyecto});
    }


    [HttpDelete("eliminar-proyecto/{id}")]
    public async Task<IActionResult> DeleteProyecto(int id)
    {
        var proyecto = await _ProyectoService.DeleteProyecto(id);
        if (proyecto == null)
        {
            return NotFound("El proyecto no se encontró.");
        }
        return Ok(new { Message = "Proyecto eliminado correctamente." });
    }


    [HttpGet("estatus")]
    public async Task<ActionResult<IEnumerable<EstatusProyecto>>> GetEstatusProyectos()
    {
        var estatus = await _context.EstatusProyecto.ToListAsync();
        return Ok(estatus);
    }

    // POST: api/Proyecto/SubirArchivo
    [HttpPost("SubirArchivo")]
    public async Task<IActionResult> SubirArchivo([FromForm] int proyectoId, [FromForm] string categoria, [FromForm] IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return BadRequest("Archivo no proporcionado.");

        var proyecto = await _ProyectoService.SubirArchivo( proyectoId, categoria, archivo);
        if (proyecto == null)
            return NotFound("Proyecto no encontrado.");
        return Ok(proyecto);

      
    }

    [HttpGet("ObtenerArchivos/{proyectoId}")]
    public async Task<IActionResult> ObtenerArchivos(int proyectoId)
    {
        var archivos = await _context.ProyectoArchivo
            .Where(a => a.ProyectoId == proyectoId)
            .OrderByDescending(a => a.FechaSubida)
            .ToListAsync();

        // ✅ Siempre regresamos 200 con lista vacía si no hay archivos
        return Ok(archivos);
    }

    [HttpGet("DescargarArchivo/{proyectoId}/{categoria}/{nombreArchivo}")]
    public IActionResult DescargarArchivo(int proyectoId, string categoria, string nombreArchivo)
    {
        try
        {
            // Leer la ruta base desde appsettings.json
            var rutaBase = _configuration["RutaArchivos"]; // "/Users/marlonjgs/Documentos/archivos_jringenieria"

            // Construir la ruta completa del archivo a descargar
            var rutaRelativa = Path.Combine(proyectoId.ToString(), categoria, nombreArchivo).Replace("\\", "/");
            var filePath = Path.Combine(rutaBase, rutaRelativa); // Ruta completa para acceder al archivo

            // Verificar si el archivo existe
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Archivo no encontrado.");
            }

            // Leer el archivo como bytes
            var archivoBytes = System.IO.File.ReadAllBytes(filePath);

            // Definir el tipo de contenido (ajustar según el tipo de archivo)
            var contentType = "application/octet-stream"; // Este puede ser ajustado según el tipo de archivo (PDF, DOCX, etc.)

            // Realizar la descarga
            return File(archivoBytes, contentType, nombreArchivo);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al descargar archivo: {ex.Message}");
        }
    }

    [HttpDelete("EliminarArchivo/{proyectoId}/{categoria}/{nombreArchivo}")]
    public async Task<IActionResult> EliminarArchivo(int proyectoId, string categoria, string nombreArchivo)
    {
        try
        {
            // Leer la ruta base desde appsettings.json
            var rutaBase = _configuration["RutaArchivos"];

            // Construir ruta relativa y completa
            var rutaRelativa = Path.Combine(proyectoId.ToString(), categoria, nombreArchivo).Replace("\\", "/");
            var filePath = Path.Combine(rutaBase, rutaRelativa);

            // Eliminar archivo físico si existe
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Buscar el archivo en la base de datos
            var archivoBD = await _context.ProyectoArchivo
                .FirstOrDefaultAsync(a =>
                    a.ProyectoId == proyectoId &&
                    a.Categoria == categoria &&
                    a.NombreArchivo == nombreArchivo);

            // Eliminar registro de la tabla si existe
            if (archivoBD != null)
            {
                _context.ProyectoArchivo.Remove(archivoBD);
                await _context.SaveChangesAsync();
            }

            return Ok("Archivo eliminado correctamente.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al eliminar archivo: {ex.Message}");
        }
    }

    [HttpGet("historial-estatus/{proyectoId}")]
    public async Task<IActionResult> GetHistorialEstatus(int proyectoId)
    {
        var historial = await _context.ProyectoEstatusHistorial
            .Where(h => h.ProyectoId == proyectoId)
            .OrderByDescending(h => h.FechaCambio)
            .Select(h => new ProyectoEstatusHistorialDto
            {
                EstatusAnterior = h.EstatusAnterior.Nombre,
                EstatusNuevo = h.EstatusNuevo.Nombre,
                FechaCambio = h.FechaCambio,
                Comentarios = h.Comentarios
            })
            .ToListAsync();

        return Ok(historial);
    }

    public class ProyectoDTO
    {
        public int ProyectoId { get; set; }
        public string Nombre { get; set; }
        public int categoria { get; set; }
        public string Lugar { get; set; }
        public int unidadDeNegocio { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Estado { get; set; }

        public int? Cliente { get; set; } 
        public string? Necesidad { get; set; }
        public string? Direccion { get; set; }
        public string? NombreContacto { get; set; }
        public string? Telefono { get; set; }
        public string? Empresa { get; set; }

        public string? Levantamiento { get; set; }
        public string? PlanoArquitectonico { get; set; }
        public string? DiagramaIsometrico { get; set; }
        public string? DiagramaUnifilar { get; set; }

        public string? MaterialesCatalogo { get; set; }
        public string? MaterialesPresupuestados { get; set; }
        public string? InventarioFinal { get; set; }
        public string? CuadroComparativo { get; set; }

        public string? Proveedor { get; set; }

        public string? ManoDeObra { get; set; }
        public string? PersonasParticipantes { get; set; }
        public string? Equipos { get; set; }
        public string? Herramientas { get; set; }

        public decimal? IndirectosCostos { get; set; }
        public decimal? Fianzas { get; set; }
        public decimal? Anticipo { get; set; }
        public decimal? Cotizacion { get; set; }

        public string? OrdenDeCompra { get; set; }
        public string? Contrato { get; set; }

        public string? ProgramaDeTrabajo { get; set; }
        public string? AvancesReportes { get; set; }
        public string? Comentarios { get; set; }
        public string? Hallazgos { get; set; }
        public string? Dosier { get; set; }
        public string? RutaCritica { get; set; }

        public string? Factura { get; set; }
        public decimal? Pago { get; set; }
        public decimal? UtilidadProgramada { get; set; }
        public decimal? UtilidadReal { get; set; }
        public decimal? Financiamiento { get; set; }

        public string? CierreProyectoActaEntrega { get; set; }
        public int Estatus { get; set; }

        public int? LiderProyectoId { get; set; }
        public string? Entregables { get; set; }
        public string? Cronograma { get; set; }
    }

    public class ProyectoArchivoDTO
    {
        public int ProyectoId { get; set; }
        public string Categoria { get; set; }
        public string NombreArchivo { get; set; }
        public string RutaArchivo { get; set; }
        public DateTime FechaSubida { get; set; }
    }
}