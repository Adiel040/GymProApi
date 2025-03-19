namespace GymProApi.Entities
{
    public class Productos
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public float Precio { get; set; }
    }
}
