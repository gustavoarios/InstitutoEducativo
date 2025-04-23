using System;
using System.ComponentModel.DataAnnotations;

namespace Instituto.C.Models
{
    public class Inscripcion
    {
        // Propiedades relacionales
        public int AlumnoId { get; set; }
        public int MateriaCursadaId { get; set; }

        // Propiedades Navegacionales
        public Alumno Alumno { get; set; }
        public MateriaCursada MateriaCursada { get; set; }

        [Required(ErrorMessage = "Debe indicar la fecha de inscripción")]
        [DataType(DataType.Date)]
        public DateTime FechaInscripcion { get; set; }
        [Required(ErrorMessage = "El estado de la inscripción es obligatorio")]
        public bool Activa { get; set; }

    }
}
