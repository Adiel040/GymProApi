namespace GymProApi.DTOs
{
    public static class CreatesDTOs
    {
        public record RegistUsuario(
            string Username,
            string Clave,
            string Rol,

            string? CorreoElectronico,
            string? NoIdentificacion,
            DateOnly? FechaNacimiento,

            float? Peso,
            float? Altura,
            char? Genero,

            string? Rango
        );

        public record AddEquipamiento(
            string Nombre,
            string Descripcion
        );



        public record AddProducto(
            string Nombre,
            string Categoria,
            float Precio
        );

    }
}
