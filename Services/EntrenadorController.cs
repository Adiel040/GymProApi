using Dapper;
using GymProApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GymProApi.Services
{
    [ApiController]
    [Route("[controller]")]
    public class EntrenadorController : ControllerBase
    {
        private readonly IConfiguration config;
        public EntrenadorController(IConfiguration config) { this.config = config; }

        [HttpGet(Name = "GetEntrenadores")]
        public async Task<ActionResult<List<Entrenador>>> GetEntrenadores()
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var entrenadores = await connection.QueryAsync<Entrenador>("SELECT * FROM Entrenadores");
            return entrenadores.ToList();
        }

        [HttpGet("{id}", Name = "GetEntrenador")]
        public async Task<ActionResult<Entrenador?>> GetEntrenadorById(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var entrenador = await connection.QueryFirstOrDefaultAsync<Entrenador>("SELECT * FROM Entrenadores WHERE EntrenadorId = @EntrenadorId", new { EntrenadorId = id });
            if (entrenador == null) return NotFound();
            return entrenador;
        }

        [HttpPost(Name = "AddEntrenador")]
        public async Task<ActionResult<Entrenador>> AddEntrenador(Entrenador entrenador)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.QueryFirstAsync<int>(
                @"INSERT INTO Entrenadores (Nombre, Apellido, Especialidad) 
                OUTPUT INSERTED.EntrenadorId
                VALUES (@Nombre, @Apellido, @Especialidad);",
                new { entrenador.Nombre, entrenador.Apellido, entrenador.Especialidad }
            );

            return CreatedAtRoute("GetEntrenador", new { id = result }, new Entrenador
            {
                EntrenadorId = result,
                Nombre = entrenador.Nombre,
                Apellido = entrenador.Apellido,
                Especialidad = entrenador.Especialidad
            });
        }

        [HttpPut("{id}", Name = "UpdateEntrenador")]
        public async Task<IActionResult> UpdateEntrenador(int id, Entrenador entrenador)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.ExecuteAsync(
                "UPDATE Entrenadores SET Nombre = @Nombre, Apellido = @Apellido, Especialidad = @Especialidad WHERE EntrenadorId = @EntrenadorId",
                new { entrenador.Nombre, entrenador.Apellido, entrenador.Especialidad, EntrenadorId = id }
            );
            if (result == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteEntrenador")]
        public async Task<IActionResult> DeleteEntrenador(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.ExecuteAsync("DELETE FROM Entrenadores WHERE EntrenadorId = @EntrenadorId", new { EntrenadorId = id });
            if (result == 0) return NotFound();
            return NoContent();
        }
    }
}
