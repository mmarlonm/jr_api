using jr_api.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using static ProyectoController;

namespace jr_api.Services
{

    public class ProyectoService : IProyectoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ProyectoService(ApplicationDbContext context)
        {
            _context = context;
            
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
        public async Task<IEnumerable<Object>> GetProyectos()
        {
            var proyectos = await _context.Proyectos
                .Include(p => p.Categoria)
                .Include(p => p.UnidadDeNegocio)
                .Include(p => p.EstatusProyecto)
                .ToListAsync();

            if (proyectos == null || !proyectos.Any())
            {
                return new List<object>(); // Retorna una lista vacía en lugar de null
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

            return result;
        }
        public async Task<Object> GetProyectoById(int id)
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
                return null;
            }

            return proyecto;
        }
        public async Task<Object> SaveProyecto([FromBody] ProyectoDTO request)
        {
            
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

                proyecto.Estatus = estatusNuevo;
                _context.Proyectos.Update(proyecto);
            }

            await _context.SaveChangesAsync();
            

            return proyecto.ProyectoId ;
        }
        public async Task<Object> DeleteProyecto(int id)
        {
            var proyecto = await _context.Proyectos.FindAsync(id);
            if (proyecto == null)
            {
                return null;
            }

            // Eliminar las relaciones asociadas, si es necesario
            var rolVistas = await _context.RolVistas.Where(rv => rv.VistaId == id).ToListAsync();
            _context.RolVistas.RemoveRange(rolVistas);

            var rolPermisos = await _context.RolPermisos.Where(rp => rp.VistaId == id).ToListAsync();
            _context.RolPermisos.RemoveRange(rolPermisos);

            _context.Proyectos.Remove(proyecto);
            await _context.SaveChangesAsync();

            return proyecto;
        }
        public async Task<Object> SubirArchivo( int proyectoId, string categoria,  IFormFile archivo)
        {
            Response res = new Response();

            var proyecto = await _context.Proyectos.FindAsync(proyectoId);
            if (proyecto == null)
                return null;

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

    }
}

public class Response
{
    public int Code { get; set; }
    public string Message { get; set; }
    public string data { get; set; }
}