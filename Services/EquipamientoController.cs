using Dapper;
using GymProApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GymProApi.Services
{
    [ApiController]
    [Route("[controller]")]
    public class EquipamientoController : ControllerBase
    {
        private readonly IConfiguration config;
        public EquipamientoController(IConfiguration config) { this.config = config; }

        [HttpGet(Name = "GetEquipamientos")]
        public async Task<ActionResult<List<Equipamiento>>> GetEquipamientos()
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var equipamientos = await connection.QueryAsync<Equipamiento>("SELECT * FROM Equipamientos");
            return equipamientos.ToList();
        }

        [HttpGet("{id}", Name = "GetEquipamiento")]
        public async Task<ActionResult<Equipamiento?>> GetEquipamientoById(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var equipamiento = await connection.QueryFirstOrDefaultAsync<Equipamiento>("SELECT * FROM Equipamientos WHERE EquipamientoId = @EquipamientoId", new { EquipamientoId = id });
            if (equipamiento == null) return NotFound();
            return equipamiento;
        }

        [HttpPost(Name = "AddEquipamiento")]
        public async Task<ActionResult<Equipamiento>> AddEquipamiento(Equipamiento equipamiento)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.QueryFirstAsync<int>(
                @"INSERT INTO Equipamientos (Nombre, Descripcion, FechaAdquisicion) 
                OUTPUT INSERTED.EquipamientoId
                VALUES (@Nombre, @Descripcion, @FechaAdquisicion);",
                new { equipamiento.Nombre, equipamiento.Descripcion, equipamiento.FechaAdquisicion }
            );

            return CreatedAtRoute("GetEquipamiento", new { id = result }, new Equipamiento
            {
                EquipamientoId = result,
                Nombre = equipamiento.Nombre,
                Descripcion = equipamiento.Descripcion,
                FechaAdquisicion = equipamiento.FechaAdquisicion
            });
        }

        [HttpPut("{id}", Name = "UpdateEquipamiento")]
        public async Task<IActionResult> UpdateEquipamiento(int id, Equipamiento equipamiento)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.ExecuteAsync(
                "UPDATE Equipamientos SET Nombre = @Nombre, Descripcion = @Descripcion, FechaAdquisicion = @FechaAdquisicion WHERE EquipamientoId = @EquipamientoId",
                new { equipamiento.Nombre, equipamiento.Descripcion, equipamiento.FechaAdquisicion, EquipamientoId = id }
            );
            if (result == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteEquipamiento")]
        public async Task<IActionResult> DeleteEquipamiento(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.ExecuteAsync("DELETE FROM Equipamientos WHERE EquipamientoId = @EquipamientoId", new { EquipamientoId = id });
            if (result == 0) return NotFound();
            return NoContent();
        }
    }
}
