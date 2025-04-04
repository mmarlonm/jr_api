
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
            int totalProspectos = 0;
            int totalClientes = 0;
            int totalProductos = 0;

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

                        // Segundo resultset: Cotizaciones por estatus
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

                        // Tercer resultset: Totales de prospectos, clientes y productos
                        if (await reader.NextResultAsync() && await reader.ReadAsync())
                        {
                            totalProspectos = reader.GetInt32(0);
                            totalClientes = reader.GetInt32(1);
                            totalProductos = reader.GetInt32(2);
                        }
                    }
                }
            }

            return new ResumenAnaliticoDto
            {
                Proyectos = proyectos,
                Cotizaciones = cotizaciones,
                TotalProspectos = totalProspectos,
                TotalClientes = totalClientes,
                TotalProductos = totalProductos
            };
        }
    }
}