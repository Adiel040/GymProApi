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
            var clientes = await connection.QueryAsync<Clientes>("SELECT C.*, FLOOR(DATEDIFF(DAY, C.FechaNacimiento, GETDATE()) / 365.25) as Edad ,U.username as Username FROM Clientes C LEFT JOIN Usuarios U ON U.UserId = C.UserId;");
            return clientes.ToList();
        }

        [HttpGet("/GetClienteById/{id}")]
        public async Task<ActionResult<Clientes?>> GetClienteById(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var cliente = await connection.QueryFirstOrDefaultAsync<Clientes>("SELECT C.*,FLOOR(DATEDIFF(DAY, C.FechaNacimiento, GETDATE()) / 365.25) as Edad  ,U.username as Username FROM Clientes C LEFT JOIN Usuarios U ON U.UserId = C.UserId WHERE ClienteId = @ClienteId", new { ClienteId = id });
            return cliente == null ? NotFound() : cliente;
        }

    }
}