using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TuProyecto.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClienteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 📌 1️⃣ Obtener todos los clientes
        [HttpGet("getAll-cliente")]
        public async Task<IActionResult> GetClientes()
        {
            var clientes = await _context.Clientes
                .Where(c => c.Activo) // Solo los activos
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            return Ok(clientes);
        }

        // 📌 2️⃣ Obtener un cliente por ID
        [HttpGet("get-cliente/{id}")]
        public async Task<IActionResult> GetClienteById(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound("Cliente no encontrado.");

            return Ok(cliente);
        }

        // 📌 3️⃣ Crear o Actualizar un cliente
        [HttpPost("save-cliente")]
        public async Task<IActionResult> SaveCliente([FromBody] ClienteDTO request)
        {
            if (request == null)
                return BadRequest("Datos inválidos.");

            Cliente cliente;

            if (request.ClienteId == 0)
            {
                // Crear nuevo
                cliente = new Cliente
                {
                    Nombre = request.Nombre,
                    Codigo = request.Codigo,
                    Direccion = request.Direccion,
                    Ciudad = request.Ciudad,
                    Colonia = request.Colonia,
                    Estado = request.Estado,
                    Pais = request.Pais,
                    CodigoPostal = request.CodigoPostal,
                    Telefono = request.Telefono,
                    Telefono2 = request.Telefono2,
                    Email = request.Email,
                    Empresa = request.Empresa,
                    RFC = request.RFC,
                    Activo = true,
                    FechaRegistro = DateTime.UtcNow,
                    Calificacion = request.Calificacion
                };

                _context.Clientes.Add(cliente);
            }
            else
            {
                // Actualizar existente
                cliente = await _context.Clientes.FindAsync(request.ClienteId);
                if (cliente == null)
                    return NotFound("Cliente no encontrado.");

                cliente.Nombre = request.Nombre;
                cliente.Codigo = request.Codigo;
                cliente.Direccion = request.Direccion;
                cliente.Ciudad = request.Ciudad;
                cliente.Colonia = request.Colonia;
                cliente.Estado = request.Estado;
                cliente.Pais = request.Pais;
                cliente.CodigoPostal = request.CodigoPostal;
                cliente.Telefono = request.Telefono;
                cliente.Telefono2 = request.Telefono2;
                cliente.Email = request.Email;
                cliente.Empresa = request.Empresa;
                cliente.RFC = request.RFC;
                cliente.Activo = request.Activo;
                cliente.Calificacion = request.Calificacion;
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Cliente guardado correctamente.", ClienteId = cliente.ClienteId });
        }

        // 📌 4️⃣ Eliminar cliente (eliminación lógica)
        [HttpDelete("delete-cliente/{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound("Cliente no encontrado.");

            cliente.Activo = false;
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Cliente eliminado correctamente (lógico)." });
        }

        [HttpPost("upload-excel")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Archivo no válido.");

            var clientes = new List<Cliente>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                IWorkbook workbook;
                if (Path.GetExtension(file.FileName).Equals(".xls", StringComparison.OrdinalIgnoreCase))
                {
                    workbook = new HSSFWorkbook(stream);
                }
                else if (Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    workbook = new XSSFWorkbook(stream);
                }
                else
                {
                    return BadRequest("Formato de archivo no soportado.");
                }

                if (workbook.NumberOfSheets == 0)
                    return BadRequest("El archivo no contiene hojas.");

                var sheet = workbook.GetSheetAt(0);

                for (int row = 5; row <= sheet.LastRowNum; row++)
                {
                    var currentRow = sheet.GetRow(row);
                    if (currentRow == null) continue;

                    var cliente = new Cliente
                    {
                        Nombre = currentRow.GetCell(1)?.ToString(),
                        Codigo = currentRow.GetCell(0)?.ToString(),
                        Telefono = currentRow.GetCell(2)?.ToString(),
                        Telefono2 = currentRow.GetCell(3)?.ToString(),
                        Email = currentRow.GetCell(4)?.ToString(),
                        Activo = true
                    };

                    clientes.Add(cliente);
                }

                await _context.Clientes.AddRangeAsync(clientes);
                await _context.SaveChangesAsync();
            }

            return Ok(new { Message = "Importación completada correctamente", Total = clientes.Count });
        }
    }

    public class ClienteDTO
    {
        public int ClienteId { get; set; }
        public string Nombre { get; set; }
        public string? Codigo { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? Colonia { get; set; }
        public string? Estado { get; set; }
        public string? Pais { get; set; }
        public string? CodigoPostal { get; set; }
        public string? Telefono { get; set; }
        public string? Telefono2 { get; set; }
        public string? Email { get; set; }
        public string? Empresa { get; set; }
        public string? RFC { get; set; }
        public bool Activo { get; set; }
        public int? Calificacion { get; set; }
    }
}