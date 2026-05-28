using Proyecto_Final.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Proyecto_Final.Models
{
    public class Proveedor
    {
        [Key]
        public int IdProveedor { get; set; }

        [Required]
        public string NombreProveedor { get; set; }

        // Navigation Property
        [JsonIgnore]
        public ICollection<Producto>? Productos { get; set; }
    }
}
