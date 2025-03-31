using static GymProApi.DTOs.FetchingDTOs;
using static GymProApi.DTOs.CreatesDTOs;
using static GymProApi.DTOs.EditsDTOs;
using static GymProApi.Utils.Utilities;
using static GymProApi.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using GymProApi.Entities;
using Dapper;

namespace GymProApi.Services
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private IConfiguration config;
        public UsuarioController(IConfiguration config) { this.config = config; }

        [HttpGet("/GetClientesByEntrenador/{id}")]
        public async Task<ActionResult<IEnumerable<GetUsuarioDTO>>> GetClientesByEntrenador(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var clientes = await connection.QueryAsync<Clientes>(@"SELECT *,  FLOOR(DATEDIFF(DAY, FechaNacimiento, GETDATE()) / 365.25) as Edad FROM Clientes WHERE EntrenadorId = @EntrenadorId", new { EntrenadorId = id });
            if (clientes is null) return NotFound("No se encontraron clientes para ese entrenador");
            var result = new List<GetUsuarioDTO>();
            Suscripciones? suscripcion;
            foreach (var cliente in clientes)
            {
                suscripcion = await connection.QueryFirstOrDefaultAsync<Suscripciones>("SELECT * FROM Suscripciones WHERE SuscripcionId = @SuscripcionId", new { cliente.SuscripcionId });
                var usuario = await connection.QueryFirstOrDefaultAsync<Usuarios>("SELECT * FROM Usuarios WHERE UserId = @UserId", new { cliente.UserId });
                if (usuario is null) continue;
                result.Add(new GetUsuarioDTO
                (
                    UserId: cliente.UserId,
                    Username: usuario.Username,
                    Clave: usuario.Clave,
                    Rol: ROLES.CLIENTE,
                    FechaCreacion: usuario.FechaCreacion.ToString("yyyy-MM-dd"),
                    ClienteId: cliente.ClienteId,
                    NoIdentificacion: cliente.NoIdentificacion,
                    FechaNacimiento: cliente.FechaNacimiento.ToString("yyyy-MM-dd"),
                    Peso: cliente.Peso,
                    Altura: cliente.Altura,
                    Genero: cliente.Genero,
                    Edad: cliente.Edad,
                    EntrenadorId: cliente.EntrenadorId,
                    Entrenador: null,
                    SuscripcionId: cliente.SuscripcionId,
                    Suscripcion: suscripcion?.Nombre,
                    Rango: null,
                    ClientesInscritos: null
                ));
            }
            return Ok(result);
        }

        [HttpGet("/GetUsuarioById/{id}")]
        public async Task<ActionResult<GetUsuarioDTO>> GetUsuarioById(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var usuario = await connection.QueryFirstOrDefaultAsync<Usuarios>("SELECT * FROM Usuarios WHERE UserId = @UserId", new { UserId = id });
            if (usuario is not null)
            {

                if (usuario.Rol is ROLES.ENTRENADOR)
                {
                    var entrenadorData = await connection.QueryFirstOrDefaultAsync<Entrenadores>("SELECT * FROM Entrenadores WHERE UserId = @UserId;", new { UserId = id });
                    return new ActionResult<GetUsuarioDTO>(new GetUsuarioDTO
                    (
                        UserId: id,
                        Username: usuario.Username,
                        Clave: usuario.Clave,
                        Rol: usuario.Rol,
                        FechaCreacion: usuario.FechaCreacion.ToString("yyyy-MM-dd"),

                        NoIdentificacion: null,
                        ClienteId: null,
                        FechaNacimiento: null,
                        Peso: null,
                        Altura: null,
                        Genero: null,
                        Edad: null,
                        EntrenadorId: entrenadorData?.EntrenadorId,
                        Entrenador: null,
                        SuscripcionId: null,
                        Suscripcion: null,

                        Rango: entrenadorData?.Rango,
                        ClientesInscritos: entrenadorData?.ClientesInscritos
                    ));
                }
                else
                {
                    var cliente = await connection.QueryFirstOrDefaultAsync<Clientes>(@"
                                SELECT *, 
                                       FLOOR(DATEDIFF(DAY, FechaNacimiento, GETDATE()) / 365.25) as Edad 
                                FROM Clientes 
                                WHERE UserId = @UserId;",
                                 new { usuario.UserId });

                    Entrenadores? entrenadorData = null;
                    Suscripciones? suscripcion = null;
                    if (cliente is not null)
                    {
                        suscripcion = await connection.QueryFirstOrDefaultAsync<Suscripciones>("SELECT * FROM Suscripciones WHERE SuscripcionId = @SuscripcionId", new { cliente.SuscripcionId });
                        entrenadorData = await connection.QueryFirstOrDefaultAsync<Entrenadores>(@"
                                    SELECT E.*, U.username as Username FROM Entrenadores E 
                                    LEFT JOIN Usuarios U on E.UserId = U.UserId
                                    WHERE E.EntrenadorId = @UserId;", new { UserId = cliente.EntrenadorId });

                    }
                    return new ActionResult<GetUsuarioDTO>(new GetUsuarioDTO
                        (
                            UserId: id,
                            Username: usuario.Username,
                            Clave: usuario.Clave,
                            Rol: usuario.Rol,
                            FechaCreacion: usuario.FechaCreacion.ToString("yyyy-MM-dd"),

                            ClienteId: cliente?.ClienteId,
                            NoIdentificacion: cliente?.NoIdentificacion,
                            FechaNacimiento: cliente?.FechaNacimiento.ToString("yyyy-MM-dd"),
                            Peso: cliente?.Peso,
                            Altura: cliente?.Altura,
                            Genero: cliente?.Genero,
                            Edad: cliente?.Edad,
                            EntrenadorId: cliente?.EntrenadorId,
                            Entrenador: entrenadorData?.Username,
                            SuscripcionId: cliente?.SuscripcionId,
                            Suscripcion: suscripcion?.Nombre,

                            Rango: entrenadorData?.Rango,
                            ClientesInscritos: entrenadorData?.ClientesInscritos
                        ));
                }
            }
            else
            {

                return NotFound();
            }
        }


        [HttpGet("/Login/{username}/{password}")]
        public async Task<ActionResult<GetUsuarioDTO>> Login(string username, string password)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var usuario = await connection.QueryFirstOrDefaultAsync<Usuarios>($"SELECT * FROM Usuarios WHERE Username = @Username AND Clave = @Clave", new { Username = username, Clave = password });

            if (usuario is null) return NotFound("No se encontró usuario con esas credenciales");

            switch (usuario.Rol)
            {
                case ROLES.ENTRENADOR:
                    {
                        var entrenadorData = await connection.QueryFirstOrDefaultAsync<Entrenadores>("SELECT * FROM Entrenadores WHERE UserId = @UserId;", new { usuario.UserId });
                        if (entrenadorData is null) return NotFound("El usuario no tiene informacion de entrenador");

                        return new ActionResult<GetUsuarioDTO>(new GetUsuarioDTO
                            (
                                UserId: usuario.UserId,
                                Username: usuario.Username,
                                Clave: usuario.Clave,
                                Rol: usuario.Rol,
                                FechaCreacion: usuario.FechaCreacion.ToString("yyyy-MM-dd"),

                                ClienteId: null,
                                FechaNacimiento: null,
                                NoIdentificacion: null,
                                Peso: null,
                                Altura: null,
                                Genero: null,
                                Edad: null,
                                EntrenadorId: entrenadorData.EntrenadorId,
                                Entrenador: null,
                                SuscripcionId: null,
                                Suscripcion: null,

                                Rango: entrenadorData.Rango,
                                ClientesInscritos: entrenadorData.ClientesInscritos
                            ));
                    }
                case ROLES.CLIENTE:
                    {
                        var cliente = await connection.QueryFirstOrDefaultAsync<Clientes>(@"
                                SELECT *, 
                                       FLOOR(DATEDIFF(DAY, FechaNacimiento, GETDATE()) / 365.25) as Edad 
                                FROM Clientes 
                                WHERE UserId = @UserId;",
                                 new { usuario.UserId });

                        if (cliente is null) return NotFound("El usuario no tiene informacion de cliente");

                        var suscripcion = await connection.QueryFirstOrDefaultAsync<Suscripciones>("SELECT * FROM Suscripciones WHERE SuscripcionId = @SuscripcionId", new { cliente.SuscripcionId });
                        var entrenadorData = await connection.QueryFirstOrDefaultAsync<Entrenadores>(@"
                                    SELECT E.*, U.username as Username FROM Entrenadores E 
                                    LEFT JOIN Usuarios U on E.UserId = U.UserId
                                    WHERE E.EntrenadorId= @UserId;", new { UserId = cliente.EntrenadorId });

                        return new ActionResult<GetUsuarioDTO>(new GetUsuarioDTO
                    (
                        UserId: usuario.UserId,
                        Username: usuario.Username,
                        Clave: usuario.Clave,
                        Rol: usuario.Rol,
                        FechaCreacion: usuario.FechaCreacion.ToString("yyyy-MM-dd"),

                        ClienteId: cliente.ClienteId,
                        FechaNacimiento: cliente.FechaNacimiento.ToString("yyyy-MM-dd"),
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
                default: return NotFound("El usuario no tiene rol asignado");

            }
        }


        [HttpPost("/CreateAccount")]
        public async Task<ActionResult<Usuarios>> CreateAccount(NewUserDto usuario)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            try
            {
                var existingUser = await connection.QueryFirstOrDefaultAsync<Usuarios>("SELECT * FROM Usuarios WHERE Username = @Username", new { usuario.Username });
                if (existingUser is not null) return BadRequest("El nombre de usuario ya existe.");


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
                                    VALUES (@UserId, @Rango, 0)
                                ", new
                            {
                                UserId = result,
                                usuario.Rango
                            });
                            break;
                        }

                    case ROLES.CLIENTE:
                    default: //Caso ADMIN
                        {
                            var result2 = await connection.QueryFirstAsync<int>(@"
                                    INSERT INTO Clientes (UserId, SuscripcionId, EntrenadorId, FechaNacimiento, NoIdentificacion, Peso, Altura, Genero) 
                                    OUTPUT INSERTED.ClienteId
                                    VALUES (@UserId, @SuscripcionId, @EntrenadorId, @FechaNacimiento, @NoIdentificacion, @Peso, @Altura, @Genero)
                                ", new
                            {
                                UserId = result,
                                usuario.SuscripcionId,
                                EntrenadorId = 0,
                                FechaNacimiento = usuario.FechaNacimiento.ToSqlDateFormat(),
                                usuario.NoIdentificacion,
                                usuario.Peso,
                                usuario.Altura,
                                usuario.Genero
                            });

                            await connection.ExecuteAsync("UPDATE Suscripciones SET ClientesSuscritos = ClientesSuscritos + 1 WHERE SuscripcionId = @SuscripcionId", new { usuario.SuscripcionId });
                            break;
                        }
                }

                return Ok(new Usuarios
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
            try
            {
                using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
                const int EXPECTED_CHANGES = 3;

                var cliente = await connection.QueryFirstOrDefaultAsync<int?>("SELECT ClienteId FROM Clientes WHERE UserId = @UserId", new { dto.UserId });

                if (cliente is 0 or null) return BadRequest("El usuario no es cliente");

                var changesMade = await connection.ExecuteAsync("UPDATE Clientes SET EntrenadorId = @EntrenadorId WHERE ClienteId = @ClienteId", new { EntrenadorId = dto.NewEntrenadorId, ClienteId = cliente });
                changesMade += await connection.ExecuteAsync("UPDATE Entrenadores SET ClientesInscritos = ClientesInscritos + 1 WHERE EntrenadorId = @EntrenadorId", new { EntrenadorId = dto.NewEntrenadorId });

                changesMade += (dto.OldEntrenadorId is 0) ? 1 : await connection.ExecuteAsync("UPDATE Entrenadores SET ClientesInscritos = ClientesInscritos - 1 WHERE EntrenadorId = @EntrenadorId", new { EntrenadorId = dto.OldEntrenadorId });
                return Ok(changesMade is EXPECTED_CHANGES);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }


        [HttpPut("/ChangeSuscripcion")]
        public async Task<ActionResult<bool>> ChangeSuscripcion(ChangeSuscripcionDto dto)
        {
            try
            {
                using var connection = new SqlConnection(config.GetConnectionString("Prueba"));

                const int EXPECTED_CHANGES = 3;

                var cliente = await connection.QueryFirstOrDefaultAsync<int?>("SELECT ClienteId FROM Clientes WHERE UserId = @UserId", new { dto.UserId });

                if (cliente is null or 0) return BadRequest("El usuario no es cliente");

                var allChanges = await connection.ExecuteAsync("UPDATE Clientes SET SuscripcionId = @SuscripcionId WHERE ClienteId = @ClienteId", new { SuscripcionId = dto.NewSuscripcionId, ClienteId = cliente });
                allChanges += await connection.ExecuteAsync("UPDATE Suscripciones SET ClientesSuscritos = ClientesSuscritos + 1 WHERE SuscripcionId = @SuscripcionId", new { SuscripcionId = dto.NewSuscripcionId });
                allChanges += (dto.OldSuscripcionId is 0) ?
                     1
                    : await connection.ExecuteAsync("UPDATE Suscripciones SET ClientesSuscritos = ClientesSuscritos - 1 WHERE SuscripcionId = @SuscripcionId", new { SuscripcionId = dto.OldSuscripcionId });

                return Ok(allChanges is EXPECTED_CHANGES);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }


        [HttpPut("/UpdatePeso")]
        public async Task<ActionResult<bool>> UpdatePeso(UpdatePesoDto dto)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            try
            {
                var cliente = await connection.QueryFirstOrDefaultAsync<int?>("SELECT ClienteId FROM Clientes WHERE UserId = @UserId", new { dto.UserId });

                if (cliente is null or 0) return BadRequest("El usuario no es cliente");

                var result = await connection.ExecuteAsync("UPDATE Clientes SET Peso = @Peso WHERE ClienteId = @ClienteId", new { dto.Peso, ClienteId = cliente });
                return Ok(result > 0);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }


        [HttpPut("/UpdateAltura")]
        public async Task<ActionResult<bool>> UpdateAltura(UpdateAlturaDto dto)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            try
            {
                var cliente = await connection.QueryFirstOrDefaultAsync<Clientes>("SELECT * FROM Clientes WHERE UserId = @UserId", new { dto.UserId });

                if (cliente is null) return BadRequest("El usuario no es cliente");

                var result = await connection.ExecuteAsync("UPDATE Clientes SET Altura = @Altura WHERE ClienteId = @ClienteId", new { dto.Altura, cliente.ClienteId });
                return Ok(result > 0);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }


        [HttpDelete("/DeleteAccount/{id}")]
        public async Task<ActionResult<bool>> DeleteAccount(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            try
            {
                var usuario = await connection.QueryFirstOrDefaultAsync<Usuarios>("SELECT * FROM Usuarios WHERE UserId = @UserId", new { UserId = id });
                if (usuario is null) return BadRequest("Usuario no existe");

                if (usuario.Rol == ROLES.CLIENTE)
                {
                    var cliente = await connection.QueryFirstOrDefaultAsync<Clientes>("SELECT * FROM Clientes WHERE UserId = @UserId", new { UserId = id });
                    if (cliente is not null)
                    {
                        await connection.ExecuteAsync("UPDATE Suscripciones SET ClientesSuscritos = ClientesSuscritos - 1 WHERE SuscripcionId = @SuscripcionId", new { cliente.SuscripcionId });
                        await connection.ExecuteAsync("UPDATE Entrenadores SET ClientesInscritos = ClientesInscritos - 1 WHERE EntrenadorId = @EntrenadorId", new { cliente.EntrenadorId });
                    }
                }
                else if (usuario.Rol == ROLES.ENTRENADOR)
                {
                    await connection.ExecuteAsync("UPDATE Clientes SET EntrenadorId = NULL WHERE EntrenadorId = @EntrenadorId", new { EntrenadorId = id });
                }

                var result = await connection.ExecuteAsync("DELETE FROM Usuarios WHERE UserId = @UserId", new { UserId = id });
                return Ok(result > 0);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
