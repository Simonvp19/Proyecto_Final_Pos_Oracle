using Microsoft.EntityFrameworkCore;
using Proyecto_Final.Models;
using Proyecto_Final_API.Models;

namespace Proyecto_Final.Data
{
    /// <summary>
    /// Contexto principal de Entity Framework Core
    /// Define las tablas de la base de datos y sus relaciones
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Constructor que recibe las opciones de configuración (inyección de dependencias)
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // ─────────────────────────────────────────────
        // TABLAS — DbSets (una propiedad por tabla)
        // ─────────────────────────────────────────────

        /// <summary>Tabla de productos del catálogo</summary>
        public DbSet<Producto> Productos { get; set; }

        /// <summary>Tabla de proveedores que surten los productos</summary>
        public DbSet<Proveedor> Proveedores { get; set; }

        /// <summary>Tabla de sucursales del negocio</summary>
        public DbSet<Sucursal> Sucursales { get; set; }

        /// <summary>Tabla de inventario (stock por producto y sucursal)</summary>
        public DbSet<Inventario> Inventarios { get; set; }

        /// <summary>Tabla de clientes del negocio</summary>
        public DbSet<Cliente> Clientes { get; set; }

        /// <summary>Tabla de empleados con sus roles</summary>
        public DbSet<Empleado> Empleados { get; set; }

        /// <summary>Tabla de ventas registradas</summary>
        public DbSet<Venta> Ventas { get; set; }

        /// <summary>Tabla de detalle de cada venta (productos vendidos)</summary>
        public DbSet<DetalleVenta> DetallesVenta { get; set; }

        /// <summary>Tabla de días de la semana (para visitas de proveedores)</summary>
        public DbSet<Dia> Dias { get; set; }

        /// <summary>Tabla de visitas programadas de proveedores</summary>
        public DbSet<Visita> Visitas { get; set; }

        /// <summary>Tabla de reportes de corte de caja</summary>
        public DbSet<Reporte> Reportes { get; set; }

        /// <summary>Tabla de relación entre reportes y ventas</summary>
        public DbSet<Reporte_Venta> Reportes_Venta { get; set; }

        // ─────────────────────────────────────────────
        // CONFIGURACIÓN DE RELACIONES
        // ─────────────────────────────────────────────

        /// <summary>
        /// Configura las llaves compuestas y relaciones especiales del modelo
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Llave primaria compuesta: DetalleVenta (IdVenta + IdProducto)
            modelBuilder.Entity<DetalleVenta>()
                .HasKey(dv => new { dv.IdVenta, dv.IdProducto });

            // Llave primaria compuesta: Inventario (IdProducto + IdSucursal)
            modelBuilder.Entity<Inventario>()
                .HasKey(i => new { i.IdProducto, i.IdSucursal });

            // Relación Venta → DetalleVenta (una venta tiene muchos detalles)
            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Venta)
                .WithMany(v => v.DetallesVenta)
                .HasForeignKey(d => d.IdVenta)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación Reporte_Venta → evitar múltiples cascadas en SQL Server
            modelBuilder.Entity<Reporte_Venta>()
                .HasOne(rv => rv.Venta)
                .WithMany()
                .HasForeignKey(rv => rv.IdVenta)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Reporte_Venta>()
                .HasOne(rv => rv.Reporte)
                .WithMany(r => r.Reportes_Venta)
                .HasForeignKey(rv => rv.IdReporte)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
