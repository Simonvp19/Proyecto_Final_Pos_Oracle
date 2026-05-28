using Microsoft.AspNetCore.Mvc;
using Proyecto_Final.DTOs;
using Proyecto_Final.Services;

namespace Proyecto_Final.Controllers
{
    /// <summary>
    /// Controller para la gestión de productos del catálogo
    /// Expone endpoints de Alta, Baja, Actualización y Consulta (CRUD)
    /// Delega la lógica de negocio al ProductoServices
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly ProductoServices _productoServices;

        /// <summary>
        /// Constructor con inyección del servicio de productos
        /// </summary>
        public ProductosController(ProductoServices productoServices)
        {
            _productoServices = productoServices;
        }

        // =========================================
        // GET: api/Productos
        // Consulta: todos los productos activos
        // =========================================

        /// <summary>
        /// Retorna todos los productos activos con información de su proveedor
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProductos()
        {
            var productos = await _productoServices.ObtenerProductosActivosAsync();
            return Ok(productos);
        }

        // =========================================
        // GET: api/Productos/todos
        // Consulta: todos incluyendo inactivos
        // =========================================

        /// <summary>
        /// Retorna todos los productos sin importar su estado (activo/inactivo)
        /// Útil para administración del catálogo
        /// </summary>
        [HttpGet("todos")]
        public async Task<IActionResult> GetTodos()
        {
            var productos = await _productoServices.ObtenerTodosAsync();
            return Ok(productos);
        }

        // =========================================
        // GET: api/Productos/5
        // Consulta: producto por ID
        // =========================================

        /// <summary>
        /// Retorna el detalle de un producto específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProducto(int id)
        {
            var producto = await _productoServices.ObtenerPorIdAsync(id);

            if (producto == null)
                return NotFound(new { mensaje = "Producto no encontrado." });

            return Ok(producto);
        }

        // =========================================
        // POST: api/Productos
        // Alta: crear producto
        // =========================================

        /// <summary>
        /// Crea un nuevo producto en el catálogo
        /// El proveedor indicado debe existir en el sistema
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PostProducto(ProductoDTO dto)
        {
            var producto = await _productoServices.CrearAsync(dto);

            if (producto == null)
                return BadRequest(new { mensaje = "El proveedor indicado no existe." });

            return Ok(new
            {
                mensaje = "Producto creado correctamente.",
                producto
            });
        }

        // =========================================
        // PUT: api/Productos/5
        // Actualización: editar producto
        // =========================================

        /// <summary>
        /// Actualiza los datos de un producto existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, ProductoDTO dto)
        {
            var actualizado = await _productoServices.ActualizarAsync(id, dto);

            if (!actualizado)
                return NotFound(new { mensaje = "Producto no encontrado." });

            return Ok(new { mensaje = "Producto actualizado correctamente." });
        }

        // =========================================
        // PATCH: api/Productos/5/desactivar
        // Baja lógica: marcar producto como inactivo
        // =========================================

        /// <summary>
        /// Realiza la baja lógica de un producto marcándolo como inactivo
        /// El producto se conserva en la base de datos para mantener el historial de ventas
        /// </summary>
        [HttpPatch("{id}/desactivar")]
        public async Task<IActionResult> DesactivarProducto(int id)
        {
            var desactivado = await _productoServices.DesactivarAsync(id);

            if (!desactivado)
                return NotFound(new { mensaje = "Producto no encontrado." });

            return Ok(new { mensaje = "Producto desactivado correctamente." });
        }

        // =========================================
        // DELETE: api/Productos/5
        // Baja física: eliminar producto
        // =========================================

        /// <summary>
        /// Elimina físicamente un producto del sistema
        /// Solo se permite si el producto no tiene ventas registradas
        /// Si ya tiene ventas, use el endpoint de baja lógica (PATCH/desactivar)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var (exito, mensaje) = await _productoServices.EliminarAsync(id);

            if (!exito)
                return BadRequest(new { mensaje });

            return Ok(new { mensaje });
        }
    }
}
