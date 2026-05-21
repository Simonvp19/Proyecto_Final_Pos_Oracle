using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // =========================================
        // GET: api/Inventario
        // =========================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inventario>>> GetInventarios()
        {
            return await _context.Inventarios
                .Include(i => i.Producto)
                .Include(i => i.Sucursal)
                .ToListAsync();
        }

        // =========================================
        // GET: api/Inventario/1/1
        // =========================================
        [HttpGet("{idProducto}/{idSucursal}")]
        public async Task<ActionResult<Inventario>> GetInventario(
            int idProducto,
            int idSucursal)
        {
            var inventario = await _context.Inventarios
                .Include(i => i.Producto)
                .Include(i => i.Sucursal)
                .FirstOrDefaultAsync(i =>
                    i.IdProducto == idProducto &&
                    i.IdSucursal == idSucursal);

            if (inventario == null)
            {
                return NotFound();
            }

            return inventario;
        }

        // =========================================
        // POST: api/Inventario
        // =========================================
        [HttpPost]
        public async Task<ActionResult<Inventario>> PostInventario(
            Inventario inventario)
        {
            _context.Inventarios.Add(inventario);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetInventario),
                new
                {
                    idProducto = inventario.IdProducto,
                    idSucursal = inventario.IdSucursal
                },
                inventario);
        }

        // =========================================
        // PUT: api/Inventario/1/1
        // =========================================
        [HttpPut("{idProducto}/{idSucursal}")]
        public async Task<IActionResult> PutInventario(
            int idProducto,
            int idSucursal,
            Inventario inventario)
        {
            if (
                idProducto != inventario.IdProducto ||
                idSucursal != inventario.IdSucursal
            )
            {
                return BadRequest();
            }

            _context.Entry(inventario).State =
                EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var existe = await _context.Inventarios
                    .AnyAsync(i =>
                        i.IdProducto == idProducto &&
                        i.IdSucursal == idSucursal);

                if (!existe)
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        // =========================================
        // DELETE: api/Inventario/1/1
        // =========================================
        [HttpDelete("{idProducto}/{idSucursal}")]
        public async Task<IActionResult> DeleteInventario(
            int idProducto,
            int idSucursal)
        {
            var inventario = await _context.Inventarios
                .FirstOrDefaultAsync(i =>
                    i.IdProducto == idProducto &&
                    i.IdSucursal == idSucursal);

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