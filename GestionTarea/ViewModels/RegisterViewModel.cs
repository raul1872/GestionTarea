using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GestionTarea.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El Nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = ("El minimo debe ser de 8 caracteres y el maximo de 40"))]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = ("Las contraseñas no coinciden"))]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmar la contraseña es obligatorio")]
        [DataType(DataType.Password)]
        [Display(Name= "Confirmar contraseña")]
        public string ConfirmPassword { get; set; }
    }
}
