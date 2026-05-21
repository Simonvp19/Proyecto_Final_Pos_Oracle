using Proyecto_Final.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Final.Models
{
    public class DetalleVenta
    {
        public int IdVenta { get; set; }

        public int IdProducto { get; set; }

        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal SubTotal { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(IdVenta))]
        public Venta Venta { get; set; }

        [ForeignKey(nameof(IdProducto))]
        public Producto Producto { get; set; }
    }
}