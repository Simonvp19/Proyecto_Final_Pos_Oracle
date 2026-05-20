using System.ComponentModel.DataAnnotations;

namespace Proyecto_Final.Models
{
    public class Reporte_Venta
    {
        [Key]
        public int IdReporteVenta { get; set; }

        // FK
        public int IdVenta { get; set; }

        public int IdReporte { get; set; }

        // Navigation Properties
        public Venta Venta { get; set; }

        public Reporte Reporte { get; set; }
    }
}
