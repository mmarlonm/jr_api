using System;
using System.IO;
using jr_api.IServices;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using static ProyectoController;

namespace jr_api.Services
{
    public class ProductoService : IProductoService
    {
        private readonly ApplicationDbContext _context;

        public ProductoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Producto>> GetAllProductosAsync()
        {
            return await _context.Productos
                .Where(p => (bool)p.Activo) // Filtramos por productos activos
                .OrderBy(p => p.NombreProducto)
                .ToListAsync();
        }

        public async Task<Producto> GetProductoByIdAsync(int id)
        {
            return await _context.Productos.FindAsync(id);
        }

        public async Task<Producto> SaveProductoAsync(ProductoDto request)
        {
            Producto producto;

            if (request.ProductoId == 0)
            {
                producto = new Producto
                {
                    CodigoProducto = request.CodigoProducto,
                    NombreProducto = request.NombreProducto,
                    Descripcion = request.Descripcion,
                    Proveedor = request.Proveedor,
                    Precio = (decimal)request.Precio,
                    UnidadMedida = request.UnidadMedida,
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                };

                _context.Productos.Add(producto);
            }
            else
            {
                producto = await _context.Productos.FindAsync(request.ProductoId);
                if (producto == null) return null;

                producto.CodigoProducto = request.CodigoProducto;
                producto.NombreProducto = request.NombreProducto;
                producto.Descripcion = request.Descripcion;
                producto.Proveedor = request.Proveedor;
                producto.Precio = (decimal)request.Precio;
                producto.UnidadMedida = request.UnidadMedida;
                producto.Activo = (bool)request.Activo;
            }

            await _context.SaveChangesAsync();
            return producto;
        }

        public async Task<bool> DeleteProductoAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return false;

            producto.Activo = false;
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<string> ImportExcelAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return "El archivo está vacío o no fue proporcionado.";
            }

            var productos = new List<Producto>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                IWorkbook workbook;
                string extension = Path.GetExtension(file.FileName).ToLower();

                if (extension == ".xls")
                {
                    workbook = new HSSFWorkbook(stream);
                }
                else if (extension == ".xlsx")
                {
                    workbook = new XSSFWorkbook(stream);
                }
                else
                {
                    return "Formato de archivo no soportado.";
                }

                if (workbook.NumberOfSheets == 0)
                {
                    return "El archivo no contiene hojas.";
                }

                var sheet = workbook.GetSheetAt(0); // Obtiene la primera hoja

                // Obtener los códigos de producto existentes en la BD
                var codigosExistentes = new HashSet<string>(
                    await _context.Productos.Select(p => p.CodigoProducto).ToListAsync()
                );

                for (int row = 2; row <= sheet.LastRowNum; row++) // Comienza desde la fila 3 (índice 2)
                {
                    var currentRow = sheet.GetRow(row);
                    if (currentRow == null) continue; // Si la fila está vacía, sigue con la siguiente

                    string codigoProducto = currentRow.GetCell(1)?.ToString(); // B

                    // Verificar si el código de producto ya existe
                    if (!string.IsNullOrWhiteSpace(codigoProducto) && codigosExistentes.Contains(codigoProducto))
                    {
                        continue; // Si ya existe, lo omitimos
                    }

                    var producto = new Producto
                    {
                        CodigoProducto = codigoProducto,
                        NombreProducto = currentRow.GetCell(2)?.ToString(), // C
                        Descripcion = null, // D
                        Proveedor = null, // E
                        Stock = 0,
                        UnidadMedida = "",
                        Precio = 0, // F
                        Activo = true
                    };

                    productos.Add(producto);
                }

                if (productos.Count > 0)
                {
                    await _context.Productos.AddRangeAsync(productos);
                    await _context.SaveChangesAsync();
                    return $"Productos importados: {productos.Count}";
                }

                return "No se encontraron productos nuevos en el archivo.";
            }
        }
    }
}

