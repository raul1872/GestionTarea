using System.ComponentModel.DataAnnotations;

namespace GestionTarea.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = ("El minimo debe ser de 8 caracteres y el maximo de 40"))]
        [Compare("ConfirmNewPassword", ErrorMessage = ("Las contraseñas no coinciden"))]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirmar la nueva contraseña es obligatorio")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nueva contraseña")]
        public string ConfirmNewPassword { get; set; }
    }
}
