using Microsoft.EntityFrameworkCore;
using Proyecto_Final.Models;
using Proyecto_Final.Models;
using Proyecto_Final_API.Models;

namespace Proyecto_Final.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }

        public DbSet<Proveedor> Proveedores { get; set; }

        public DbSet<Sucursal> Sucursales { get; set; }

        public DbSet<Inventario> Inventarios { get; set; }

        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Empleado> Empleados { get; set; }

        public DbSet<Venta> Ventas { get; set; }

        public DbSet<DetalleVenta> DetallesVenta { get; set; }

        public DbSet<Dia> Dias { get; set; }

        public DbSet<Visita> Visitas { get; set; }
        public DbSet<Reporte> Reportes { get; set; }

        public DbSet<Reporte_Venta> Reportes_Venta { get; set; }
    }
}