using Microsoft.AspNetCore.Mvc;
using Proyecto_Final.Services;

namespace Proyecto_Final.Controllers
{
    /// <summary>
    /// Controller para la gestión de inventario por sucursal
    /// Permite consultar stock, registrar entradas y salidas de productos
    /// Delega la lógica de negocio al InventarioServices.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class InventarioController : ControllerBase
    {
        private readonly InventarioServices _inventarioServices;

        /// <summary>
        /// Constructor con inyección del servicio de inventario
        /// </summary>
        public InventarioController(InventarioServices inventarioServices)
        {
            _inventarioServices = inventarioServices;
        }

        // =========================================
        // GET: api/Inventario/sucursal/1
        // Consulta: inventario completo de una sucursal
        // =========================================

        /// <summary>
        /// Retorna todos los productos con su stock en una sucursal específica
        /// </summary>
        [HttpGet("sucursal/{idSucursal}")]
        public async Task<IActionResult> GetInventarioPorSucursal(int idSucursal)
        {
            var inventarios = await _inventarioServices.ObtenerPorSucursalAsync(idSucursal);
            return Ok(inventarios);
        }

        // =========================================
        // GET: api/Inventario/1/1
        // Consulta: stock de un producto en una sucursal
        // =========================================

        /// <summary>
        /// Retorna el stock de un producto específico en una sucursal
        /// </summary>
        [HttpGet("{idProducto}/{idSucursal}")]
        public async Task<IActionResult> GetStock(int idProducto, int idSucursal)
        {
            var inventario = await _inventarioServices.ObtenerStockAsync(idProducto, idSucursal);

            if (inventario == null)
                return NotFound(new { mensaje = "No existe registro de inventario para ese producto en esta sucursal." });

            return Ok(inventario);
        }

        // =========================================
        // GET: api/Inventario/sucursal/1/stock-bajo
        // Consulta: productos con stock bajo
        // =========================================

        /// <summary>
        /// Retorna los productos con stock menor o igual al mínimo indicado (default 5)
        /// Útil para generar alertas de reabastecimiento
        /// </summary>
        [HttpGet("sucursal/{idSucursal}/stock-bajo")]
        public async Task<IActionResult> GetStockBajo(int idSucursal, [FromQuery] int minimo = 5)
        {
            var productos = await _inventarioServices.ObtenerStockBajoAsync(idSucursal, minimo);
            return Ok(new
            {
                idSucursal,
                minimoStock = minimo,
                cantidad = productos.Count,
                productos
            });
        }

        // =========================================
        // POST: api/Inventario/entrada
        // Alta: registrar entrada de producto
        // =========================================

        /// <summary>
        /// Registra una entrada de stock para un producto en una sucursal
        /// Si ya existe el inventario, suma la cantidad al stock actual
        /// Si no existe, lo crea con la cantidad indicada
        /// </summary>
        [HttpPost("entrada")]
        public async Task<IActionResult> RegistrarEntrada(
            [FromQuery] int idProducto,
            [FromQuery] int idSucursal,
            [FromQuery] int cantidad)
        {
            var (exito, mensaje, inventario) =
                await _inventarioServices.RegistrarEntradaAsync(idProducto, idSucursal, cantidad);

            if (!exito)
                return BadRequest(new { mensaje });

            return Ok(new { mensaje, inventario });
        }

        // =========================================
        // POST: api/Inventario/salida
        // Baja: registrar salida manual de producto
        // =========================================

        /// <summary>
        /// Registra una salida manual de stock (merma, ajuste, robo, etc.)
        /// Valida que haya suficiente stock antes de descontar
        /// </summary>
        [HttpPost("salida")]
        public async Task<IActionResult> RegistrarSalida(
            [FromQuery] int idProducto,
            [FromQuery] int idSucursal,
            [FromQuery] int cantidad)
        {
            var (exito, mensaje, inventario) =
                await _inventarioServices.RegistrarSalidaAsync(idProducto, idSucursal, cantidad);

            if (!exito)
                return BadRequest(new { mensaje });

            return Ok(new { mensaje, inventario });
        }

        // =========================================
        // PUT: api/Inventario/ajuste
        // Actualización: ajuste directo de stock
        // =========================================

        /// <summary>
        /// Ajusta directamente el stock de un producto en una sucursal
        /// Útil cuando se realiza un conteo físico de inventario
        /// </summary>
        [HttpPut("ajuste")]
        public async Task<IActionResult> AjustarStock(
            [FromQuery] int idProducto,
            [FromQuery] int idSucursal,
            [FromQuery] int nuevoStock)
        {
            var (exito, mensaje) =
                await _inventarioServices.AjustarStockAsync(idProducto, idSucursal, nuevoStock);

            if (!exito)
                return BadRequest(new { mensaje });

            return Ok(new { mensaje });
        }
    }
}
