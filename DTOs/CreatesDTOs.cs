namespace GymProApi.DTOs
{
    public static class CreatesDTOs
    {
        public record AddCliente(
            int UserId,
            string CorreoElectronico,
            string NoIdentificacion,
            int? SuscripcionId = null,
            int? EntrenadorId = null
        );
        public record AddEquipamiento (
            string Nombre,
            string Descripcion
        );
        public record AddEntrenador (
            int UserId,
            string Rango
        );
        public record AddSuscripcion(
            string Nombre,
            string Descripcion,
            float Precio
        );
        public record AddProducto(
            string Nombre,
            string Categoria,
            float Precio
        );
        public record AddUsuario(
            string Username,
            string Clave, 
            string Rol,
            DateTime FechaCreacion
        );
    }
}
