using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Final.Data;
using Proyecto_Final.DTOs;
using Proyecto_Final.Models;
using Proyecto_Final.Data;
using Proyecto_Final.DTOs;
using Proyecto_Final_API.Models;

namespace Proyecto_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VentasController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET TODAS LAS VENTAS
        // =========================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Venta>>> GetVentas()
        {
            return await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Empleado)
                .Include(v => v.Sucursal)
                .Include(v => v.DetallesVenta)
                .ToListAsync();
        }

        // =========================
        // GET VENTA POR ID
        // =========================
        [HttpGet("{id}")]
        public async Task<ActionResult<Venta>> GetVenta(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Empleado)
                .Include(v => v.Sucursal)
                .Include(v => v.DetallesVenta)
                .FirstOrDefaultAsync(v => v.IdVenta == id);

            if (venta == null)
            {
                return NotFound();
            }

            return venta;
        }

        // =========================
        // GET VENTAS POR SUCURSAL
        // =========================
        [HttpGet("sucursal/{idSucursal}")]
        public async Task<ActionResult<IEnumerable<Venta>>>
            GetVentasPorSucursal(int idSucursal)
        {
            var ventas = await _context.Ventas
                .Where(v => v.IdSucursal == idSucursal)
                .Include(v => v.Cliente)
                .Include(v => v.Empleado)
                .Include(v => v.Sucursal)
                .Include(v => v.DetallesVenta)
                .ToListAsync();

            return ventas;
        }

        // =========================
        // GET TOTAL VENTAS SUCURSAL
        // =========================
        [HttpGet("sucursal/{idSucursal}/total")]
        public async Task<ActionResult>
            GetTotalVentasSucursal(int idSucursal)
        {
            var total = await _context.Ventas
                .Where(v => v.IdSucursal == idSucursal)
                .SumAsync(v => v.TotalFinal);

            return Ok(new
            {
                idSucursal,
                totalVentas = total
            });
        }

        // =========================
        // CREAR VENTA
        // =========================
        [HttpPost]
        public async Task<IActionResult> CrearVenta(CreateVentaDTO dto)
        {
            decimal total = 0;

            var venta = new Venta
            {
                Fecha = DateTime.Now,
                IdCliente = dto.IdCliente,
                IdEmpleado = dto.IdEmpleado,
                IdSucursal = dto.IdSucursal
            };

            _context.Ventas.Add(venta);

            await _context.SaveChangesAsync();

            foreach (var item in dto.Productos)
            {
                // Buscar producto
                var producto = await _context.Productos
                    .FirstOrDefaultAsync(p =>
                        p.IdProducto == item.IdProducto);

                if (producto == null)
                {
                    return BadRequest(
                        $"Producto {item.IdProducto} no encontrado");
                }

                // Buscar inventario
                var inventario = await _context.Inventarios
                    .FirstOrDefaultAsync(i =>
                        i.IdProducto == item.IdProducto &&
                        i.IdSucursal == dto.IdSucursal);

                if (inventario == null)
                {
                    return BadRequest(
                        $"Inventario no encontrado");
                }

                // Validar stock
                if (inventario.Stock < item.Cantidad)
                {
                    return BadRequest(
                        $"Stock insuficiente para " +
                        producto.NombreProducto);
                }

                // Calcular subtotal
                decimal subtotal =
                    producto.PrecioUnitario * item.Cantidad;

                total += subtotal;

                // Crear detalle venta
                var detalle = new DetalleVenta
                {
                    IdVenta = venta.IdVenta,
                    IdProducto = item.IdProducto,
                    Cantidad = item.Cantidad,
                    SubTotal = subtotal
                };

                _context.DetallesVenta.Add(detalle);

                // Descontar inventario
                inventario.Stock -= item.Cantidad;
            }

            // Actualizar total final
            venta.TotalFinal = total;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Venta realizada correctamente",
                total
            });
        }

        // =========================
        // DELETE
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenta(int id)
        {
            var venta = await _context.Ventas
                .FindAsync(id);

            if (venta == null)
            {
                return NotFound();
            }

            _context.Ventas.Remove(venta);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}