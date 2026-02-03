using System.ComponentModel.DataAnnotations;

namespace Document_Manager.Web.Models.AuthViewModels
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        [RegularExpression(
           @"^[a-zA-Z0-9._%-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
           ErrorMessage = "El correo contiene caracteres no permitidos"
   )]
        public string? Email { get; set; }
    }
}
