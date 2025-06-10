using System.ComponentModel.DataAnnotations;
using Instituto.C.Helpers;

namespace Instituto.C.Models
{
    public class Registrar
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = Messages.PasswordIncorrecta)]
        public string ConfirmPassword { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        public string Direccion { get; set; }

        [Required]
        public string DNI { get; set; }

        public string Telefono { get; set; }

        [Required]
        [Display(Name = "Carrera")]
        public int CarreraId { get; set; }








    }
}
