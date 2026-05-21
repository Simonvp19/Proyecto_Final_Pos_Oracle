using Proyecto_Final.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [ForeignKey(nameof(IdProveedor))]
        public Proveedor Proveedor { get; set; }

        [ForeignKey(nameof(IdDia))]
        public Dia Dia { get; set; }
    }
}
