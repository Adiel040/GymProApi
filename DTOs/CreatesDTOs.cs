namespace GymProApi.DTOs
{
    public static class CreatesDTOs
    {
        public record NewUserDto(
            string Username,
            string Clave,
            string Rol,

            string? NoIdentificacion,
            DateOnly? FechaNacimiento,

            float? Peso,
            float? Altura,
            char? Genero,
            int? SuscripcionId,

            string? Rango
        );

        public record AddEquipamientoDto(
            string Nombre,
            string Descripcion
        );
        public record AddSuscripcionDto(
            string Nombre,
            string Descripcion,
            float Precio
        );
        public record AddProductoDto(
            string Nombre,
            string Categoria,
            float Precio
        );

    }
}
