using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Final.Data;
using Proyecto_Final.DTOs;
using Proyecto_Final.Models;

namespace Proyecto_Final.Controllers
{
    /// <summary>
    /// Controller para la gestión de reportes de corte de caja
    /// Permite generar, consultar y eliminar reportes por sucursal
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReportesController : ControllerBase
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Constructor con inyección del contexto de base de datos
        /// </summary>
        public ReportesController(AppDbContext context)
        {
            _context = context;
        }

        // =========================================
        // GET: api/Reportes
        // Consulta: todos los reportes
        // =========================================

        /// <summary>
        /// Retorna todos los reportes de corte de caja registrados
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reporte>>> GetReportes()
        {
            return await _context.Reportes
                .Include(r => r.Reportes_Venta)
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();
        }

        // =========================================
        // GET: api/Reportes/5
        // Consulta: reporte por ID
        // =========================================

        /// <summary>
        /// Retorna un reporte específico con sus ventas asociadas
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Reporte>> GetReporte(int id)
        {
            var reporte = await _context.Reportes
                .Include(r => r.Reportes_Venta)
                    .ThenInclude(rv => rv.Venta)
                .FirstOrDefaultAsync(r => r.IdReporte == id);

            if (reporte == null)
                return NotFound(new { mensaje = "Reporte no encontrado." });

            return reporte;
        }

        // =========================================
        // GET: api/Reportes/sucursal/1/hoy
        // Consulta: resumen de ventas del día en una sucursal
        // =========================================

        /// <summary>
        /// Genera un resumen de las ventas del día de una sucursal
        /// Útil para visualizar el estado antes de hacer el corte
        /// </summary>
        [HttpGet("sucursal/{idSucursal}/hoy")]
        public async Task<ActionResult> GetResumenHoy(int idSucursal)
        {
            var hoy = DateTime.Today;

            var ventasHoy = await _context.Ventas
                .Where(v => v.IdSucursal == idSucursal && v.Fecha.Date == hoy)
                .Include(v => v.DetallesVenta)
                .ToListAsync();

            var totalVentas = ventasHoy.Sum(v => v.TotalFinal);
            var cantidadVentas = ventasHoy.Count;

            return Ok(new
            {
                idSucursal,
                fecha = hoy.ToString("yyyy-MM-dd"),
                cantidadVentas,
                totalVentas,
                mensaje = "Resumen del día calculado correctamente."
            });
        }

        // =========================================
        // POST: api/Reportes
        // Alta: generar corte de caja
        // =========================================

        /// <summary>
        /// Genera un reporte de corte de caja para una sucursal
        /// Calcula el total real de ventas del día, lo compara con
        /// lo declarado por el cajero y registra el descuadre
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Reporte>> GenerarCorte(CrearReporteDTO dto)
        {
            // Verificar que la sucursal exista
            var sucursalExiste = await _context.Sucursales
                .AnyAsync(s => s.IdSucursal == dto.IdSucursal);

            if (!sucursalExiste)
                return BadRequest(new { mensaje = "Sucursal no encontrada." });

            var hoy = DateTime.Today;

            // Obtener ventas del día en esa sucursal
            var ventasHoy = await _context.Ventas
                .Where(v => v.IdSucursal == dto.IdSucursal && v.Fecha.Date == hoy)
                .ToListAsync();

            if (ventasHoy.Count == 0)
                return BadRequest(new
                {
                    mensaje = "No hay ventas registradas hoy en esta sucursal."
                });

            // Calcular totales reales del sistema
            decimal totalRealVenta = ventasHoy.Sum(v => v.TotalFinal);

            // Total estimado = efectivo declarado + tarjeta
            decimal totalEstimado = dto.TotalEfectivoDeclarado + dto.TotalTarjeta;

            // Descuadre = real - estimado (positivo = sobra, negativo = falta)
            decimal descuadre = totalRealVenta - totalEstimado;

            // Crear el reporte
            var reporte = new Reporte
            {
                Fecha = DateTime.Now,
                TotalVentaEfectivo = dto.TotalEfectivoDeclarado,
                TotalVentaTarjeta = dto.TotalTarjeta,
                TotalEstimadoVenta = totalEstimado,
                TotalRealVenta = totalRealVenta,
                Descuadre = descuadre
            };

            _context.Reportes.Add(reporte);
            await _context.SaveChangesAsync();

            // Asociar las ventas del día a este reporte
            foreach (var venta in ventasHoy)
            {
                _context.Reportes_Venta.Add(new Reporte_Venta
                {
                    IdReporte = reporte.IdReporte,
                    IdVenta = venta.IdVenta
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Corte de caja generado correctamente.",
                idReporte = reporte.IdReporte,
                fecha = reporte.Fecha,
                totalRealVenta,
                totalEstimado,
                descuadre,
                estado = descuadre == 0 ? "Cuadrado" :
                                     descuadre > 0 ? "Sobrante" : "Faltante"
            });
        }

        // =========================================
        // PUT: api/Reportes/5
        // Actualización: corregir montos declarados
        // =========================================

        /// <summary>
        /// Permite corregir los montos declarados de efectivo y tarjeta de un reporte
        /// Recalcula automáticamente el descuadre
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReporte(int id, CrearReporteDTO dto)
        {
            var reporte = await _context.Reportes.FindAsync(id);

            if (reporte == null)
                return NotFound(new { mensaje = "Reporte no encontrado." });

            // Actualizar montos declarados y recalcular descuadre
            reporte.TotalVentaEfectivo = dto.TotalEfectivoDeclarado;
            reporte.TotalVentaTarjeta = dto.TotalTarjeta;
            reporte.TotalEstimadoVenta = dto.TotalEfectivoDeclarado + dto.TotalTarjeta;
            reporte.Descuadre = reporte.TotalRealVenta - reporte.TotalEstimadoVenta;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Reporte actualizado correctamente.",
                descuadre = reporte.Descuadre,
                estado = reporte.Descuadre == 0 ? "Cuadrado" :
                            reporte.Descuadre > 0 ? "Sobrante" : "Faltante"
            });
        }

        // =========================================
        // DELETE: api/Reportes/5
        // Baja: eliminar reporte
        // =========================================

        /// <summary>
        /// Elimina un reporte y sus asociaciones con ventas
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReporte(int id)
        {
            var reporte = await _context.Reportes
                .Include(r => r.Reportes_Venta)
                .FirstOrDefaultAsync(r => r.IdReporte == id);

            if (reporte == null)
                return NotFound(new { mensaje = "Reporte no encontrado." });

            // Eliminar asociaciones primero
            _context.Reportes_Venta.RemoveRange(reporte.Reportes_Venta);
            _context.Reportes.Remove(reporte);

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Reporte eliminado correctamente." });
        }
    }
}
