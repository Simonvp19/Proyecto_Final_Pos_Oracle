using Proyecto_Final.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Final.Models
{
    public class DetalleVenta
    {
        [Key]
        public int IdDetalleVenta { get; set; }

        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal SubTotal { get; set; }

        // FK
        public int IdVenta { get; set; }

        public int IdProducto { get; set; }

        // Navigation Properties
        public Venta Venta { get; set; }

        public Producto Producto { get; set; }
    }
}