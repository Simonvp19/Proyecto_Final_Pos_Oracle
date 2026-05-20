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
    public class InventarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InventarioController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET TODO EL INVENTARIO
        // =========================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inventario>>> GetInventario()
        {
            return await _context.Inventarios
                .Include(i => i.Producto)
                .Include(i => i.Sucursal)
                .ToListAsync();
        }

        // =========================
        // GET INVENTARIO POR ID
        // =========================
        [HttpGet("{id}")]
        public async Task<ActionResult<Inventario>> GetInventario(int id)
        {
            var inventario = await _context.Inventarios
                .Include(i => i.Producto)
                .Include(i => i.Sucursal)
                .FirstOrDefaultAsync(i => i.IdInventario == id);

            if (inventario == null)
            {
                return NotFound();
            }

            return inventario;
        }

        // =========================
        // POST
        // =========================
        [HttpPost]
        public async Task<ActionResult<Inventario>> PostInventario(Inventario inventario)
        {
            // Validar producto
            var productoExiste = await _context.Productos
                .AnyAsync(p => p.IdProducto == inventario.IdProducto);

            if (!productoExiste)
            {
                return BadRequest("El producto no existe");
            }

            // Validar sucursal
            var sucursalExiste = await _context.Sucursales
                .AnyAsync(s => s.IdSucursal == inventario.IdSucursal);

            if (!sucursalExiste)
            {
                return BadRequest("La sucursal no existe");
            }

            _context.Inventarios.Add(inventario);

            await _context.SaveChangesAsync();

            return Ok(inventario);
        }

        // =========================
        // PUT
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInventario(int id, Inventario inventario)
        {
            if (id != inventario.IdInventario)
            {
                return BadRequest();
            }

            _context.Entry(inventario).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =========================
        // DELETE
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventario(int id)
        {
            var inventario = await _context.Inventarios
                .FindAsync(id);

            if (inventario == null)
            {
                return NotFound();
            }

            _context.Inventarios.Remove(inventario);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}