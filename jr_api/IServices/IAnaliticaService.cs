using System;
namespace jr_api.IServices
{
    /// <summary>
    /// Define los métodos disponibles para obtener datos analíticos del sistema.
    /// </summary>
    public interface IAnaliticaService
    {
        /// <summary>
        /// Obtiene el resumen analítico de proyectos y cotizaciones agrupados por estatus.
        /// </summary>
        /// <returns>Un DTO con las listas de resumen por estatus para proyectos y cotizaciones.</returns>
        Task<ResumenAnaliticoDto> ObtenerResumenAnaliticoPorEstatusAsync();
    }
}

