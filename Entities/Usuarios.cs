namespace GymProApi.Entities
{
    public class Usuarios
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Clave { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
