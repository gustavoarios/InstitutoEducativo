using System.Collections.Generic;
using Instituto.C.Helpers;
using System.ComponentModel.DataAnnotations;
using System;

namespace Instituto.C.Models
{
    public class Alumno : Persona
    {

        //no incluyo la propiedad Id porque hereda de Persona

        //declaro las propiedades del Alumno
        [Display(Name = Alias.NumeroMatricula)]
        public String NumeroMatricula { get; set; }

        //propiedad navegacional
        [Display(Name = Alias.CodigoCarrera)]
        public Carrera Carrera { get; set; }
        public List<Inscripcion> Inscripciones { get; set; } = []; //inicializo la lista para evitar null reference exception
        public List<Calificacion> Calificaciones { get; set; } = []; //inicializo la lista para evitar null reference exception

        //propiedad relacional
        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [Display(Name = Alias.CodigoCarrera)]
        public int CarreraId { get; set; } //propiedad que relaciona el Alumno con la Carrera

        public Alumno()
        {
            
        }



    }
}