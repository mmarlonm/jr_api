
using jr_api.DTOs;
using Microsoft.AspNetCore.Mvc;

public interface ICotizacionService
    {
    Task<Object> GetCotizaciones();
    Task<Object> GetCotizacionById(int id);
    Task<Object> SaveCotizacion(CotizacionesDTO cotizacionDto);
    Task<Object> DeleteCotizacion(int id);
    Task<ResponseDTO> GetEstatusCotizaciones();
    Task<Object> GetHistorialEstatus(int id);

    }

