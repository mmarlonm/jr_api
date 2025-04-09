using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static ProyectoController;

namespace jr_api.IServices
{
    public interface IProyectoService
    {
        Task<IEnumerable<Object>> GetUnidadesDeNegocio();
        Task<IEnumerable<Object>> GetCategorias();
        Task<IEnumerable<Object>> GetProyectos();
        Task<Object> GetProyectoById(int id);
        Task<Object> SaveProyecto([FromBody] ProyectoDTO request);


    }
}
