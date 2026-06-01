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
        // CONFIGURACIÓN DE RELACIONES Y MAPEO ORACLE
        // ─────────────────────────────────────────────

        /// <summary>
        /// Configura las llaves compuestas, relaciones especiales y el mapeo a tablas de Oracle
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Mapeo explícito a nombres de tablas en Oracle (Garantiza coincidencia exacta con tus scripts)
            modelBuilder.Entity<Sucursal>().ToTable("SUCURSAL");
            modelBuilder.Entity<Empleado>().ToTable("EMPLEADO");
            modelBuilder.Entity<Cliente>().ToTable("CLIENTE");
            modelBuilder.Entity<Proveedor>().ToTable("PROVEEDOR");
            modelBuilder.Entity<Producto>().ToTable("PRODUCTO");
            modelBuilder.Entity<Inventario>().ToTable("INVENTARIO");
            modelBuilder.Entity<Venta>().ToTable("VENTA");
            modelBuilder.Entity<DetalleVenta>().ToTable("DETALLE_VENTA");
            modelBuilder.Entity<Reporte>().ToTable("REPORTE");

            // Si agregas tablas extras que no estaban en los scripts iniciales, mantén su mapeo en mayúsculas:
            modelBuilder.Entity<Dia>().ToTable("DIA");
            modelBuilder.Entity<Visita>().ToTable("VISITA");
            modelBuilder.Entity<Reporte_Venta>().ToTable("REPORTE_VENTA");

            // 2. Llaves primarias compuestas
            // Llave primaria compuesta: DetalleVenta (IdVenta + IdProducto)
            modelBuilder.Entity<DetalleVenta>()
                .HasKey(dv => new { dv.IdVenta, dv.IdProducto });

            // Llave primaria compuesta: Inventario (IdProducto + IdSucursal)
            modelBuilder.Entity<Inventario>()
                .HasKey(i => new { i.IdProducto, i.IdSucursal });

            // 3. Configuración de Relaciones y Llaves Foráneas (Foreign Keys)
            // Relación Venta → DetalleVenta
            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Venta)
                .WithMany(v => v.DetallesVenta)
                .HasForeignKey(d => d.IdVenta)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación Reporte_Venta → Venta
            // NOTA: Oracle maneja los esquemas de cascada diferente a SQL Server, 
            // pero mantener "NoAction" o "Restrict" aquí es totalmente seguro.
            modelBuilder.Entity<Reporte_Venta>()
                .HasOne(rv => rv.Venta)
                .WithMany()
                .HasForeignKey(rv => rv.IdVenta)
                .OnDelete(DeleteBehavior.NoAction);

            // Relación Reporte_Venta → Reporte
            modelBuilder.Entity<Reporte_Venta>()
                .HasOne(rv => rv.Reporte)
                .WithMany(r => r.Reportes_Venta)
                .HasForeignKey(rv => rv.IdReporte)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}