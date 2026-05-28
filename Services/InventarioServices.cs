using Microsoft.EntityFrameworkCore;
using Proyecto_Final.Data;
using Proyecto_Final.Models;

namespace Proyecto_Final.Services
{
    /// <summary>
    /// Servicio que encapsula la lógica de negocio para la gestión de inventario
    /// Permite consultar stock, registrar entradas y salidas manuales de productos
    /// </summary>
    public class InventarioServices
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Constructor con inyección de dependencias del contexto de base de datos
        /// </summary>
        public InventarioServices(AppDbContext context)
        {
            _context = context;
        }

        // ─────────────────────────────────────────────
        // CONSULTA: inventario completo de una sucursal
        // ─────────────────────────────────────────────

        /// <summary>
        /// Retorna todos los registros de inventario de una sucursal específica
        /// con el detalle de cada producto
        /// </summary>
        public async Task<List<Inventario>> ObtenerPorSucursalAsync(int idSucursal)
        {
            return await _context.Inventarios
                .Where(i => i.IdSucursal == idSucursal)
                .Include(i => i.Producto)
                .Include(i => i.Sucursal)
                .ToListAsync();
        }

        // ─────────────────────────────────────────────
        // CONSULTA: stock de un producto en una sucursal
        // ─────────────────────────────────────────────

        /// <summary>
        /// Retorna el registro de inventario de un producto en una sucursal
        /// Retorna null si no existe el registro
        /// </summary>
        public async Task<Inventario?> ObtenerStockAsync(int idProducto, int idSucursal)
        {
            return await _context.Inventarios
                .Include(i => i.Producto)
                .Include(i => i.Sucursal)
                .FirstOrDefaultAsync(i =>
                    i.IdProducto == idProducto &&
                    i.IdSucursal == idSucursal);
        }

        // ─────────────────────────────────────────────
        // CONSULTA: productos con stock bajo
        // ─────────────────────────────────────────────

        /// <summary>
        /// Retorna los productos cuyo stock en una sucursal es menor al mínimo indicado
        /// Útil para generar alertas de reabastecimiento
        /// </summary>
        public async Task<List<Inventario>> ObtenerStockBajoAsync(int idSucursal, int minimoStock = 5)
        {
            return await _context.Inventarios
                .Where(i => i.IdSucursal == idSucursal && i.Stock <= minimoStock)
                .Include(i => i.Producto)
                .ToListAsync();
        }

        // ─────────────────────────────────────────────
        // ALTA: registrar entrada de producto
        // ─────────────────────────────────────────────

        /// <summary>
        /// Registra una entrada de stock para un producto en una sucursal
        /// Si ya existe el registro de inventario, suma la cantidad al stock actual
        /// Si no existe, crea el registro nuevo
        /// </summary>
        public async Task<(bool exito, string mensaje, Inventario? inventario)>
            RegistrarEntradaAsync(int idProducto, int idSucursal, int cantidad)
        {
            if (cantidad <= 0)
                return (false, "La cantidad de entrada debe ser mayor a cero.", null);

            // Verificar que el producto exista y esté activo
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.IdProducto == idProducto && p.Activo);

            if (producto == null)
                return (false, "Producto no encontrado o inactivo.", null);

            // Verificar que la sucursal exista
            var sucursalExiste = await _context.Sucursales
                .AnyAsync(s => s.IdSucursal == idSucursal);

            if (!sucursalExiste)
                return (false, "Sucursal no encontrada.", null);

            var inventario = await _context.Inventarios
                .FirstOrDefaultAsync(i =>
                    i.IdProducto == idProducto &&
                    i.IdSucursal == idSucursal);

            if (inventario == null)
            {
                // Crear nuevo registro de inventario
                inventario = new Inventario
                {
                    IdProducto = idProducto,
                    IdSucursal = idSucursal,
                    Stock = cantidad
                };
                _context.Inventarios.Add(inventario);
            }
            else
            {
                // Sumar al stock existente
                inventario.Stock += cantidad;
            }

            await _context.SaveChangesAsync();

            return (true, $"Entrada registrada. Stock actual: {inventario.Stock}", inventario);
        }

        // ─────────────────────────────────────────────
        // BAJA: registrar salida manual de producto
        // ─────────────────────────────────────────────

        /// <summary>
        /// Registra una salida manual de stock (merma, ajuste, etc.)
        /// Valida que haya suficiente stock antes de descontar
        /// </summary>
        public async Task<(bool exito, string mensaje, Inventario? inventario)>
            RegistrarSalidaAsync(int idProducto, int idSucursal, int cantidad)
        {
            if (cantidad <= 0)
                return (false, "La cantidad de salida debe ser mayor a cero.", null);

            var inventario = await _context.Inventarios
                .FirstOrDefaultAsync(i =>
                    i.IdProducto == idProducto &&
                    i.IdSucursal == idSucursal);

            if (inventario == null)
                return (false, "No existe registro de inventario para ese producto en esta sucursal.", null);

            if (inventario.Stock < cantidad)
                return (false, $"Stock insuficiente. Stock actual: {inventario.Stock}", null);

            inventario.Stock -= cantidad;

            await _context.SaveChangesAsync();

            return (true, $"Salida registrada. Stock actual: {inventario.Stock}", inventario);
        }

        // ─────────────────────────────────────────────
        // ACTUALIZACIÓN: ajuste directo de stock
        // ─────────────────────────────────────────────

        /// <summary>
        /// Ajusta directamente el stock de un producto en una sucursal
        /// Útil para correcciones de inventario físico
        /// </summary>
        public async Task<(bool exito, string mensaje)>
            AjustarStockAsync(int idProducto, int idSucursal, int nuevoStock)
        {
            if (nuevoStock < 0)
                return (false, "El stock no puede ser negativo.");

            var inventario = await _context.Inventarios
                .FirstOrDefaultAsync(i =>
                    i.IdProducto == idProducto &&
                    i.IdSucursal == idSucursal);

            if (inventario == null)
                return (false, "No existe registro de inventario para ese producto en esta sucursal.");

            inventario.Stock = nuevoStock;

            await _context.SaveChangesAsync();

            return (true, $"Stock ajustado correctamente a {nuevoStock} unidades.");
        }
    }
}
