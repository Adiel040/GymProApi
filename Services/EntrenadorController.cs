using Dapper;
using GymProApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using static GymProApi.DTOs.EditsDTOs;

namespace GymProApi.Services
{
    [ApiController]
    [Route("[controller]")]
    public class EntrenadorController : ControllerBase
    {
        private readonly IConfiguration config;
        public EntrenadorController(IConfiguration config) { this.config = config; }

        [HttpGet("/GetEntrenadores")]
        public async Task<ActionResult<List<Entrenadores>>> GetEntrenadores()
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var entrenadores = await connection.QueryAsync<Entrenadores>("SELECT E.*, U.username FROM Entrenadores E LEFT JOIN Usuarios U ON U.UserId = E.UserId;");
            return entrenadores.ToList();
        }

        [HttpGet("/GetEntrenadorById/{id}")]
        public async Task<ActionResult<Entrenadores?>> GetEntrenadorById(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var entrenador = await connection.QueryFirstOrDefaultAsync<Entrenadores>("SELECT E.*, U.username FROM Entrenadores E LEFT JOIN Usuarios U ON U.UserId = E.UserId WHERE EntrenadorId = @EntrenadorId", new { EntrenadorId = id });
            return entrenador is null ? NotFound() : entrenador;
        }

        [HttpPut("/UpdateEntrenador")]
        public async Task<IActionResult> UpdateEntrenador(UpdateEntrenadorDto entrenador)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.ExecuteAsync(
                "UPDATE Entrenadores SET Rango = @Rango WHERE EntrenadorId = @EntrenadorId",
                new {  entrenador.Rango, entrenador.EntrenadorId }
            );
            return result == 0 ? NotFound() : NoContent();
        }

    }
}



