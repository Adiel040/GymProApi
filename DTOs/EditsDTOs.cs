namespace GymProApi.DTOs
{
    public class EditsDTOs
    {
        public record UpdatePesoDto(
            int UserId,
            float Peso
        );
        public record UpdateAlturaDto(
            int UserId,
            float Altura
        );
        

        public record ChangePasswordDto
        (
            string NewClave,
            int UserId
        );
        
        public record ChangeEntrenadorDto(
            int UserId,
            int NewEntrenadorId,
            int OldEntrenadorId
        );

        public record ChangeSuscripcionDto(
            int UserId,
            int NewSuscripcionId,
            int OldSuscripcionId
        );
        public record UpdateSuscripcionDto(
            int SuscripcionId,
            string Nombre,
            string Descripcion,
            float Precio
        );

        public record UpdateEntrenadorDto(
            int EntrenadorId,
            string Rango
        );

    }
}
