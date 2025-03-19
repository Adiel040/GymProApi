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
        public async Task<ActionResult<List<Clientes>>> GetClientes()
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var clientes = await connection.QueryAsync<Clientes>("SELECT * FROM Clientes");
            return clientes.ToList();
        }

        [HttpGet("{id}", Name = "GetCliente")]
        public async Task<ActionResult<Clientes?>> GetClienteById(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var cliente = await connection.QueryFirstOrDefaultAsync<Clientes>("SELECT * FROM Clientes WHERE ClienteId = @ClienteId", new { ClienteId = id });
            if (cliente == null) return NotFound();
            return cliente;
        }

        [HttpPost(Name = "AddCliente")]
        public async Task<ActionResult<Clientes>> AddCliente(Clientes cliente)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.QueryFirstAsync<int>(
                @"INSERT INTO Clientes (UserId, SuscripcionId, EntrenadorId, CorreoElectronico, NoIdentificacion) 
                OUTPUT INSERTED.ClienteId
                VALUES (@UserId, @SuscripcionId, @EntrenadorId, @CorreoElectronico, @NoIdentificacion);",
                new { cliente.UserId, cliente.SuscripcionId, cliente.EntrenadorId, cliente.CorreoElectronico, cliente.NoIdentificacion }
            );

            return CreatedAtRoute("GetCliente", new { id = result }, new Clientes
            {
                ClienteId = result,
                UserId = cliente.UserId,
                SuscripcionId = cliente.SuscripcionId,
                EntrenadorId = cliente.EntrenadorId,
                CorreoElectronico = cliente.CorreoElectronico,
                NoIdentificacion = cliente.NoIdentificacion
            });
        }

        [HttpPut("{id}", Name = "UpdateCliente")]
        public async Task<IActionResult> UpdateCliente(int id, Clientes cliente)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.ExecuteAsync(
                "UPDATE Clientes SET UserId = @UserId, SuscripcionId = @SuscripcionId, EntrenadorId = @EntrenadorId, CorreoElectronico = @CorreoElectronico, NoIdentificacion = @NoIdentificacion WHERE ClienteId = @ClienteId",
                new { cliente.UserId, cliente.SuscripcionId, cliente.EntrenadorId, cliente.CorreoElectronico, cliente.NoIdentificacion, ClienteId = id }
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



