using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Final.Models
{
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }

        [Required]
        public string NombreProducto { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Costo { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioPieza { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioPaquete { get; set; }

        public bool Activo { get; set; }

        // FK
        public int IdProveedor { get; set; }

        // Navigation Property
        public Proveedor Proveedor { get; set; }
    }
}