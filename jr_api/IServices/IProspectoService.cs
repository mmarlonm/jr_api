using Microsoft.AspNetCore.Mvc;
using System;
namespace jr_api.IServices
{
	public interface IProspectoService
	{
        Task<IEnumerable<object>> GetAllProspectos();
        Task<object> GetProspectoById(int id);
        Task<Object> SaveProspecto(Prospecto request);
        Task<Object> DeleteProspecto(int id);
        Task<Object> GetSeguimientos(int prospectoId);
        Task<Object> GetNotasByProspecto(int prospectoId);
        Task<Object> SaveNota([FromBody] NotaProspectoDto notaDto);
        Task<Object> DeleteNota(int notaId);





    }
}

