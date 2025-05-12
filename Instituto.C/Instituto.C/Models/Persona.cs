using System;
using System.ComponentModel.DataAnnotations;
using Instituto.C.Helpers;

namespace Instituto.C.Models
{
    public abstract class Persona
    {
        public int Id { get; set; }

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [StringLength(30,MinimumLength = 3, ErrorMessage = Messages.StrMaxMin)]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]{3,30}$", ErrorMessage = Messages.RegEx)]
        public string UserName { get; set; }

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [EmailAddress(ErrorMessage = Messages.EmaiInvalido)]
        public string Email { get; set; }
        public DateTime FechaAlta { get; set; } = DateTime.Today;

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [StringLength(100, MinimumLength = 2, ErrorMessage = Messages.StrMaxMin)]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]{2,100}$", ErrorMessage = Messages.RegEx)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [StringLength(100, MinimumLength = 2, ErrorMessage = Messages.StrMaxMin)]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]{2,100}$", ErrorMessage = Messages.RegEx)]
        public string Apellido { get; set; }

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [RegularExpression(@"^\d{7,8}$", ErrorMessage = Messages.Dni)]
        public string DNI { get; set; }

        [Phone(ErrorMessage = Messages.TelefonoInvalido)]
        public string Telefono { get; set; }

        [StringLength(100, MinimumLength = 4, ErrorMessage = Messages.StrMaxMin)]
        public string Direccion { get; set; }
        public bool Activo { get; set; }

    }
}