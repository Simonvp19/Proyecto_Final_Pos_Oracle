using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Final.Models
{
    public class Reporte
    {
        [Key]
        public int IdReporte { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalVentaEfectivo { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalVentaTarjeta { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalEstimadoVenta { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalRealVenta { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Descuadre { get; set; }

        public DateTime Fecha { get; set; }

        public ICollection<Reporte_Venta>? Reportes_Venta { get; set; }
    }
}

