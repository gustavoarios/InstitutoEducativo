using System;
using System.ComponentModel.DataAnnotations;

namespace Instituto.C.Models
{
    public class Calificacion
    {
        
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe ingresar la fecha de calificación")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una nota")]
        public Nota Nota { get; set; }


        [Required(ErrorMessage = "El ID del profesor es obligatorio")]
        public int ProfesorId { get; set; }
        public Profesor Profesor { get; set; }

        [Required(ErrorMessage = "El ID del alumno es obligatorio")]
        public int AlumnoId { get; set; }

        [Required(ErrorMessage = "El ID de la materia cursada es obligatorio")]
        public int MateriaCursadaId { get; set; }


        public Inscripcion Inscripcion { get; set; }


    }
}
