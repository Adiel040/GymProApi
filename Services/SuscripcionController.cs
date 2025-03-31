using Dapper;
using GymProApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using static GymProApi.DTOs.CreatesDTOs;
using static GymProApi.DTOs.EditsDTOs;

namespace GymProApi.Services
{
    [ApiController]
    [Route("[controller]")]
    public class SuscripcionController : ControllerBase
    {
        private readonly IConfiguration config;
        public SuscripcionController(IConfiguration config) { this.config = config; }

        [HttpGet("/GetSuscripciones")]
        public async Task<ActionResult<List<Suscripciones>>> GetSuscripciones()
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var suscripciones = await connection.QueryAsync<Suscripciones>("SELECT * FROM Suscripciones");
            return suscripciones.ToList();
        }


        [HttpGet("/GetSuscripcionById/{id}")]
        public async Task<ActionResult<Suscripciones?>> GetSuscripcionById(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var suscripcion = await connection.QueryFirstOrDefaultAsync<Suscripciones>("SELECT * FROM Suscripciones WHERE SuscripcionId = @SuscripcionId", new { SuscripcionId = id });
            return suscripcion == null ? NotFound() : suscripcion;
        }


        [HttpPost("/AddSuscripcion")]
        public async Task<ActionResult<Suscripciones>> AddSuscripcion(AddSuscripcionDto suscripcion)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.QueryFirstAsync<int>(
                @"INSERT INTO Suscripciones (Nombre, Descripcion, Precio, ClientesSuscritos) 
                OUTPUT INSERTED.SuscripcionId
                VALUES (@Nombre, @Descripcion, @Precio, 0);",
                new { suscripcion.Nombre, suscripcion.Descripcion, suscripcion.Precio }
            );

            return Ok(new Suscripciones
            {
                SuscripcionId = result,
                Nombre = suscripcion.Nombre,
                Descripcion = suscripcion.Descripcion,
                Precio = suscripcion.Precio,
                ClientesSuscritos = 0
            });
        }


        [HttpPut("/UpdateSuscripcion")]
        public async Task<IActionResult> UpdateSuscripcion(UpdateSuscripcionDto suscripcion)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.ExecuteAsync(
                "UPDATE Suscripciones SET Nombre = @Nombre, Precio = @Precio, Descripcion = @Descripcion WHERE SuscripcionId = @SuscripcionId",
                new { suscripcion.Nombre, suscripcion.Precio,suscripcion.Descripcion, suscripcion.SuscripcionId }
            );
            return result == 0 ? NotFound() : NoContent();
        }


        [HttpDelete("/DeleteSuscripcion/{id}")]
        public async Task<IActionResult> DeleteSuscripcion(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.ExecuteAsync("DELETE FROM Suscripciones WHERE SuscripcionId = @SuscripcionId", new { SuscripcionId = id });
            await connection.ExecuteAsync("UPDATE Clientes SET SuscripcionId = NULL, EntrenadorId = NULL WHERE SuscripcionId = @SuscripcionId", new { SuscripcionId = id });
            return result == 0 ? NotFound() : NoContent();
        }
    }
}

