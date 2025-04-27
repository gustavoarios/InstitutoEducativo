using System;
using System.ComponentModel.DataAnnotations;
using Instituto.C.Helpers;

namespace Instituto.C.Models
{
    public abstract class Persona
    {
        private const int CARACTERES_MAXIMOS = 8;
        private const int CARACTERES_MINIMOS = 7;
        
        public int Id { get; set; }

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [StringLength(100, ErrorMessage = Messages.StrMaxMin)]
        public string UserName { get; set; }

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [StringLength(100, ErrorMessage = Messages.StrMaxMin)]
        [EmailAddress (ErrorMessage = Messages.EmaiInvalido)]
        public string Email { get; set; }
        public DateTime FechaAlta { get; set; } = DateTime.Today;

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