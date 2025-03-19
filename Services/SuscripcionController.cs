using Dapper;
using GymProApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GymProApi.Services
{
    [ApiController]
    [Route("[controller]")]
    public class SuscripcionController : ControllerBase
    {
        private readonly IConfiguration config;
        public SuscripcionController(IConfiguration config) { this.config = config; }

        [HttpGet(Name = "GetSuscripciones")]
        public async Task<ActionResult<List<Suscripcion>>> GetSuscripciones()
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var suscripciones = await connection.QueryAsync<Suscripcion>("SELECT * FROM Suscripciones");
            return suscripciones.ToList();
        }

        [HttpGet("{id}", Name = "GetSuscripcion")]
        public async Task<ActionResult<Suscripcion?>> GetSuscripcionById(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var suscripcion = await connection.QueryFirstOrDefaultAsync<Suscripcion>("SELECT * FROM Suscripciones WHERE SuscripcionId = @SuscripcionId", new { SuscripcionId = id });
            if (suscripcion == null) return NotFound();
            return suscripcion;
        }

        [HttpPost(Name = "AddSuscripcion")]
        public async Task<ActionResult<Suscripcion>> AddSuscripcion(Suscripcion suscripcion)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.QueryFirstAsync<int>(
                @"INSERT INTO Suscripciones (ClienteId, FechaInicio, FechaFin) 
                OUTPUT INSERTED.SuscripcionId
                VALUES (@ClienteId, @FechaInicio, @FechaFin);",
                new { suscripcion.ClienteId, suscripcion.FechaInicio, suscripcion.FechaFin }
            );

            return CreatedAtRoute("GetSuscripcion", new { id = result }, new Suscripcion
            {
                SuscripcionId = result,
                ClienteId = suscripcion.ClienteId,
                FechaInicio = suscripcion.FechaInicio,
                FechaFin = suscripcion.FechaFin
            });
        }

        [HttpPut("{id}", Name = "UpdateSuscripcion")]
        public async Task<IActionResult> UpdateSuscripcion(int id, Suscripcion suscripcion)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.ExecuteAsync(
                "UPDATE Suscripciones SET ClienteId = @ClienteId, FechaInicio = @FechaInicio, FechaFin = @FechaFin WHERE SuscripcionId = @SuscripcionId",
                new { suscripcion.ClienteId, suscripcion.FechaInicio, suscripcion.FechaFin, SuscripcionId = id }
            );
            if (result == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteSuscripcion")]
        public async Task<IActionResult> DeleteSuscripcion(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.ExecuteAsync("DELETE FROM Suscripciones WHERE SuscripcionId = @SuscripcionId", new { SuscripcionId = id });
            if (result == 0) return NotFound();
            return NoContent();
        }
    }
}
