using System.ComponentModel.DataAnnotations;

namespace Document_Manager.Web.Models.AuthViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        [RegularExpression(
            @"^[a-zA-Z0-9._%-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "El correo contiene caracteres no permitidos"
        )]
        public string? Email { get; set; }

        [Required]
        public string Token { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(40, MinimumLength = 8,
            ErrorMessage = "The password must be at least 8 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "Password does not match.")]
        public string? ConfirmNewPassword { get; set; }
    }

}
