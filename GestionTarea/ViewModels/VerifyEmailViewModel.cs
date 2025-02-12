using System.ComponentModel.DataAnnotations;

namespace GestionTarea.ViewModels
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
