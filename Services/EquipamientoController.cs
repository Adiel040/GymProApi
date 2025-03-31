using Dapper;
using GymProApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using static GymProApi.DTOs.CreatesDTOs;

namespace GymProApi.Services
{
    [ApiController]
    [Route("[controller]")]
    public class EquipamientoController : ControllerBase
    {
        private readonly IConfiguration config;
        public EquipamientoController(IConfiguration config) { this.config = config; }

        [HttpGet("/GetEquipamientos")]
        public async Task<ActionResult<List<Equipamientos>>> GetEquipamientos()
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var equipamientos = await connection.QueryAsync<Equipamientos>("SELECT * FROM Equipamientos");
            return equipamientos.ToList();
        }


        [HttpGet("/GetEquipamientoById/{id}")]
        public async Task<ActionResult<Equipamientos?>> GetEquipamientoById(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var equipamiento = await connection.QueryFirstOrDefaultAsync<Equipamientos>("SELECT * FROM Equipamientos WHERE EquipoId = @EquipoId", new { EquipoId = id });
            return equipamiento is null ? NotFound() : equipamiento;
        }


        [HttpPost("/AddEquipamiento")]
        public async Task<ActionResult<Equipamientos>> AddEquipamiento(AddEquipamientoDto equipamiento)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.QueryFirstAsync<int>(
                @"INSERT INTO Equipamientos (Nombre, Descripcion) 
                OUTPUT INSERTED.EquipoId
                VALUES (@Nombre, @Descripcion);",
                new { equipamiento.Nombre, equipamiento.Descripcion }
            );

            return Ok(new Equipamientos
            {
                EquipoId = result,
                Nombre = equipamiento.Nombre,
                Descripcion = equipamiento.Descripcion
            });
        }


        [HttpPut("/UpdateEquipamiento")]
        public async Task<IActionResult> UpdateEquipamiento(Equipamientos equipamiento)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.ExecuteAsync(
                "UPDATE Equipamientos SET Nombre = @Nombre, Descripcion = @Descripcion WHERE EquipoId = @EquipoId",
                new { equipamiento.Nombre, equipamiento.Descripcion, equipamiento.EquipoId }
            );
            return result == 0 ? NotFound() : NoContent();
        }

        [HttpDelete("/DeleteEquipamiento/{id}")]
        public async Task<IActionResult> DeleteEquipamiento(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.ExecuteAsync("DELETE FROM Equipamientos WHERE EquipoId = @EquipoId", new { EquipoId = id });
            return result == 0 ? NotFound() : NoContent();
        }
    }
}


