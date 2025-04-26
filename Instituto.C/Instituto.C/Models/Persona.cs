using System;
using System.ComponentModel.DataAnnotations;

namespace Instituto.C.Models
{
    public abstract class Persona
    {
        private const string ERROR_CAMPO_REQUERIDO = "El {0} es obligatorio";
        private const string ERROR_EXCESO_CARACTERES = "El {0} no puede superar los 100 caracteres.";
        private const string ERROR_EMAIL_INVALIDO = "El formato del correo electronico no es valido.";
        private const string ERROR_TELEFONO_INVALIDO = "El número de telefono no es valido.";
        private const int CARACTERES_MAXIMOS = 8;
        private const int CARACTERES_MINIMOS = 7;
        private const string ERROR_CARACTERES_DNI = "El DNI debe tener entre 7 y 8 caracteres.";
        
        
        public int Id { get; set; }

        [Required(ErrorMessage = ERROR_CAMPO_REQUERIDO)]
        [StringLength(100, ErrorMessage = ERROR_EXCESO_CARACTERES)]
        public string UserName { get; set; }


        [Required(ErrorMessage = ERROR_CAMPO_REQUERIDO)]
        [StringLength(100, ErrorMessage = ERROR_EXCESO_CARACTERES)]
        [EmailAddress (ErrorMessage = ERROR_EMAIL_INVALIDO)]
        public string Email { get; set; }

        public DateTime FechaAlta { get; set; } = DateTime.Today;

        [Required (ErrorMessage = ERROR_CAMPO_REQUERIDO)]
        [StringLength(100, ErrorMessage = ERROR_EXCESO_CARACTERES)]
        public string Nombre { get; set; }


        [Required(ErrorMessage = ERROR_CAMPO_REQUERIDO)]
        [StringLength(100, ErrorMessage = ERROR_EXCESO_CARACTERES)]
        public string Apellido { get; set; }


        [Required (ErrorMessage = ERROR_CAMPO_REQUERIDO)]
        [StringLength(CARACTERES_MAXIMOS,MinimumLength =  CARACTERES_MINIMOS, ErrorMessage = ERROR_CARACTERES_DNI)]
        public string DNI{ get; set; }


        [Phone(ErrorMessage = ERROR_TELEFONO_INVALIDO)]
        public string Telefono { get; set; }


        [StringLength(100, ErrorMessage = ERROR_EXCESO_CARACTERES)]
        public string Direccion { get; set; }

        public bool Activo { get; set; }

     

    }
}