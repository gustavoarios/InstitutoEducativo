using System;
using System.Linq;
using Instituto.C.Data;

namespace Instituto.C.Models
{
    public class GestorAlumnos
    {
        private const string PREFIJO_MATRICULA = "A-"; //prefijo de la matrícula, se inicializa en "A-"
        private const string SEPARADOR = "/"; //separador de la matrícula, se inicializa en "/"



        public void AsignarNumeroMatricula(Alumno alumno, InstitutoDb context)
        {
            string matriculaPotencial;
            bool matriculaExistente;

            do
            {
                int valorRandom = new Random().Next(000000, 999999);
                string ultimoTresDni = alumno.DNI.Substring(alumno.DNI.Length - 3);
                matriculaPotencial = $"{PREFIJO_MATRICULA}{valorRandom}{SEPARADOR}{ultimoTresDni}";

                matriculaExistente = context.Alumnos.Any(a => a.NumeroMatricula == matriculaPotencial);

            } while (matriculaExistente);

            alumno.NumeroMatricula = matriculaPotencial;
        }

    }
}
