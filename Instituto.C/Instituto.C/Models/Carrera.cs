using System.Collections.Generic;
using Instituto.C.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Instituto.C.Models
{
    public class Carrera
    {
        //propiedades de la Carrera
        public int Id { get; set; }

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [StringLength(50, MinimumLength = 5, ErrorMessage = Messages.StrMaxMin)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = Messages.RestriccionLetras)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [StringLength(10, MinimumLength = 2, ErrorMessage = Messages.StrMaxMin)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = Messages.RestriccionAlfanumerica)]
        public string CodigoCarrera { get; set; }

        public List<Materia> Materias { get; set; } = new List<Materia>(); //Lista de materias que pertenecen a la carrera
        public List<Alumno> Alumnos { get; set; } = new List<Alumno>(); //Lista de alumnos que pertenecen a la carrera

    }
}
