using Dapper;
using GymProApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using static GymProApi.DTOs.CreatesDTOs;
using MD5Hash;

namespace GymProApi.Services
{
    [ApiController]
    [Route("[controller]")]
    public class UsuariosService : ControllerBase
    {
        private IConfiguration config;
        public UsuariosService(IConfiguration config) { this.config = config; }

        [HttpGet(Name = "GetUsuarios")]
        public async Task<ActionResult<List<Usuarios>>> GetUsuarios()
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var productos = await connection.QueryAsync<Usuarios>("SELECT * FROM Usuarios");
            return productos.ToList();
        }
        [HttpGet("{id}", Name = "GetUsuario")]
        public async Task<ActionResult<Usuarios?>> GetUsuarioById(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var usuario = await connection.QueryFirstOrDefaultAsync<Usuarios>("SELECT * FROM Usuarios WHERE UserId = @UserId", new { UserId = id });
            if (usuario == null) return NotFound();
            return usuario;
        }
        [HttpPost(Name = "AddUsuario")]
        public async Task<ActionResult<Usuarios>> AddUsuario(AddUsuario usuario)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            string Clave = usuario.Clave.GetMD5();
            try
            {
                var result = await connection.QueryFirstAsync<int>(
                @"INSERT INTO Usuarios (Username, Clave, Rol, FechaCreacion) 
                OUTPUT INSERTED.UserId
                VALUES (@Username, @Clave, @Rol, GETDATE());",
                new { usuario.Username, Clave, usuario.Rol }
            );

                return CreatedAtRoute("GetUsuario", new { id = result }, new Usuarios
                {
                    UserId = result,
                    Username = usuario.Username,
                    Clave = Clave,
                    Rol = usuario.Rol,
                    FechaCreacion = DateTime.Now
                });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }
        [HttpPut("{id}", Name = "UpdateUsuario")]
        public async Task<IActionResult> UpdateUsuario(int id, AddUsuario usuario)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            string Clave = usuario.Clave.GetMD5();
            var result = await connection.ExecuteAsync(
                "UPDATE Usuarios SET Username = @Username, Clave = @Clave, Rol = @Rol WHERE UserId = @UserId",
                new { usuario.Username, Clave, usuario.Rol, UserId = id }
            );
            if (result == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteUsuario")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.ExecuteAsync("DELETE FROM Usuarios WHERE UserId = @UserId", new { UserId = id });
            if (result == 0) return NotFound();
            return NoContent();
        }
    }
}
