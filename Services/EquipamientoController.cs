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
        public async Task<ActionResult<List<Equipamientos>>> GetEquipamientos()
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var equipamientos = await connection.QueryAsync<Equipamientos>("SELECT * FROM Equipamientos");
            return equipamientos.ToList();
        }

        [HttpGet("{id}", Name = "GetEquipamiento")]
        public async Task<ActionResult<Equipamientos?>> GetEquipamientoById(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var equipamiento = await connection.QueryFirstOrDefaultAsync<Equipamientos>("SELECT * FROM Equipamientos WHERE EquipoId = @EquipoId", new { EquipoId = id });
            if (equipamiento == null) return NotFound();
            return equipamiento;
        }

        [HttpPost(Name = "AddEquipamiento")]
        public async Task<ActionResult<Equipamientos>> AddEquipamiento(Equipamientos equipamiento)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.QueryFirstAsync<int>(
                @"INSERT INTO Equipamientos (Nombre, Descripcion) 
                OUTPUT INSERTED.EquipoId
                VALUES (@Nombre, @Descripcion);",
                new { equipamiento.Nombre, equipamiento.Descripcion }
            );

            return CreatedAtRoute("GetEquipamiento", new { id = result }, new Equipamientos
            {
                EquipoId = result,
                Nombre = equipamiento.Nombre,
                Descripcion = equipamiento.Descripcion
            });
        }

        [HttpPut("{id}", Name = "UpdateEquipamiento")]
        public async Task<IActionResult> UpdateEquipamiento(int id, Equipamientos equipamiento)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.ExecuteAsync(
                "UPDATE Equipamientos SET Nombre = @Nombre, Descripcion = @Descripcion WHERE EquipoId = @EquipoId",
                new { equipamiento.Nombre, equipamiento.Descripcion, EquipoId = id }
            );
            if (result == 0) return NotFound();
            return NoContent();
        }

     
    }
}


