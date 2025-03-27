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

    public ProyectoController(ApplicationDbContext context, IWebHostEnvironment environment, IConfiguration configuration)
    {
        _context = context;
        _environment = environment;
        _configuration = configuration;
    }

    // 📌 1️⃣ Obtener todas las Unidades de Negocio
    [HttpGet("unidades-negocio")]
    public async Task<IActionResult> GetUnidadesDeNegocio()
    {
        var unidades = await _context.UnidadDeNegocio.ToListAsync();
        return Ok(unidades);
    }

    // 📌 2️⃣ Obtener todas las Categorías
    [HttpGet("categorias")]
    public async Task<IActionResult> GetCategorias()
    {
        var categorias = await _context.Categorias.ToListAsync();
        return Ok(categorias);
    }

    // 📌 3️⃣ Obtener todos los Proyectos con sus relaciones (Categoría y Unidad de Negocio)
    [HttpGet("proyectos")]
    public async Task<IActionResult> GetProyectos()
    {
        var proyectos = await _context.Proyectos
            .Include(p => p.Categoria)
            .Include(p => p.UnidadDeNegocio)
            .Include(p => p.EstatusProyecto)
            .ToListAsync();

        if (proyectos == null || !proyectos.Any())
        {
            return Ok(new List<object>()); // Retorna una lista vacía en lugar de null
        }

        var result = proyectos.Select(p => new
        {
            p.ProyectoId,
            p.Nombre,
            Categoria = p.Categoria?.Nombre ?? "Sin categoría",
            p.Lugar,
            UnidadDeNegocio = p.UnidadDeNegocio?.Nombre ?? "Sin unidad de negocio",
            p.FechaInicio,
            p.FechaFin,
            p.Estado,
            Estatus = p.EstatusProyecto?.Nombre ?? "Sin estatus",
        }).ToList();

        return Ok(result);
    }

    [HttpGet("proyecto/{id}")]
    public async Task<IActionResult> GetProyectoById(int id)
    {
        // Buscar el proyecto por su ID
        var proyecto = await _context.Proyectos
            .Where(p => p.ProyectoId == id)
            .Select(p => new
            {
                p.ProyectoId,
                p.Nombre,
                CategoriaId = p.CategoriaId,  // Solo enviar el ID de la categoría
                UnidadDeNegocioId = p.UnidadId,  // Solo enviar el ID de la unidad de negocio
                p.Lugar,
                p.FechaInicio,
                p.FechaFin,
                p.Estado,

                // Nuevas propiedades
                p.Cliente,
                p.Necesidad,
                p.Direccion,
                p.NombreContacto,
                p.Telefono,
                p.Empresa,

                p.Levantamiento,
                p.PlanoArquitectonico,
                p.DiagramaIsometrico,
                p.DiagramaUnifilar,

                p.MaterialesCatalogo,
                p.MaterialesPresupuestados,
                p.InventarioFinal,
                p.CuadroComparativo,

                p.Proveedor,

                p.ManoDeObra,
                p.PersonasParticipantes,
                p.Equipos,
                p.Herramientas,

                p.IndirectosCostos,
                p.Fianzas,
                p.Anticipo,
                p.Cotizacion,

                p.OrdenDeCompra,
                p.Contrato,

                p.ProgramaDeTrabajo,
                p.AvancesReportes,
                p.Comentarios,
                p.Hallazgos,
                p.Dosier,
                p.RutaCritica,

                p.Factura,
                p.Pago,
                p.UtilidadProgramada,
                p.UtilidadReal,
                p.Financiamiento,

                p.CierreProyectoActaEntrega,
                p.Estatus,
                p.Entregables,
                p.Cronograma,
                p.LiderProyectoId
            })
            .FirstOrDefaultAsync();

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

        Proyecto proyecto;

        if (request.ProyectoId == 0)
        {
            // NUEVO PROYECTO
            proyecto = new Proyecto
            {
                Nombre = request.Nombre,
                CategoriaId = request.categoria,
                Lugar = request.Lugar,
                UnidadId = request.unidadDeNegocio,
                FechaInicio = request.FechaInicio,
                FechaFin = request.FechaFin,
                Estado = request.Estado,

                Cliente = request.Cliente,
                Necesidad = request.Necesidad,
                Direccion = request.Direccion,
                NombreContacto = request.NombreContacto,
                Telefono = request.Telefono,
                Empresa = request.Empresa,

                Levantamiento = request.Levantamiento,
                PlanoArquitectonico = request.PlanoArquitectonico,
                DiagramaIsometrico = request.DiagramaIsometrico,
                DiagramaUnifilar = request.DiagramaUnifilar,

                MaterialesCatalogo = request.MaterialesCatalogo,
                MaterialesPresupuestados = request.MaterialesPresupuestados,
                InventarioFinal = request.InventarioFinal,
                CuadroComparativo = request.CuadroComparativo,

                Proveedor = request.Proveedor,
                ManoDeObra = request.ManoDeObra,
                PersonasParticipantes = request.PersonasParticipantes,
                Equipos = request.Equipos,
                Herramientas = request.Herramientas,

                IndirectosCostos = request.IndirectosCostos,
                Fianzas = request.Fianzas,
                Anticipo = request.Anticipo,
                Cotizacion = request.Cotizacion,
                OrdenDeCompra = request.OrdenDeCompra,
                Contrato = request.Contrato,

                ProgramaDeTrabajo = request.ProgramaDeTrabajo,
                AvancesReportes = request.AvancesReportes,
                Comentarios = request.Comentarios,
                Hallazgos = request.Hallazgos,
                Dosier = request.Dosier,
                RutaCritica = request.RutaCritica,

                Factura = request.Factura,
                Pago = request.Pago,
                UtilidadProgramada = request.UtilidadProgramada,
                UtilidadReal = request.UtilidadReal,
                Financiamiento = request.Financiamiento,

                CierreProyectoActaEntrega = request.CierreProyectoActaEntrega,
                Estatus = request.Estatus,
                LiderProyectoId = request.LiderProyectoId,
                Entregables = request.Entregables,
                Cronograma = request.Cronograma
            };

            _context.Proyectos.Add(proyecto);
        }
        else
        {
            // ACTUALIZACIÓN
            proyecto = await _context.Proyectos.FindAsync(request.ProyectoId);
            if (proyecto == null)
                return NotFound("Proyecto no encontrado.");

            var estatusAnterior = proyecto.Estatus;
            var estatusNuevo = request.Estatus;

            // Actualizar campos
            proyecto.Nombre = request.Nombre;
            proyecto.CategoriaId = request.categoria;
            proyecto.Lugar = request.Lugar;
            proyecto.UnidadId = request.unidadDeNegocio;
            proyecto.FechaInicio = request.FechaInicio;
            proyecto.FechaFin = request.FechaFin;
            proyecto.Estado = request.Estado;

            proyecto.Cliente = request.Cliente;
            proyecto.Necesidad = request.Necesidad;
            proyecto.Direccion = request.Direccion;
            proyecto.NombreContacto = request.NombreContacto;
            proyecto.Telefono = request.Telefono;
            proyecto.Empresa = request.Empresa;

            proyecto.Levantamiento = request.Levantamiento;
            proyecto.PlanoArquitectonico = request.PlanoArquitectonico;
            proyecto.DiagramaIsometrico = request.DiagramaIsometrico;
            proyecto.DiagramaUnifilar = request.DiagramaUnifilar;

            proyecto.MaterialesCatalogo = request.MaterialesCatalogo;
            proyecto.MaterialesPresupuestados = request.MaterialesPresupuestados;
            proyecto.InventarioFinal = request.InventarioFinal;
            proyecto.CuadroComparativo = request.CuadroComparativo;

            proyecto.Proveedor = request.Proveedor;
            proyecto.ManoDeObra = request.ManoDeObra;
            proyecto.PersonasParticipantes = request.PersonasParticipantes;
            proyecto.Equipos = request.Equipos;
            proyecto.Herramientas = request.Herramientas;

            proyecto.IndirectosCostos = request.IndirectosCostos;
            proyecto.Fianzas = request.Fianzas;
            proyecto.Anticipo = request.Anticipo;
            proyecto.Cotizacion = request.Cotizacion;
            proyecto.OrdenDeCompra = request.OrdenDeCompra;
            proyecto.Contrato = request.Contrato;

            proyecto.ProgramaDeTrabajo = request.ProgramaDeTrabajo;
            proyecto.AvancesReportes = request.AvancesReportes;
            proyecto.Comentarios = request.Comentarios;
            proyecto.Hallazgos = request.Hallazgos;
            proyecto.Dosier = request.Dosier;
            proyecto.RutaCritica = request.RutaCritica;

            proyecto.Factura = request.Factura;
            proyecto.Pago = request.Pago;
            proyecto.UtilidadProgramada = request.UtilidadProgramada;
            proyecto.UtilidadReal = request.UtilidadReal;
            proyecto.Financiamiento = request.Financiamiento;

            proyecto.CierreProyectoActaEntrega = request.CierreProyectoActaEntrega;

            proyecto.LiderProyectoId = request.LiderProyectoId;
            proyecto.Entregables = request.Entregables;
            proyecto.Cronograma = request.Cronograma;

            // ⚠️ SOLO SI CAMBIA EL ESTATUS: Registrar en historial
            if (estatusAnterior != estatusNuevo)
            {
                var historial = new ProyectoEstatusHistorial
                {
                    ProyectoId = request.ProyectoId,
                    EstatusAnteriorId = estatusAnterior,
                    EstatusNuevoId = estatusNuevo,
                    Comentarios = request.Comentarios,
                    FechaCambio = DateTime.Now,
                    UsuarioCambio = User?.Identity?.Name ?? "Sistema"
                };

                _context.ProyectoEstatusHistorial.Add(historial);
            }

            proyecto.Estatus = estatusNuevo;
            _context.Proyectos.Update(proyecto);
        }

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Proyecto guardado correctamente", ProyectoId = proyecto.ProyectoId });
    }


    [HttpDelete("eliminar-proyecto/{id}")]
    public async Task<IActionResult> DeleteProyecto(int id)
    {
        var proyecto = await _context.Proyectos.FindAsync(id);
        if (proyecto == null)
        {
            return NotFound("El proyecto no se encontró.");
        }

        // Eliminar las relaciones asociadas, si es necesario
        var rolVistas = await _context.RolVistas.Where(rv => rv.VistaId == id).ToListAsync();
        _context.RolVistas.RemoveRange(rolVistas);

        var rolPermisos = await _context.RolPermisos.Where(rp => rp.VistaId == id).ToListAsync();
        _context.RolPermisos.RemoveRange(rolPermisos);

        _context.Proyectos.Remove(proyecto);
        await _context.SaveChangesAsync();

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

        var proyecto = await _context.Proyectos.FindAsync(proyectoId);
        if (proyecto == null)
            return NotFound("Proyecto no encontrado.");

        try
        {
            // ✅ Leer la ruta base desde appsettings.json
            var rutaBase = _configuration["RutaArchivos"]; // "/Users/marlonjgs/Documentos/archivos_jringenieria"

            // Construir ruta completa
            var folderPath = Path.Combine(rutaBase, proyectoId.ToString(), categoria);

            // Crear carpeta si no existe
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = Path.GetFileName(archivo.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            // Guardar archivo
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            // Ruta relativa para guardar en base de datos (opcionalmente podrías guardarla completa también)
            var rutaRelativa = Path.Combine(proyectoId.ToString(), categoria, fileName).Replace("\\", "/");

            var archivoProyecto = new ProyectoArchivo
            {
                ProyectoId = proyectoId,
                Categoria = categoria,
                NombreArchivo = fileName,
                RutaArchivo = rutaRelativa,
                FechaSubida = DateTime.Now
            };

            _context.ProyectoArchivo.Add(archivoProyecto);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Archivo subido exitosamente", ruta = rutaRelativa });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al subir archivo: {ex.Message}");
        }
    }

    [HttpGet("ObtenerArchivos/{proyectoId}")]
    public async Task<IActionResult> ObtenerArchivos(int proyectoId)
    {
        var archivos = await _context.ProyectoArchivo
            .Where(a => a.ProyectoId == proyectoId)
            .OrderByDescending(a => a.FechaSubida)
            .ToListAsync();

        if (archivos == null || !archivos.Any())
            return NotFound("No se encontraron archivos para este proyecto.");

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