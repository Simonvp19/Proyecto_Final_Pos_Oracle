using Microsoft.EntityFrameworkCore;
using Proyecto_Final.Data;
using Proyecto_Final.DTOs;
using Proyecto_Final.Models;

namespace Proyecto_Final.Services
{
    /// <summary>
    /// Servicio que encapsula la lógica de negocio para la gestión de productos.
    /// </summary>
    public class ProductoServices
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Constructor con inyección de dependencias del contexto de base de datos.
        /// </summary>
        public ProductoServices(AppDbContext context)
        {
            _context = context;
        }

        // ─────────────────────────────────────────────
        // CONSULTA: obtener todos los productos activos
        // ─────────────────────────────────────────────

        /// <summary>
        /// Retorna todos los productos activos con información de su proveedor.
        /// </summary>
        public async Task<List<Producto>> ObtenerProductosActivosAsync()
        {
            return await _context.Productos
                .Where(p => p.Activo)
                .Include(p => p.Proveedor)
                .ToListAsync();
        }

        // ─────────────────────────────────────────────
        // CONSULTA: obtener todos (incluyendo inactivos)
        // ─────────────────────────────────────────────

        /// <summary>
        /// Retorna todos los productos sin filtrar, útil para administración.
        /// </summary>
        public async Task<List<Producto>> ObtenerTodosAsync()
        {
            return await _context.Productos
                .Include(p => p.Proveedor)
                .ToListAsync();
        }

        // ─────────────────────────────────────────────
        // CONSULTA: buscar por ID
        // ─────────────────────────────────────────────

        /// <summary>
        /// Busca un producto por su ID. Retorna null si no existe.
        /// </summary>
        public async Task<Producto?> ObtenerPorIdAsync(int id)
        {
            return await _context.Productos
                .Include(p => p.Proveedor)
                .FirstOrDefaultAsync(p => p.IdProducto == id);
        }

        // ─────────────────────────────────────────────
        // ALTA: crear producto
        // ─────────────────────────────────────────────

        /// <summary>
        /// Crea un nuevo producto validando que el proveedor exista.
        /// </summary>
        /// <returns>El producto creado o null si el proveedor no existe.</returns>
        public async Task<Producto?> CrearAsync(ProductoDTO dto)
        {
            // Validar que el proveedor exista antes de crear el producto
            var proveedorExiste = await _context.Proveedores
                .AnyAsync(p => p.IdProveedor == dto.IdProveedor);

            if (!proveedorExiste)
                return null;

            var producto = new Producto
            {
                NombreProducto = dto.NombreProducto,
                Costo = dto.Costo,
                PrecioUnitario = dto.PrecioUnitario,
                PrecioPieza = dto.PrecioPieza,
                PrecioPaquete = dto.PrecioPaquete,
                Activo = dto.Activo,
                IdProveedor = dto.IdProveedor
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return producto;
        }

        // ─────────────────────────────────────────────
        // ACTUALIZACIÓN: editar producto
        // ─────────────────────────────────────────────

        /// <summary>
        /// Actualiza un producto existente. Retorna false si no se encuentra.
        /// </summary>
        public async Task<bool> ActualizarAsync(int id, ProductoDTO dto)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return false;

            // Actualizar sólo los campos del DTO
            producto.NombreProducto = dto.NombreProducto;
            producto.Costo = dto.Costo;
            producto.PrecioUnitario = dto.PrecioUnitario;
            producto.PrecioPieza = dto.PrecioPieza;
            producto.PrecioPaquete = dto.PrecioPaquete;
            producto.Activo = dto.Activo;
            producto.IdProveedor = dto.IdProveedor;

            await _context.SaveChangesAsync();

            return true;
        }

        // ─────────────────────────────────────────────
        // BAJA: eliminar producto (baja lógica)
        // ─────────────────────────────────────────────

        /// <summary>
        /// Realiza una baja lógica del producto marcándolo como inactivo.
        /// Retorna false si el producto no existe.
        /// </summary>
        public async Task<bool> DesactivarAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return false;

            // Baja lógica: se conserva en BD pero ya no aparece activo
            producto.Activo = false;

            await _context.SaveChangesAsync();

            return true;
        }

        // ─────────────────────────────────────────────
        // BAJA: eliminar físico (solo si no tiene ventas)
        // ─────────────────────────────────────────────

        /// <summary>
        /// Elimina físicamente un producto solo si no tiene detalles de venta asociados.
        /// </summary>
        public async Task<(bool exito, string mensaje)> EliminarAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return (false, "Producto no encontrado.");

            // Verificar que no tenga ventas registradas
            var tieneVentas = await _context.DetallesVenta
                .AnyAsync(d => d.IdProducto == id);

            if (tieneVentas)
                return (false, "No se puede eliminar: el producto tiene ventas registradas. Use la baja lógica.");

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return (true, "Producto eliminado correctamente.");
        }
    }
}
