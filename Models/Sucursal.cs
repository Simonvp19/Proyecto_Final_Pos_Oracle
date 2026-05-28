using Proyecto_Final.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Proyecto_Final.Models
{
    public class Sucursal
    {
        [Key]
        public int IdSucursal { get; set; }

        [Required]
        public string NombreSucursal { get; set; }

        public string Direccion { get; set; }

        [JsonIgnore]
        public ICollection<Inventario>? Inventarios { get; set; }
    }
}