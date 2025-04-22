using jr_api.DTOs;
using jr_api.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System.Linq.Expressions;
using static ProyectoController;

namespace jr_api.Services
{

    public class ProyectoService : IProyectoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ProyectoService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            
        }
        public async Task<IEnumerable<Object>> GetUnidadesDeNegocio()
        {
            var unidades = await _context.UnidadDeNegocio.ToListAsync();
            return unidades;
        }
        public async Task<IEnumerable<Object>> GetCategorias()
        {
            var categorias = await _context.Categorias.ToListAsync();
            return categorias;
        }
        public async Task<Object> GetProyectos()
        {
            ResponseDTO res = new ResponseDTO();
            var proyectos = await _context.Proyectos
                .Include(p => p.Categoria)
                .Include(p => p.UnidadDeNegocio)
                .Include(p => p.EstatusProyecto)
                .ToListAsync();

            if (proyectos == null || !proyectos.Any())
            {
                res.Code = 500;
                res.Message = "Proyecto no encontrado";
                res.data = "";
                return res;
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

            res.Code = 200;
            res.Message = "";
            res.data = result;
            return res;
        }

        public async Task<Object> GetProyectoById(int id)
        {

            ResponseDTO res = new ResponseDTO();
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
                res.Code = 500;
                res.Message = "Proyecto no encontrado";
                res.data = "";
                return res;
            }
            res.Code = 200;
            res.data = proyecto;

            return res;
        }
        public async Task<Object> SaveProyecto([FromBody] ProyectoDTO request)
        {
            Response res = new Response();

            Proyecto proyecto;
            try 
            {
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
                        Cronograma = request.Cronograma,
                        UsuarioId = 1
                    };

                    _context.Proyectos.Add(proyecto);
                }
                else
                {
                    // ACTUALIZACIÓN
                    proyecto = await _context.Proyectos.FindAsync(request.ProyectoId);
                    if (proyecto == null)
                        return null;
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
                    proyecto.Estatus = estatusNuevo;

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
                            UsuarioCambio = "Sistema"
                        };
                        _context.ProyectoEstatusHistorial.Add(historial);
                    }
                }

                
                _context.Proyectos.Update(proyecto);

                await _context.SaveChangesAsync();


                return res;

            }
            catch (Exception ex)
            {
                res.Code = 500;
                res.Message = "Error al guardar proyecto: " + ex;
                res.data = "";
                return res;
            }
        }
        public async Task<Object> DeleteProyecto(int id)
        {
            ResponseDTO res = new ResponseDTO();
            var proyecto = await _context.Proyectos.FindAsync(id);
            try
            {
                if (proyecto == null)
                {
                    res.Code = 500;
                    res.Message = "Error al encontrar el proyecto";
                    res.data = "";
                    return res;
                }

                // Eliminar las
                // relaciones asociadas, si es necesario
               

                var archivo = await _context.ProyectoArchivo.Where(rp => rp.ProyectoId == id).ToListAsync();
                _context.ProyectoArchivo.RemoveRange(archivo);

                var ventas = await _context.Ventas.Where(rp => rp.proyectoId == id).ToListAsync();
                _context.Ventas.RemoveRange(ventas);


                _context.Proyectos.Remove(proyecto);

                await _context.SaveChangesAsync();
                res.Code = 200;
                res.data = proyecto;
                return res;
               }
            catch (Exception ex)
            {
                res.Code = 500;
                res.Message = "Error al borrar proyecto: " + ex;
                res.data = "";
                return res;
            }
        }
        public async Task<Object> SubirArchivo( int proyectoId, string categoria,  IFormFile archivo)
        {
            Response res = new Response();
            if (archivo == null || archivo.Length == 0)
            {
                res.Code = 500;
                res.Message = "Archivo no proporcionado";
                res.data = "";
                return res;
            }
            var proyecto = await _context.Proyectos.FindAsync(proyectoId);
            if (proyecto == null)
            {
                res.Code = 500;
                res.Message = "proyecto no encontrado";
                res.data = "";
                return res;
            }
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
                res.Code = 200;
                res.Message = "OK";
                res.data = rutaRelativa;
                return  res;
            }
            catch (Exception ex)
            {
                res.Code = 500;
                res.Message = "Error al guardar archivo: " + ex;
                res.data = "";
                return res;
            }
        }
        public async Task<Object> ObtenerArchivos(int proyectoId)
        {
            var archivos = await _context.ProyectoArchivo
                .Where(a => a.ProyectoId == proyectoId)
                .OrderByDescending(a => a.FechaSubida)
                .ToListAsync();

            if (archivos == null || !archivos.Any())
                return null;

            return archivos;
        }
        public async Task<Response> DescargarArchivo(int proyectoId, string categoria, string nombreArchivo)
        {
            Response res = new Response();
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
                    res.Code = 500;
                    res.Message = "Archivo no encontrado" ;
                    res.data = "";
                    return res;
                }

                // Leer el archivo como bytes
                var archivoBytes = System.IO.File.ReadAllBytes(filePath);

                // Definir el tipo de contenido (ajustar según el tipo de archivo)
                var contentType = "application/octet-stream"; // Este puede ser ajustado según el tipo de archivo (PDF, DOCX, etc.)

                // Realizar la descarga
                res.Code = 200;
                res.Message = "Ok";
                res.content = archivoBytes;
                res.contentType = contentType;
                res.NombreArchivo = nombreArchivo;
                return res;
            }
            catch (Exception ex)
            {
                res.Code = 500;
                res.Message = "Error al descargar archivo: " + ex;
                res.data = "";
                return res;
            }
        }
        public async Task<Response> EliminarArchivo(int proyectoId, string categoria, string nombreArchivo)
        {
            Response res = new Response();
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
                else {
                    res.Code = 500;
                    res.Message = "no existe el archivo";
                    res.data = "";
                    return res;
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
                    res.Code = 200;
                    res.Message = "Archivo eliminado correctamente";
                    res.data = "";

                }
                else
                {
                    res.Code = 500;
                    res.Message = "No se encontro el registro en la base de datos";
                    res.data = "";
                }
                    return res;
            }
            catch (Exception ex)
            {
                res.Code = 500;
                res.Message = "Error al eliminar el archivo:" + ex;
                res.data = "";
                return res;
              }
        }




    }
}

public class Response
{
    public int Code { get; set; }
    public string Message { get; set; }
    public string data { get; set; }
    public byte[] content { get; set; }
    public string contentType { get; set; }
    public string NombreArchivo { get; set; }
}