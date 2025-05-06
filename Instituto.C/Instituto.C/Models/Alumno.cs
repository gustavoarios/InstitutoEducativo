using System.Collections.Generic;
using Instituto.C.Helpers;
using System.ComponentModel.DataAnnotations;
using System;

namespace Instituto.C.Models
{
    public class Alumno : Persona
    {

        //valor estatico para la asignacion de matriculas
        private static int _contadorMatricula = 1; //contador para la matricula, se inicializa en 0 y se incrementa cada vez que se crea un nuevo alumno

        //declaro una constante para el prefijo de la matricula
        private const String PREFIJO_MATRICULA = "A-"; //prefijo para la matricula, se inicializa en "A-"

        //no incluyo la propiedad Id porque hereda de Persona

        //declaro las propiedades del Alumno

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [StringLength(20, MinimumLength = 2, ErrorMessage = Messages.StrMaxMin)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = Messages.RestriccionAlfanumerica)]
        public String NumeroMatricula { get; private set; } //la hago private asi no se modifica de afuera

        //propiedad navegacional
        [Required(ErrorMessage = Messages.CampoObligatorio)]
        public Carrera Carrera { get; set; }

        //propiedad relacional
        [Required(ErrorMessage = Messages.CampoObligatorio)]
        public int CarreraId { get; set; } //propiedad que relaciona el Alumno con la Carrera

        public List<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>(); //inicializo la lista para evitar null reference exception
        public List<Calificacion> Calificaciones { get; set; } = new List<Calificacion>(); //inicializo la lista para evitar null reference exception
    

        public Alumno (string userName, string email, string nombre, string apellido, string dni, string telefono, string direccion, Carrera carrera) : base() //constructor con parametros
        {
            UserName = userName;
            Email = email;
            Nombre = nombre;
            Apellido = apellido;
            DNI = dni;
            Telefono = telefono;
            Direccion = direccion;
            Carrera = carrera; //asigno la carrera al alumno
            AsignarMatricula(); //llamo al metodo y asigno la matricula al alumno
        }


        private void AsignarMatricula()
        {
                int contadorActual = _contadorMatricula++;
                NumeroMatricula = $"{PREFIJO_MATRICULA}{contadorActual.ToString("D6")}"; //asigno la matricula al alumno concatenando el prefijo "A-" y que sea de 6 digitos.
        }

    }
}