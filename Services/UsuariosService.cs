using Dapper;
using GymProApi.Entities;
using Microsoft.AspNetCore.Mvc;
using static GymProApi.DTOs.CreatesDTOs;
using MD5Hash;
using Microsoft.Data.SqlClient;
using static GymProApi.Constants;
using static GymProApi.DTOs.FetchingDTOs;
using static GymProApi.DTOs.EditsDTOs;

namespace GymProApi.Services
{
    [ApiController]
    [Route("[controller]")]
    public class UsuariosService : ControllerBase
    {
        private IConfiguration config;
        public UsuariosService(IConfiguration config) { this.config = config; }


        [HttpGet("/GetUsuarioById/{id}")]
        public async Task<ActionResult<GetUsuarioDTO>> GetUsuarioById(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var usuario = await connection.QueryFirstOrDefaultAsync<Usuarios>("SELECT * FROM Usuarios WHERE UserId = @UserId", new { UserId = id });
            if (usuario is not null)
            {
                switch (usuario.Rol)
                {
                    case ROLES.ENTRENADOR:
                        {
                            var entrenadorData = await connection.QueryFirstOrDefaultAsync<Entrenadores>("SELECT * FROM Entrenadores WHERE UserId = @UserId;", new { UserId = id });
                            return new ActionResult<GetUsuarioDTO>(new GetUsuarioDTO
                            (
                                UserId: id,
                                Username: usuario.Username,
                                Clave: usuario.Clave,
                                Rol: usuario.Rol,
                                FechaCreacion: usuario.FechaCreacion,

                                CorreoElectronico: null,
                                NoIdentificacion: null,
                                Peso: null,
                                Altura: null,
                                Genero: null,
                                Edad: null,
                                EntrenadorId: null,
                                Entrenador: null,
                                SuscripcionId: null,
                                Suscripcion: null,

                                Rango: entrenadorData?.Rango,
                                ClientesInscritos: entrenadorData?.ClientesInscritos
                            ));
                        }
                    case ROLES.CLIENTE:
                    default:
                        {
                            var cliente = await connection.QueryFirstOrDefaultAsync<Clientes>("SELECT *, DateDiff(YEAR, FechaNacimiento, GETDATE()) as Edad FROM Clientes WHERE UserId = @UserId;", new { usuario.UserId });
                            if (cliente is not null)
                            {
                                var suscripcion = await connection.QueryFirstOrDefaultAsync<Suscripciones>("SELECT * FROM Suscripciones WHERE SuscripcionId = @SuscripcionId", new { cliente.SuscripcionId });
                                var entrenadorData = await connection.QueryFirstOrDefaultAsync<Entrenadores>(@"
                                    SELECT E.*, U.username as Username FROM Entrenadores E 
                                    LEFT JOIN Usuarios U on E.UserId = E.UserId
                                    WHERE E.UserId = @UserId;", new { UserId = cliente.EntrenadorId });

                                return new ActionResult<GetUsuarioDTO>(new GetUsuarioDTO
                            (
                                UserId: id,
                                Username: usuario.Username,
                                Clave: usuario.Clave,
                                Rol: usuario.Rol,
                                FechaCreacion: usuario.FechaCreacion,

                                CorreoElectronico: cliente.CorreoElectronico,
                                NoIdentificacion: cliente.NoIdentificacion,
                                Peso: cliente.Peso,
                                Altura: cliente.Altura,
                                Genero: cliente.Genero,
                                Edad: cliente.Edad,
                                EntrenadorId: cliente.EntrenadorId,
                                Entrenador: entrenadorData?.Username,
                                SuscripcionId: cliente.SuscripcionId,
                                Suscripcion: suscripcion?.Nombre,

                                Rango: entrenadorData?.Rango,
                                ClientesInscritos: entrenadorData?.ClientesInscritos
                            ));
                            }
                            break;
                        }

                }
            }
            return NotFound();
        }

        
        [HttpGet("/Login/{username}/{password}/{rol}")]
        public async Task<ActionResult<GetUsuarioDTO>> Login(string username, string password, string rol)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var usuario = await connection.QueryFirstOrDefaultAsync<Usuarios>($"SELECT * FROM Usuarios WHERE U.Username = @Username AND U.Clave = @Clave AND U.Rol = @Rol", new { Username = username, Clave = password, Rol = rol });
            if (usuario is not null)
            {
                switch (usuario.Rol)
                {
                    case ROLES.ENTRENADOR:
                        {
                            var entrenadorData = await connection.QueryFirstOrDefaultAsync<Entrenadores>("SELECT * FROM Entrenadores WHERE UserId = @UserId;", new { usuario.UserId });
                            return new ActionResult<GetUsuarioDTO>(new GetUsuarioDTO
                            (
                                UserId: usuario.UserId,
                                Username: usuario.Username,
                                Clave: usuario.Clave,
                                Rol: usuario.Rol,
                                FechaCreacion: usuario.FechaCreacion,

                                CorreoElectronico: null,
                                NoIdentificacion: null,
                                Peso: null,
                                Altura: null,
                                Genero: null,
                                Edad: null,
                                EntrenadorId: null,
                                Entrenador: null,
                                SuscripcionId: null,
                                Suscripcion: null,

                                Rango: entrenadorData?.Rango,
                                ClientesInscritos: entrenadorData?.ClientesInscritos
                            ));
                        }
                    case ROLES.CLIENTE:
                    default:
                        {
                            var cliente = await connection.QueryFirstOrDefaultAsync<Clientes>("SELECT *, DateDiff(YEAR, FechaNacimiento, GETDATE()) as Edad FROM Clientes WHERE UserId = @UserId;", new { usuario.UserId });
                            if (cliente is not null)
                            {
                                var suscripcion = await connection.QueryFirstOrDefaultAsync<Suscripciones>("SELECT * FROM Suscripciones WHERE SuscripcionId = @SuscripcionId", new { cliente.SuscripcionId });
                                var entrenadorData = await connection.QueryFirstOrDefaultAsync<Entrenadores>(@"
                                    SELECT E.*, U.username as Username FROM Entrenadores E 
                                    LEFT JOIN Usuarios U on E.UserId = E.UserId
                                    WHERE E.UserId = @UserId;", new { UserId = cliente.EntrenadorId });

                                return new ActionResult<GetUsuarioDTO>(new GetUsuarioDTO
                            (
                                UserId: usuario.UserId,
                                Username: usuario.Username,
                                Clave: usuario.Clave,
                                Rol: usuario.Rol,
                                FechaCreacion: usuario.FechaCreacion,

                                CorreoElectronico: cliente.CorreoElectronico,
                                NoIdentificacion: cliente.NoIdentificacion,
                                Peso: cliente.Peso,
                                Altura: cliente.Altura,
                                Genero: cliente.Genero,
                                Edad: cliente.Edad,
                                EntrenadorId: cliente.EntrenadorId,
                                Entrenador: entrenadorData?.Username,
                                SuscripcionId: cliente.SuscripcionId,
                                Suscripcion: suscripcion?.Nombre,

                                Rango: entrenadorData?.Rango,
                                ClientesInscritos: entrenadorData?.ClientesInscritos
                            ));
                            }
                            break;
                        }

                }
            }
            return NotFound();
        }

        
        [HttpPost("/CreateAccount")]
        public async Task<ActionResult<Usuarios>> CreateAccount(RegistUsuario usuario)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            try
            {
                var result = await connection.QueryFirstAsync<int>(
                    @"INSERT INTO Usuarios (Username, Clave, Rol, FechaCreacion) 
                    OUTPUT INSERTED.UserId
                    VALUES (@Username, @Clave, @Rol, GETDATE());",
                    new { usuario.Username, usuario.Clave, usuario.Rol }
                );

                switch (usuario.Rol)
                {
                    case ROLES.ENTRENADOR:
                        {
                            var result2 = await connection.QueryFirstAsync<int>(@"
                                    INSERT INTO Entrenadores (UserId, Rango, ClientesInscritos) 
                                    OUTPUT INSERTED.EntrenadorId
                                    VALUES (@UserId, @Rango, @ClientesInscritos)
                                ", new
                            {
                                UserId = result,
                                usuario.Rango,
                                ClientesInscritos = 0
                            }
                                                                                );
                            break;
                        }

                    case ROLES.CLIENTE:
                    default: //Caso ADMIN
                        {
                            var result2 = await connection.QueryFirstAsync<int>(@"
                                    INSERT INTO Clientes (UserId, SuscripcionId, EntrenadorId, CorreoElectronico, FechaNacimiento, NoIdentificacion, Peso, Altura, Genero) 
                                    OUTPUT INSERTED.ClienteId
                                    VALUES (@UserId, @SuscripcionId, @EntrenadorId, @CorreoElectronico, @FechaNacimiento, @NoIdentificacion, @Peso, @Altura, @Genero)
                                ", new
                            {
                                UserId = result,
                                SuscripcionId = 0,
                                EntrenadorId = 0,
                                usuario.CorreoElectronico,
                                usuario.FechaNacimiento,
                                usuario.NoIdentificacion,
                                usuario.Peso,
                                usuario.Altura,
                                usuario.Genero
                            }
                                                                                );
                            break;
                        }

                }

                return CreatedAtRoute("GetUsuario", new { id = result }, new Usuarios
                {
                    UserId = result,
                    Username = usuario.Username,
                    Clave = usuario.Clave,
                    Rol = usuario.Rol,
                    FechaCreacion = DateTime.Now
                });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }

        
        [HttpPut("/ChangePassword")]
        public async Task<ActionResult<bool>> ChangePassword(ChangePasswordDto dto)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            try
            {
                var usercount = await connection.QueryFirstAsync<int>("SELECT Count(UserId) FROM Usuarios WHERE UserId = @UserId", new { dto.UserId });

                if (usercount is 0) return BadRequest("Usuario no existe");

                var result = await connection.ExecuteAsync("UPDATE Usuarios SET Clave = @Clave WHERE UserId = @UserId", new { Clave = dto.NewClave, dto.UserId });
                return Ok(result > 0);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPut("/ChangeEntrenador")]
        public async Task<ActionResult<bool>> ChangeEntrenador(ChangeEntrenadorDto dto)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            try
            {
                var cliente = await connection.QueryFirstOrDefaultAsync<Clientes>("SELECT * FROM Clientes WHERE UserId = @UserId", new { dto.UserId });

                if (cliente is null) return BadRequest("El usuario no es cliente");

                var result = await connection.ExecuteAsync("UPDATE Clientes SET EntrenadorId = @EntrenadorId WHERE ClienteId = @ClienteId", new { dto.EntrenadorId, cliente.ClienteId });
                return Ok(result > 0);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPut("/ChangeSuscripcion")]
        public async Task<ActionResult<bool>> ChangeSuscripcion(ChangeSuscripcionDto dto)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            try
            {
                var cliente = await connection.QueryFirstOrDefaultAsync<Clientes>("SELECT * FROM Clientes WHERE UserId = @UserId", new { dto.UserId });

                if (cliente is null) return BadRequest("El usuario no es cliente");

                var result = await connection.ExecuteAsync("UPDATE Clientes SET SuscripcionId = @SuscripcionId WHERE ClienteId = @ClienteId", new { dto.SuscripcionId, cliente.ClienteId });
                return Ok(result > 0);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
