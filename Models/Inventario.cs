using Proyecto_Final.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Final.Models
{
    public class Inventario
    {
        public int IdProducto { get; set; }

        public int IdSucursal { get; set; }

        public int Stock { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(IdProducto))]
        public Producto Producto { get; set; }

        [ForeignKey(nameof(IdSucursal))]
        public Sucursal Sucursal { get; set; }
    }
}