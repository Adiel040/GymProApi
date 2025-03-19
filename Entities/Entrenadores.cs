namespace GymProApi.Entities
{
    public class Entrenadores
    {
        public int EntrenadorId { get; set; }
        public int UserId { get; set; }
        public string Rango { get; set; } = string.Empty;
        public int ClientesInscritos { get; set; }
    }
}
