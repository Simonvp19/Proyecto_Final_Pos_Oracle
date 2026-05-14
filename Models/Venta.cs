using Proyecto_Final.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Final.Models
{
    public class Venta
    {
        [Key]
        public int IdVenta { get; set; }

        public DateTime Fecha { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalFinal { get; set; }

        // FK
        public int IdCliente { get; set; }

        public int IdEmpleado { get; set; }

        public int IdSucursal { get; set; }

        // Navigation Properties
        public Cliente Cliente { get; set; }

        public Empleado Empleado { get; set; }

        public Sucursal Sucursal { get; set; }

        public ICollection<DetalleVenta> DetallesVenta { get; set; }
    }
}