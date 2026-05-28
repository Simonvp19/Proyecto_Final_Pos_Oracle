namespace Proyecto_Final.DTOs
{
    /// <summary>
    /// DTO de respuesta con el resumen de una venta registrada
    /// </summary>
    public class VentaDTO
    {
        /// <summary>ID único de la venta</summary>
        public int IdVenta { get; set; }

        /// <summary>Fecha y hora en que se realizó la venta</summary>
        public DateTime Fecha { get; set; }

        /// <summary>Total final cobrado al cliente</summary>
        public decimal TotalFinal { get; set; }

        /// <summary>Nombre del cliente.</summary>
        public string NombreCliente { get; set; }

        /// <summary>Nombre del empleado que atendio</summary>
        public string NombreEmpleado { get; set; }

        /// <summary>Nombre de la sucursal donde se realizó la venta</summary>
        public string NombreSucursal { get; set; }

        /// <summary>Lista de productos vendidos con cantidades y subtotales</summary>
        public List<DetalleVentaRespuestaDTO> Detalles { get; set; }
    }

    /// <summary>
    /// Detalle de un producto dentro de la respuesta de venta
    /// </summary>
    public class DetalleVentaRespuestaDTO
    {
        /// <summary>Nombre del producto vendido</summary>
        public string NombreProducto { get; set; }

        /// <summary>Cantidad de unidades vendidas</summary>
        public int Cantidad { get; set; }

        /// <summary>Subtotal de este producto en la venta</summary>
        public decimal SubTotal { get; set; }
    }
}
