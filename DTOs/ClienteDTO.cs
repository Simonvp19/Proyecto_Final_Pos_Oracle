namespace Proyecto_Final.DTOs
{
    /// <summary>
    /// DTO para crear o actualizar un cliente
    /// </summary>
    public class ClienteDTO
    {
        /// <summary>Nombre completo del cliente</summary>
        public string NombreCliente { get; set; }

        /// <summary>Indica si el cliente tiene una cuenta pendiente de pago</summary>
        public bool CuentaPendiente { get; set; }

        /// <summary>Monto pendiente de pago del cliente</summary>
        public decimal CantidadPendiente { get; set; }
    }
}
