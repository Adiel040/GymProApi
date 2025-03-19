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
        public async Task<ActionResult<List<Entrenadores>>> GetEntrenadores()
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var entrenadores = await connection.QueryAsync<Entrenadores>("SELECT * FROM Entrenadores");
            return entrenadores.ToList();
        }

        [HttpGet("{id}", Name = "GetEntrenador")]
        public async Task<ActionResult<Entrenadores?>> GetEntrenadorById(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var entrenador = await connection.QueryFirstOrDefaultAsync<Entrenadores>("SELECT * FROM Entrenadores WHERE EntrenadorId = @EntrenadorId", new { EntrenadorId = id });
            if (entrenador == null) return NotFound();
            return entrenador;
        }

        [HttpPost(Name = "AddEntrenador")]
        public async Task<ActionResult<Entrenadores>> AddEntrenador(Entrenadores entrenador)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.QueryFirstAsync<int>(
                @"INSERT INTO Entrenadores (UserId, Rango, ClientesInscritos) 
                OUTPUT INSERTED.EntrenadorId
                VALUES (@UserId, @Rango, @ClientesInscritos);",
                new { entrenador.UserId, entrenador.Rango, entrenador.ClientesInscritos }
            );

            return CreatedAtRoute("GetEntrenador", new { id = result }, new Entrenadores
            {
                EntrenadorId = result,
                UserId = entrenador.UserId,
                Rango = entrenador.Rango,
                ClientesInscritos = entrenador.ClientesInscritos
            });
        }

        [HttpPut("{id}", Name = "UpdateEntrenador")]
        public async Task<IActionResult> UpdateEntrenador(int id, Entrenadores entrenador)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.ExecuteAsync(
                "UPDATE Entrenadores SET UserId = @UserId, Rango = @Rango, ClientesInscritos = @ClientesInscritos WHERE EntrenadorId = @EntrenadorId",
                new { entrenador.UserId, entrenador.Rango, entrenador.ClientesInscritos, EntrenadorId = id }
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



