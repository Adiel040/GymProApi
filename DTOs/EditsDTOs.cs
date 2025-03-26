namespace GymProApi.DTOs
{
    public class EditsDTOs
    {
        public record ChangePasswordDto
        (
            string NewClave,
            int UserId
        );
        
        public record ChangeEntrenadorDto(
            int UserId,
            int EntrenadorId
        );

        public record ChangeSuscripcionDto(
            int UserId,
            int SuscripcionId
        );
        public record UpdateSuscripcion(
            int SuscripcionId,
            string Nombre,
            float Precio
        );
    }
}
