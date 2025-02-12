using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GestionTarea.Models
{
    public class Usuario : IdentityUser
    {
        public int Id_Usuario { get; set; }

        public string Nombre { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public ICollection<Tarea> Tarea { get; set; }

    }
}
