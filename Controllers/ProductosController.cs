using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Final.Data;
using Proyecto_Final.Models;
using Proyecto_Final.Data;
using Proyecto_Final.Models;

namespace Proyecto_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET TODOS LOS PRODUCTOS
        // =========================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos
                .Include(p => p.Proveedor)
                .ToListAsync();
        }

        // =========================
        // GET PRODUCTO POR ID
        // =========================
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Proveedor)
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (producto == null)
            {
                return NotFound();
            }

            return producto;
        }

        // =========================
        // POST
        // =========================
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            // Validar proveedor
            var proveedorExiste = await _context.Proveedores
                .AnyAsync(p => p.IdProveedor == producto.IdProveedor);

            if (!proveedorExiste)
            {
                return BadRequest("El proveedor no existe");
            }

            _context.Productos.Add(producto);

            await _context.SaveChangesAsync();

            return Ok(producto);
        }

        // =========================
        // PUT
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, Producto producto)
        {
            if (id != producto.IdProducto)
            {
                return BadRequest();
            }

            _context.Entry(producto).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =========================
        // DELETE
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos
                .FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            _context.Productos.Remove(producto);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}