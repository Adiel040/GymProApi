namespace GymProApi.DTOs
{
    public static class FetchingDTOs
    {
        public record GetUsuarioDTO (
            int UserId,
            string Username,
            string Clave,
            string Rol,
            DateTime FechaCreacion,

            //Cliente Data
            string? CorreoElectronico,
            string? NoIdentificacion,
            float? Peso,
            float? Altura,
            char? Genero, 
            int? Edad,

            int? EntrenadorId,
            string? Entrenador,

            int? SuscripcionId,
            string? Suscripcion,

            //EntrenadorData
            string? Rango,
            int? ClientesInscritos
        );
    }
}
