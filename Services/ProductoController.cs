using Dapper;
using GymProApi.Entities;
using Microsoft.AspNetCore.Mvc;
using static GymProApi.DTOs.CreatesDTOs;
using Microsoft.Data.SqlClient;

namespace GymProApi.Services
{
    [ApiController]
    [Route("[controller]")]
    public class ProductoController : ControllerBase
    {
        private readonly IConfiguration config;
        public ProductoController(IConfiguration config) { this.config = config; }

        [HttpGet("/GetProductos")]
        public async Task<ActionResult<List<Productos>>> GetProductos()
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var productos = await connection.QueryAsync<Productos>("SELECT * FROM Productos");
            return productos.ToList();
        }


        [HttpGet("/GetProductoById/{id}")]
        public async Task<ActionResult<Productos?>> GetProductoById(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var producto = await connection.QueryFirstOrDefaultAsync<Productos>("SELECT * FROM Productos WHERE ProductoId = @ProductoId", new { ProductoId = id });
            if (producto == null) return NotFound();
            return producto;
        }


        [HttpPost("/AddProducto")]
        public async Task<ActionResult<Productos>> AddProducto(AddProductoDto producto)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.QueryFirstAsync<int>(
                @"INSERT INTO Productos (Nombre, Categoria, Precio) 
                OUTPUT INSERTED.ProductoId
                VALUES (@Nombre, @Categoria, @Precio);",
                new { producto.Nombre, producto.Categoria, producto.Precio }
            );

            return Ok(new Productos
            {
                ProductoId = result,
                Nombre = producto.Nombre,
                Categoria = producto.Categoria,
                Precio = producto.Precio
            });
        }


        [HttpPut("/UpdateProducto")]
        public async Task<IActionResult> UpdateProducto(Productos producto)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.ExecuteAsync(
                "UPDATE Productos SET Nombre = @Nombre, Categoria = @Categoria, Precio = @Precio WHERE ProductoId = @ProductoId",
                new { producto.Nombre, producto.Categoria, producto.Precio,  producto.ProductoId }
            );
            return result == 0 ? NotFound() : NoContent();
        }


        [HttpDelete("/DeleteProducto/{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            using var connection = new SqlConnection(config.GetConnectionString("Prueba"));
            var result = await connection.ExecuteAsync("DELETE FROM Productos WHERE ProductoId = @ProductoId", new { ProductoId = id });
            return result == 0 ? NotFound("No se encontro el producto con ID " + id) : NoContent();
        }
    }
}


