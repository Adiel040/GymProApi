namespace GymProApi.Entities
{
    public class Clientes
    {
        public int ClienteId { get; set; }
        public int UserId { get; set; }
        public int SuscripcionId { get; set; }
        public int EntrenadorId { get; set; }
        public string CorreoElectronico { get; set; } = string.Empty;
        public string NoIdentificacion { get; set; } = string.Empty;
    }
}
