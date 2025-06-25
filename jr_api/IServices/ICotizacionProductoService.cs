using System.Collections.Generic;
using System.Threading.Tasks;
using jr_api.DTOs;

public interface ICotizacionProductoService
{
    Task<ResponseDTO> GetAllAsync();
    Task<ResponseDTO> GetByIdAsync(int id);
    Task<ResponseDTO> SaveAsync(CotizacionProductoDto dto);
    Task<bool> DeleteAsync(int id);
}