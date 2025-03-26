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
        public async Task<ActionResult<List<Suscripciones>>> GetSuscripciones()
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var suscripciones = await connection.QueryAsync<Suscripciones>("SELECT * FROM Suscripciones");
            return suscripciones.ToList();
        }

        [HttpGet("{id}", Name = "GetSuscripcion")]
        public async Task<ActionResult<Suscripciones?>> GetSuscripcionById(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var suscripcion = await connection.QueryFirstOrDefaultAsync<Suscripciones>("SELECT * FROM Suscripciones WHERE SuscripcionId = @SuscripcionId", new { SuscripcionId = id });
            if (suscripcion == null) return NotFound();
            return suscripcion;
        }

        [HttpPost(Name = "AddSuscripcion")]
        public async Task<ActionResult<Suscripciones>> AddSuscripcion(Suscripciones suscripcion)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.QueryFirstAsync<int>(
                @"INSERT INTO Suscripciones (Nombre, Precio, ClientesSuscritos) 
                OUTPUT INSERTED.SuscripcionId
                VALUES (@Nombre, @Precio, @ClientesSuscritos);",
                new { suscripcion.Nombre, suscripcion.Precio, suscripcion.ClientesSuscritos }
            );

            return CreatedAtRoute("GetSuscripcion", new { id = result }, new Suscripciones
            {
                SuscripcionId = result,
                Nombre = suscripcion.Nombre,
                Precio = suscripcion.Precio,
                ClientesSuscritos = suscripcion.ClientesSuscritos
            });
        }

        [HttpPut("{id}", Name = "UpdateSuscripcion")]
        public async Task<IActionResult> UpdateSuscripcion(int id, Suscripciones suscripcion)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.ExecuteAsync(
                "UPDATE Suscripciones SET Nombre = @Nombre, Precio = @Precio, ClientesSuscritos = @ClientesSuscritos WHERE SuscripcionId = @SuscripcionId",
                new { suscripcion.Nombre, suscripcion.Precio, suscripcion.ClientesSuscritos, SuscripcionId = id }
            );
            if (result == 0) return NotFound();
            return NoContent();
        }

     
    }
}

