using Proyecto_Final.Models;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Final.Models
{
    public class Empleado
    {
        [Key]
        public int IdEmpleado { get; set; }

        public string NombreEmpleado { get; set; }

        public string PasswordHash { get; set; }

        public string Rol { get; set; }

        public ICollection<Venta> Ventas { get; set; }
    }
}