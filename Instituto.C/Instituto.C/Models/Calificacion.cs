using System;
using System.ComponentModel.DataAnnotations;
using Instituto.C.Helpers;

namespace Instituto.C.Models
{
    public class Calificacion
    {
        public int Id { get; set; }

        [DataType(DataType.Date, ErrorMessage = Messages.RestriccionNumeros)]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        public Nota Nota { get; set; }

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [Range(1, int.MaxValue, ErrorMessage = Messages.RestriccionNumeros)]
        public int ProfesorId { get; set; }
        public Profesor Profesor { get; set; }

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [Range(1, int.MaxValue, ErrorMessage = Messages.RestriccionNumeros)]
        public int AlumnoId { get; set; }

        public Alumno Alumno { get; set; }

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [Range(1, int.MaxValue, ErrorMessage = Messages.RestriccionNumeros)]
        public int MateriaCursadaId { get; set; }

        public Inscripcion Inscripcion { get; set; }
    }
}
