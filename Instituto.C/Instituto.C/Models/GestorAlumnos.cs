using System;
using System.Linq;
using Instituto.C.Data;

namespace Instituto.C.Models
{
    public class GestorAlumnos
    {
        private const string PREFIJO_MATRICULA = "A-"; //prefijo de la matrícula, se inicializa en "A-"
        private const string SEPARADOR = "/"; //separador de la matrícula, se inicializa en "/"



        public void AsignarNumeroMatricula(Alumno alumno, InstitutoDb context) //paso el DbContext al gestor para poder consultar si otro la tiene antes de asignarla.
        {
            if (alumno.Id > 0)
            {
                string matriculaPotencial;
                bool matriculaExistente;

                do
                {
                    int ValorRandom = new Random().Next(000000, 999999); //genero un numero aleatorio de 6 digitos entre 000000 y 999999
                    string UltimoTresDni = alumno.DNI.Substring(alumno.DNI.Length - 3); //obtengo los ultimos 3 digitos del dni
                    matriculaPotencial = $"{PREFIJO_MATRICULA}{ValorRandom}{SEPARADOR}{UltimoTresDni}"; //asigno la matricula a una variable, pero antes verificamos que otro alumno no la tenga.
                    matriculaExistente = context.Alumnos.Any(a => a.NumeroMatricula == matriculaPotencial); //valido en si hay otra igual a la recién "creada" en la instancia de la DbContext

                } while (matriculaExistente); //esto lo hago mientras sea sea false, si es false sale de la iteracion y se asigna.

                alumno.NumeroMatricula = matriculaPotencial;

            }






        }
    }
}
