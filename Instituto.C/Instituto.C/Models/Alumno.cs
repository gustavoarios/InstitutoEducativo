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

        public String NumeroMatricula { get; set; }

        //propiedad navegacional
        [Required(ErrorMessage = Messages.CampoObligatorio)]
        public Carrera Carrera { get; set; }
        public List<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>(); //inicializo la lista para evitar null reference exception
        public List<Calificacion> Calificaciones { get; set; } = new List<Calificacion>(); //inicializo la lista para evitar null reference exception

        //propiedad relacional
        [Required(ErrorMessage = Messages.CampoObligatorio)]
        public int CarreraId { get; set; } //propiedad que relaciona el Alumno con la Carrera

        public Alumno()
        {
            
        }



    }
}