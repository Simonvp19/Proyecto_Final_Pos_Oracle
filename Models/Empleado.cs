using Proyecto_Final.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Proyecto_Final.Models
{
    public class Empleado
    {
        [Key]
        public int IdEmpleado { get; set; }

        public string NombreEmpleado { get; set; }

        public string PasswordHash { get; set; }

        public string Rol { get; set; }

        [JsonIgnore]
        public ICollection<Venta>? Ventas { get; set; }
    }
}