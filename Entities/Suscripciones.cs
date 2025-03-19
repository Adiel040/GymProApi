namespace GymProApi.Entities
{
    public class Suscripciones
    {
        public int SuscripcionId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public float Precio { get; set; }
        public int ClientesSuscritos { get; set; }
    }
}
