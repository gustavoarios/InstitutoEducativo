using System;
using System.ComponentModel.DataAnnotations;
using Instituto.C.Helpers;

namespace Instituto.C.Models
{
    public abstract class Persona
    {
        private const int CARACTERES_MAXIMOS = 8;
        private const int CARACTERES_MINIMOS = 7;
        private const string ERROR_CARACTERES_DNI = "El DNI debe tener entre 7 y 8 caracteres.";
        //hay mensajes de error en el helper para usar :)
        
        public int Id { get; set; }

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [StringLength(100, ErrorMessage = Messages.StrMaxMin)]
        [Required(ErrorMessage = ERROR_CAMPO_REQUERIDO)]
        [StringLength(100, ErrorMessage = ERROR_EXCESO_CARACTERES)] //falta el minimo
        public string UserName { get; set; }

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [StringLength(100, ErrorMessage = Messages.StrMaxMin)]
        [EmailAddress (ErrorMessage = Messages.EmaiInvalido)]

        [Required(ErrorMessage = ERROR_CAMPO_REQUERIDO)]
        [StringLength(100, ErrorMessage = ERROR_EXCESO_CARACTERES)] //EmailAdressAttribute ya tiene un techo de 254 caracteres
        [EmailAddress (ErrorMessage = ERROR_EMAIL_INVALIDO)]
        public string Email { get; set; }
        public DateTime FechaAlta { get; set; } = DateTime.Today;

        public DateTime FechaAlta { get; set; } = DateTime.Today; //cambiar a Now

        [Required (ErrorMessage = Messages.CampoObligatorio)]
        [StringLength(100, ErrorMessage = Messages.StrMaxMin)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [StringLength(100, ErrorMessage = Messages.StrMaxMin)]
        public string Apellido { get; set; }

        [Required (ErrorMessage = Messages.CampoObligatorio)]
        [StringLength(CARACTERES_MAXIMOS,MinimumLength =  CARACTERES_MINIMOS, ErrorMessage = Messages.StrMaxMin)]
        public string DNI{ get; set; }

        [Phone(ErrorMessage = Messages.TelefonoInvalido)]
        public string Telefono { get; set; }

        [StringLength(100, ErrorMessage = Messages.StrMaxMin)]
        public string Direccion { get; set; }
        public bool Activo { get; set; }

    }
}