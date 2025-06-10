using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Instituto.C.Helpers;

namespace Instituto.C.Models
{
    public class Inscripcion    
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [Range(1, int.MaxValue, ErrorMessage = Messages.RestriccionNumeros)]
        public int AlumnoId { get; set; }

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [Range(1, int.MaxValue, ErrorMessage = Messages.RestriccionNumeros)]
        public int MateriaCursadaId { get; set; }

        public Alumno Alumno { get; set; }

        [Display(Name =Alias.MateriaCursada)]
        public MateriaCursada MateriaCursada { get; set; }

        //[Required(ErrorMessage = Messages.CampoObligatorio)] No hace falta si la agrega el sistema, si no se completa la fecha usa la fecha actual
        [DataType(DataType.Date, ErrorMessage = Messages.RestriccionNumeros)]

        [Display(Name = Alias.FechaInscripcion)]
        public DateTime FechaInscripcion { get; set; } = DateTime.Now;

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        public bool Activa { get; set; }

        public List<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();
    }
}
