using System.Collections.Generic;
using Instituto.C.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Instituto.C.Models
{
    public class Alumno : Persona
    {

        //no incluyo la propiedad Id porque hereda de Persona

        //declaro las propiedades del Alumno

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [StringLength(20,MinimumLength =2, ErrorMessage = Messages.StrMaxMin)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = Messages.RestriccionAlfanumerica)]
        public int NumeroMatricula { get; set; }

        //propiedad navegacional
        [Required(ErrorMessage = Messages.CampoObligatorio)]
        public Carrera Carrera { get; set; }

        //propiedad relacional
        [Required(ErrorMessage = Messages.CampoObligatorio)]
        public int CarreraId { get; set; }

        public List<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>(); //Lista de materias que cursa el alumno
        public List<Calificacion> Calificaciones { get; set; } = new List<Calificacion>(); //Lista de calificaciones que tiene el alumno


    }
}
