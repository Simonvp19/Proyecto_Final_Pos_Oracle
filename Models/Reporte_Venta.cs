    using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [ForeignKey(nameof(IdVenta))]
        public Venta Venta { get; set; }

        [ForeignKey(nameof(IdReporte))]
        public Reporte Reporte { get; set; }
    }
    }
