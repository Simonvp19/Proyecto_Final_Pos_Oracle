using Proyecto_Final.Models;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Final.Models
{
    public class Sucursal
    {
        [Key]
        public int IdSucursal { get; set; }

        [Required]
        public string NombreSucursal { get; set; }

        public string Direccion { get; set; }

        public ICollection<Inventario> Inventarios { get; set; }
    }
}