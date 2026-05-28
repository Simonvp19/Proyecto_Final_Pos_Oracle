namespace Proyecto_Final.DTOs
{
    /// <summary>
    /// DTO para crear o actualizar un producto
    /// Evita exponer la navigation property del proveedor
    /// </summary>
    public class ProductoDTO
    {
        /// <summary>Nombre descriptivo del producto</summary>
        public string NombreProducto { get; set; }

        /// <summary>Costo de adquisicion del producto</summary>
        public decimal Costo { get; set; }

        /// <summary>Precio de venta unitario</summary>
        public decimal PrecioUnitario { get; set; }

        /// <summary>Precio de venta por pieza (presentación menor)</summary>
        public decimal PrecioPieza { get; set; }

        /// <summary>Precio de venta por paquete (presentación mayor)</summary>
        public decimal PrecioPaquete { get; set; }

        /// <summary>Indica si el producto está activo en el sistema</summary>
        public bool Activo { get; set; }

        /// <summary>ID del proveedor que surte este producto</summary>
        public int IdProveedor { get; set; }
    }
}
