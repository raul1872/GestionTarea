using System.ComponentModel.DataAnnotations;
using GestionTarea.Models;

namespace GestionTarea.Models
{
    public class Tarea
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Titulo { get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaVencimiento { get; set; }
        public bool Completada { get; set; } = false;

        public int Id_Usuario { get; set; }
        public Usuario Usuario { get; set; }
    }
}

