using Microsoft.AspNetCore.Mvc;
using System;
namespace jr_api.IServices
{
	public interface IProspectoService
	{
        Task<IEnumerable<object>> GetAllProspectos();
        Task<object> GetProspectoById(int id);
        Task<int?> SaveProspecto(Prospecto request);


    }
}

