using System;
using System.ComponentModel.DataAnnotations;
using Instituto.C.Helpers;

namespace Instituto.C.Models
{
    public class Calificacion
    {
        
        public int Id { get; set; }

        [Required(ErrorMessage = Messages.FechaRequerida)]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = Messages.SeleccionarNota)]
        public Nota Nota { get; set; }


        [Required(ErrorMessage = Messages.IdProfesorRequerido)]
        public int ProfesorId { get; set; }
        public Profesor Profesor { get; set; }

        [Required(ErrorMessage = Messages.IdAlumnoRequerido)]
        public int AlumnoId { get; set; }

        [Required(ErrorMessage = Messages.IdMateriaRequerido)]
        public int MateriaCursadaId { get; set; }


        public Inscripcion Inscripcion { get; set; }


    }
}
