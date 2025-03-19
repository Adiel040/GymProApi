using Dapper;
using GymProApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GymProApi.Services
{
    [ApiController]
    [Route("[controller]")]
    public class ProductoController : ControllerBase
    {
        private readonly IConfiguration config;
        public ProductoController(IConfiguration config) { this.config = config; }

        [HttpGet(Name = "GetProductos")]
        public async Task<ActionResult<List<Productos>>> GetProductos()
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var productos = await connection.QueryAsync<Productos>("SELECT * FROM Productos");
            return productos.ToList();
        }

        [HttpGet("{id}", Name = "GetProducto")]
        public async Task<ActionResult<Productos?>> GetProductoById(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var producto = await connection.QueryFirstOrDefaultAsync<Productos>("SELECT * FROM Productos WHERE ProductoId = @ProductoId", new { ProductoId = id });
            if (producto == null) return NotFound();
            return producto;
        }

        [HttpPost(Name = "AddProducto")]
        public async Task<ActionResult<Productos>> AddProducto(Productos producto)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.QueryFirstAsync<int>(
                @"INSERT INTO Productos (Nombre, Categoria, Precio) 
                OUTPUT INSERTED.ProductoId
                VALUES (@Nombre, @Categoria, @Precio);",
                new { producto.Nombre, producto.Categoria, producto.Precio }
            );

            return CreatedAtRoute("GetProducto", new { id = result }, new Productos
            {
                ProductoId = result,
                Nombre = producto.Nombre,
                Categoria = producto.Categoria,
                Precio = producto.Precio
            });
        }

        [HttpPut("{id}", Name = "UpdateProducto")]
        public async Task<IActionResult> UpdateProducto(int id, Productos producto)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.ExecuteAsync(
                "UPDATE Productos SET Nombre = @Nombre, Categoria = @Categoria, Precio = @Precio WHERE ProductoId = @ProductoId",
                new { producto.Nombre, producto.Categoria, producto.Precio, ProductoId = id }
            );
            if (result == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteProducto")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.ExecuteAsync("DELETE FROM Productos WHERE ProductoId = @ProductoId", new { ProductoId = id });
            if (result == 0) return NotFound();
            return NoContent();
        }
    }
}


