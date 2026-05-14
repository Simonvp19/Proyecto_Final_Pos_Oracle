using Proyecto_Final.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Final.Models
{
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }

        public string NombreCliente { get; set; }

        
       public bool CuentaPendiente { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal CantidadPendiente { get; set; }

        public ICollection<Venta> Ventas { get; set; }
    }
}