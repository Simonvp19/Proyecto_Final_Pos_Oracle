using Proyecto_Final.Models;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Final_API.Models
{
    public class Visita
    {
        [Key]
        public int IdVisita { get; set; }

        // FK
        public int IdProveedor { get; set; }

        public int IdDia { get; set; }

        // Navigation Properties
        public Proveedor Proveedor { get; set; }

        public Dia Dia { get; set; }
    }
}
