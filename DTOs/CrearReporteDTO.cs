namespace Proyecto_Final.DTOs
{
    /// <summary>
    /// DTO para generar un reporte de corte de caja de una sucursal
    /// </summary>
    public class CrearReporteDTO
    {
        /// <summary>ID de la sucursal sobre la que se genera el reporte</summary>
        public int IdSucursal { get; set; }

        /// <summary>Total en efectivo declarado por el cajero al cierre de turno</summary>
        public decimal TotalEfectivoDeclarado { get; set; }

        /// <summary>Total cobrado con tarjeta según el terminal bancario</summary>
        public decimal TotalTarjeta { get; set; }
    }
}
