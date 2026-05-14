using Proyecto_Final.Models;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Final.Models
{
    public class Inventario
    {
        [Key]
        public int IdInventario { get; set; }

        public int Stock { get; set; }

        // FK
        public int IdProducto { get; set; }

        public int IdSucursal { get; set; }

        // Navigation Properties
        public Producto Producto { get; set; }

        public Sucursal Sucursal { get; set; }
    }
}