using Microsoft.AspNetCore.Mvc;
using jr_api.Services;
using jr_api.IServices;
using System.IO;

[ApiController]
[Route("api/[controller]")]
public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        // 📌 Obtener todos los productos
        [HttpGet("getAll-productos")]
        public async Task<IActionResult> GetAllProductos()
        {
            var productos = await _productoService.GetAllProductosAsync();
            return Ok(productos);
        }

        // 📌 Obtener un producto por ID
        [HttpGet("get-producto/{id}")]
        public async Task<IActionResult> GetProductoById(int id)
        {
            var producto = await _productoService.GetProductoByIdAsync(id);
            if (producto == null)
                return NotFound("Producto no encontrado.");

            return Ok(producto);
        }

        // 📌 Crear o Actualizar un producto
        [HttpPost("save-producto")]
        public async Task<IActionResult> SaveProducto([FromBody] ProductoDto request)
        {
            if (request == null)
                return BadRequest("Datos inválidos.");

            var producto = await _productoService.SaveProductoAsync(request);
            if (producto == null)
                return NotFound("Producto no encontrado.");

            return Ok(new { Message = "Producto guardado correctamente.", ProductoId = producto.ProductoId });
        }

        // 📌 Eliminar producto (eliminación lógica)
        [HttpDelete("delete-producto/{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var success = await _productoService.DeleteProductoAsync(id);
            if (!success)
                return NotFound("Producto no encontrado.");

            return Ok(new { Message = "Producto eliminado correctamente (lógicamente)." });
        }

        // 📌 Importar productos desde un archivo Excel
        [HttpPost("upload-excel")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Archivo no válido.");

        var importedCount = await _productoService.ImportExcelAsync(file);

        return Ok(new { Message = importedCount });
        }
    }