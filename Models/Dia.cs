using System.ComponentModel.DataAnnotations;

namespace Proyecto_Final_API.Models
{
    public class Dia
    {
        [Key]
        public int IdDia { get; set; }

        public string NombreDia { get; set; }

        public ICollection<Visita> Visitas { get; set; }
    }
}