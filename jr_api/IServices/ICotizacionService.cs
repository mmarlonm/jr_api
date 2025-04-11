
using Microsoft.AspNetCore.Mvc;

public interface ICotizacionService
    {
    Task<Object> GetCotizaciones();
    Task<Object> GetCotizacionById(int id);
    Task<Object> SaveCotizacion(CotizacionesDTO cotizacionDto);
    Task<Object> DeleteCotizacion(int id);
    Task<IEnumerable<EstatusCotizacion>> GetEstatusCotizaciones();
    Task<Object> GetHistorialEstatus(int id);

    }

