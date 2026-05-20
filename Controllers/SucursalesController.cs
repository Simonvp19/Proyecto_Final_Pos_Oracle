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
    public class SucursalesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SucursalesController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET TODAS LAS SUCURSALES
        // =========================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sucursal>>> GetSucursales()
        {
            return await _context.Sucursales.ToListAsync();
        }

        // =========================
        // GET SUCURSAL POR ID
        // =========================
        [HttpGet("{id}")]
        public async Task<ActionResult<Sucursal>> GetSucursal(int id)
        {
            var sucursal = await _context.Sucursales
                .FindAsync(id);

            if (sucursal == null)
            {
                return NotFound();
            }

            return sucursal;
        }

        // =========================
        // POST
        // =========================
        [HttpPost]
        public async Task<ActionResult<Sucursal>> PostSucursal(Sucursal sucursal)
        {
            _context.Sucursales.Add(sucursal);

            await _context.SaveChangesAsync();

            return Ok(sucursal);
        }

        // =========================
        // PUT
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSucursal(int id, Sucursal sucursal)
        {
            if (id != sucursal.IdSucursal)
            {
                return BadRequest();
            }

            _context.Entry(sucursal).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =========================
        // DELETE
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSucursal(int id)
        {
            var sucursal = await _context.Sucursales
                .FindAsync(id);

            if (sucursal == null)
            {
                return NotFound();
            }

            _context.Sucursales.Remove(sucursal);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
