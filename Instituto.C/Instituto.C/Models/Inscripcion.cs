using System;
using System.ComponentModel.DataAnnotations;
using Instituto.C.Helpers;

namespace Instituto.C.Models
{
    public class Inscripcion
    {
        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [Range(1, int.MaxValue, ErrorMessage = Messages.RestriccionNumeros)]
        public int AlumnoId { get; set; }

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [Range(1, int.MaxValue, ErrorMessage = Messages.RestriccionNumeros)]
        public int MateriaCursadaId { get; set; }

        public Alumno Alumno { get; set; }
        public MateriaCursada MateriaCursada { get; set; }

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [DataType(DataType.Date, ErrorMessage = Messages.RestriccionNumeros)]
        public DateTime FechaInscripcion { get; set; }

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        public bool Activa { get; set; }
    }
}
