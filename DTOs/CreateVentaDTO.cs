namespace Proyecto_Final.DTOs
{
    public class CreateVentaDTO
    {
        public int IdCliente { get; set; }

        public int IdEmpleado { get; set; }

        public int IdSucursal { get; set; }

        public List<DetalleVentaDTO> Productos { get; set; }
    }
}
