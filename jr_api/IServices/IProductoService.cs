namespace jr_api.IServices
{
    public interface IProductoService
    {
        Task<List<Producto>> GetAllProductosAsync();
        Task<Producto> GetProductoByIdAsync(int id);
        Task<Producto> SaveProductoAsync(ProductoDto request);
        Task<bool> DeleteProductoAsync(int id);
        Task<string> ImportExcelAsync(IFormFile file);
    }
}