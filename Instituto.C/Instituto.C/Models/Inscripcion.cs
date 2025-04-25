using System;
using System.ComponentModel.DataAnnotations;
using Instituto.C.Helpers;

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

        [Required(ErrorMessage = Messages.FechaRequerida)]
        [DataType(DataType.Date)]
        public DateTime FechaInscripcion { get; set; }
        [Required(ErrorMessage = Messages.CampoObligatorio)]
        public bool Activa { get; set; }

    }
}
