using Proyecto_Final.Models;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Final.Models
{
    public class Proveedor
    {
        [Key]
        public int IdProveedor { get; set; }

        [Required]
        public string NombreProveedor { get; set; }

        // Navigation Property
        public ICollection<Producto> Productos { get; set; }
    }
}
