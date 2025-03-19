using Dapper;
using GymProApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GymProApi.Services
{
    [ApiController]
    [Route("[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly IConfiguration config;
        public ClienteController(IConfiguration config) { this.config = config; }

        [HttpGet(Name = "GetClientes")]
        public async Task<ActionResult<List<Cliente>>> GetClientes()
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var clientes = await connection.QueryAsync<Cliente>("SELECT * FROM Clientes");
            return clientes.ToList();
        }

        [HttpGet("{id}", Name = "GetCliente")]
        public async Task<ActionResult<Cliente?>> GetClienteById(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var cliente = await connection.QueryFirstOrDefaultAsync<Cliente>("SELECT * FROM Clientes WHERE ClienteId = @ClienteId", new { ClienteId = id });
            if (cliente == null) return NotFound();
            return cliente;
        }

        [HttpPost(Name = "AddCliente")]
        public async Task<ActionResult<Cliente>> AddCliente(Cliente cliente)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.QueryFirstAsync<int>(
                @"INSERT INTO Clientes (Nombre, Apellido, Email, FechaNacimiento) 
                OUTPUT INSERTED.ClienteId
                VALUES (@Nombre, @Apellido, @Email, @FechaNacimiento);",
                new { cliente.Nombre, cliente.Apellido, cliente.Email, cliente.FechaNacimiento }
            );

            return CreatedAtRoute("GetCliente", new { id = result }, new Cliente
            {
                ClienteId = result,
                Nombre = cliente.Nombre,
                Apellido = cliente.Apellido,
                Email = cliente.Email,
                FechaNacimiento = cliente.FechaNacimiento
            });
        }

        [HttpPut("{id}", Name = "UpdateCliente")]
        public async Task<IActionResult> UpdateCliente(int id, Cliente cliente)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.ExecuteAsync(
                "UPDATE Clientes SET Nombre = @Nombre, Apellido = @Apellido, Email = @Email, FechaNacimiento = @FechaNacimiento WHERE ClienteId = @ClienteId",
                new { cliente.Nombre, cliente.Apellido, cliente.Email, cliente.FechaNacimiento, ClienteId = id }
            );
            if (result == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCliente")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.ExecuteAsync("DELETE FROM Clientes WHERE ClienteId = @ClienteId", new { ClienteId = id });
            if (result == 0) return NotFound();
            return NoContent();
        }
    }
}
