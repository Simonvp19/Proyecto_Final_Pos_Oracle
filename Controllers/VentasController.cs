using Microsoft.AspNetCore.Mvc;
using Proyecto_Final.DTOs;
using Proyecto_Final.Services;

namespace Proyecto_Final.Controllers
{
    /// <summary>
    /// Controller para la gestión de ventas del sistema POS
    /// Permite registrar ventas, consultar historial y calcular totales por turno
    /// Delega la lógica de negocio al VentaServices
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly VentaServices _ventaServices;

        /// <summary>
        /// Constructor con inyección del servicio de ventas
        /// </summary>
        public VentasController(VentaServices ventaServices)
        {
            _ventaServices = ventaServices;
        }

        // =========================================
        // GET: api/Ventas
        // Consulta: historial completo de ventas
        // =========================================

        /// <summary>
        /// Retorna el historial completo de ventas del sistema
        /// con nombre de cliente, empleado, sucursal y detalle de productos
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetVentas()
        {
            var ventas = await _ventaServices.ObtenerTodasAsync();
            return Ok(ventas);
        }

        // =========================================
        // GET: api/Ventas/5
        // Consulta: venta por ID
        // =========================================

        /// <summary>
        /// Retorna el detalle completo de una venta específica
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVenta(int id)
        {
            var venta = await _ventaServices.ObtenerPorIdAsync(id);

            if (venta == null)
                return NotFound(new { mensaje = "Venta no encontrada." });

            return Ok(venta);
        }

        // =========================================
        // GET: api/Ventas/sucursal/1
        // Consulta: ventas por sucursal con filtro de fechas
        // =========================================

        /// <summary>
        /// Retorna el historial de ventas de una sucursal
        /// Se puede filtrar por rango de fechas con los parámetros 'desde' y 'hasta'
        /// Ejemplo: /api/Ventas/sucursal/1?desde=2026-01-01&hasta=2026-12-31
        /// </summary>
        [HttpGet("sucursal/{idSucursal}")]
        public async Task<IActionResult> GetVentasPorSucursal(
            int idSucursal,
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta)
        {
            var ventas = await _ventaServices.ObtenerPorSucursalAsync(idSucursal, desde, hasta);
            return Ok(ventas);
        }

        // =========================================
        // GET: api/Ventas/sucursal/1/total-hoy
        // Consulta: total del día (corte de caja)
        // =========================================

        /// <summary>
        /// Retorna el total de ventas del día actual en una sucursal
        /// Se usa para verificar el estado antes de generar el corte de caja
        /// </summary>
        [HttpGet("sucursal/{idSucursal}/total-hoy")]
        public async Task<IActionResult> GetTotalHoy(int idSucursal)
        {
            var total = await _ventaServices.ObtenerTotalDiaAsync(idSucursal);

            return Ok(new
            {
                idSucursal,
                fecha = DateTime.Today.ToString("yyyy-MM-dd"),
                totalHoy = total
            });
        }

        // =========================================
        // POST: api/Ventas
        // Alta: registrar nueva venta
        // =========================================

        /// <summary>
        /// Registra una nueva venta en el sistema
        /// Descuenta automáticamente el inventario de cada producto vendido
        /// Valida stock disponible antes de procesar
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PostVenta(CreateVentaDTO dto)
        {
            var (exito, mensaje, resultado) = await _ventaServices.RegistrarVentaAsync(dto);

            if (!exito)
                return BadRequest(new { mensaje });

            return Ok(new { mensaje, resultado });
        }
    }
}
