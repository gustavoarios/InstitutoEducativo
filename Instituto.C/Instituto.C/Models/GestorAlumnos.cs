using System;

namespace Instituto.C.Models
{
    public class GestorAlumnos
    { 
        private const string PREFIJO_MATRICULA = "A-"; //prefijo de la matrícula, se inicializa en "A-"

        public void AsignarNumeroMatricula(Alumno alumno)
        {
            if(alumno.Id > 0)
            {
                alumno.NumeroMatricula = $"{PREFIJO_MATRICULA}{alumno.Id:D6}"; //asigno la matricula al alumno concatenando el prefijo "A-" y que sea de 6 digitos.
            }
            else
            {
                Console.WriteLine("No se puede asignar número de matrícula porque el Alumno aún no tiene un Id asignado.");
            }
        }
    }
}
