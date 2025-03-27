
using jr_api.IServices;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace jr_api.Services
{
    public class AnaliticaService : IAnaliticaService
    {
        private readonly ApplicationDbContext _context;

        public AnaliticaService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado usp_GetResumenAnaliticoPorEstatus y mapea los resultados a DTOs.
        /// </summary>
        public async Task<ResumenAnaliticoDto> ObtenerResumenAnaliticoPorEstatusAsync()
        {
            var proyectos = new List<AnaliticaResumenProyecto>();
            var cotizaciones = new List<AnaliticaResumenCotizacion>();

            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "usp_GetResumenAnaliticoPorEstatus";
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // Primer resultset: Proyectos por estatus
                        while (await reader.ReadAsync())
                        {
                            proyectos.Add(new AnaliticaResumenProyecto
                            {
                                EstatusId = reader.GetInt32(0),
                                EstatusNombre = reader.GetString(1),
                                TotalProyectos = reader.GetInt32(2)
                            });
                        }

                        // Siguiente resultset: Cotizaciones por estatus
                        if (await reader.NextResultAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                cotizaciones.Add(new AnaliticaResumenCotizacion
                                {
                                    EstatusId = reader.GetInt32(0),
                                    EstatusNombre = reader.GetString(1),
                                    TotalCotizaciones = reader.GetInt32(2)
                                });
                            }
                        }
                    }
                }
            }

            return new ResumenAnaliticoDto
            {
                Proyectos = proyectos,
                Cotizaciones = cotizaciones
            };
        }
    }
}