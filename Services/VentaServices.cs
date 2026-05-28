using Microsoft.EntityFrameworkCore;
using Proyecto_Final.Data;
using Proyecto_Final.DTOs;
using Proyecto_Final.Models;

namespace Proyecto_Final.Services
{
    /// <summary>
    /// Servicio que encapsula la lógica de negocio de ventas:
    /// registro de ventas, consulta de historial y cálculo de totales por turno
    /// </summary>
    public class VentaServices
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Constructor con inyección de dependencias del contexto de base de datos
        /// </summary>
        public VentaServices(AppDbContext context)
        {
            _context = context;
        }

        // ─────────────────────────────────────────────
        // CONSULTA: historial completo de ventas
        // ─────────────────────────────────────────────

        /// <summary>
        /// Retorna todas las ventas registradas con sus detalles, empleado,
        /// cliente y sucursal relacionados
        /// </summary>
        public async Task<List<VentaDTO>> ObtenerTodasAsync()
        {
            var ventas = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Empleado)
                .Include(v => v.Sucursal)
                .Include(v => v.DetallesVenta)
                    .ThenInclude(d => d.Producto)
                .ToListAsync();

            return ventas.Select(MapearVentaDTO).ToList();
        }

        // ─────────────────────────────────────────────
        // CONSULTA: historial de ventas por sucursal
        // ─────────────────────────────────────────────

        /// <summary>
        /// Retorna el historial de ventas de una sucursal específica,
        /// opcionalmente filtrado por rango de fechas
        /// </summary>
        public async Task<List<VentaDTO>> ObtenerPorSucursalAsync(
            int idSucursal,
            DateTime? desde = null,
            DateTime? hasta = null)
        {
            var query = _context.Ventas
                .Where(v => v.IdSucursal == idSucursal)
                .Include(v => v.Cliente)
                .Include(v => v.Empleado)
                .Include(v => v.Sucursal)
                .Include(v => v.DetallesVenta)
                    .ThenInclude(d => d.Producto)
                .AsQueryable();

            if (desde.HasValue)
                query = query.Where(v => v.Fecha >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(v => v.Fecha <= hasta.Value);

            var ventas = await query.OrderByDescending(v => v.Fecha).ToListAsync();

            return ventas.Select(MapearVentaDTO).ToList();
        }

        // ─────────────────────────────────────────────
        // CONSULTA: venta por ID
        // ─────────────────────────────────────────────

        /// <summary>
        /// Retorna el detalle de una venta específica. Retorna null si no existe
        /// </summary>
        public async Task<VentaDTO?> ObtenerPorIdAsync(int idVenta)
        {
            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Empleado)
                .Include(v => v.Sucursal)
                .Include(v => v.DetallesVenta)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.IdVenta == idVenta);

            return venta == null ? null : MapearVentaDTO(venta);
        }

        // ─────────────────────────────────────────────
        // CONSULTA: total de ventas (corte de caja)
        // ─────────────────────────────────────────────

        /// <summary>
        /// Calcula el total de ventas de una sucursal en el día actual
        /// Se usa para el corte de caja al finalizar el turno
        /// </summary>
        public async Task<decimal> ObtenerTotalDiaAsync(int idSucursal)
        {
            var hoy = DateTime.Today;

            return await _context.Ventas
                .Where(v => v.IdSucursal == idSucursal &&
                            v.Fecha.Date == hoy)
                .SumAsync(v => v.TotalFinal);
        }

        // ─────────────────────────────────────────────
        // ALTA: registrar nueva venta
        // ─────────────────────────────────────────────

        /// <summary>
        /// Registra una nueva venta descontando el inventario de cada producto
        /// Valida stock disponible antes de procesar
        /// </summary>
        public async Task<(bool exito, string mensaje, object? resultado)>
            RegistrarVentaAsync(CreateVentaDTO dto)
        {
            // Validar que el cliente exista
            var clienteExiste = await _context.Clientes
                .AnyAsync(c => c.IdCliente == dto.IdCliente);
            if (!clienteExiste)
                return (false, "Cliente no encontrado.", null);

            // Validar que el empleado exista
            var empleadoExiste = await _context.Empleados
                .AnyAsync(e => e.IdEmpleado == dto.IdEmpleado);
            if (!empleadoExiste)
                return (false, "Empleado no encontrado.", null);

            // Validar que la sucursal exista
            var sucursalExiste = await _context.Sucursales
                .AnyAsync(s => s.IdSucursal == dto.IdSucursal);
            if (!sucursalExiste)
                return (false, "Sucursal no encontrada.", null);

            if (dto.Productos == null || dto.Productos.Count == 0)
                return (false, "La venta debe contener al menos un producto.", null);

            // Crear encabezado de venta
            var venta = new Venta
            {
                Fecha = DateTime.Now,
                IdCliente = dto.IdCliente,
                IdEmpleado = dto.IdEmpleado,
                IdSucursal = dto.IdSucursal,
                TotalFinal = 0
            };

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync(); // Necesario para obtener IdVenta

            decimal totalFinal = 0;

            foreach (var item in dto.Productos)
            {
                var producto = await _context.Productos
                    .FirstOrDefaultAsync(p => p.IdProducto == item.IdProducto && p.Activo);

                if (producto == null)
                    return (false, $"Producto ID {item.IdProducto} no encontrado o inactivo.", null);

                var inventario = await _context.Inventarios
                    .FirstOrDefaultAsync(i =>
                        i.IdProducto == item.IdProducto &&
                        i.IdSucursal == dto.IdSucursal);

                if (inventario == null)
                    return (false, $"Sin inventario para '{producto.NombreProducto}' en esta sucursal.", null);

                if (inventario.Stock < item.Cantidad)
                    return (false,
                        $"Stock insuficiente para '{producto.NombreProducto}'. " +
                        $"Disponible: {inventario.Stock}, solicitado: {item.Cantidad}.", null);

                decimal subtotal = producto.PrecioUnitario * item.Cantidad;
                totalFinal += subtotal;

                _context.DetallesVenta.Add(new DetalleVenta
                {
                    IdVenta = venta.IdVenta,
                    IdProducto = item.IdProducto,
                    Cantidad = item.Cantidad,
                    SubTotal = subtotal
                });

                // Descontar del inventario
                inventario.Stock -= item.Cantidad;
            }

            venta.TotalFinal = totalFinal;
            await _context.SaveChangesAsync();

            return (true, "Venta registrada correctamente.", new
            {
                idVenta = venta.IdVenta,
                fecha = venta.Fecha,
                total = totalFinal
            });
        }

        // ─────────────────────────────────────────────
        // Helper: mapear Venta → VentaDTO
        // ─────────────────────────────────────────────

        /// <summary>
        /// Convierte una entidad Venta a su DTO de respuesta
        /// </summary>
        private static VentaDTO MapearVentaDTO(Venta v) => new VentaDTO
        {
            IdVenta = v.IdVenta,
            Fecha = v.Fecha,
            TotalFinal = v.TotalFinal,
            NombreCliente = v.Cliente?.NombreCliente ?? "N/A",
            NombreEmpleado = v.Empleado?.NombreEmpleado ?? "N/A",
            NombreSucursal = v.Sucursal?.NombreSucursal ?? "N/A",
            Detalles = v.DetallesVenta?.Select(d => new DetalleVentaRespuestaDTO
            {
                NombreProducto = d.Producto?.NombreProducto ?? "N/A",
                Cantidad = d.Cantidad,
                SubTotal = d.SubTotal
            }).ToList() ?? new()
        };
    }
}
