using Instituto.C.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Instituto.C.Helpers
{
    public class MateriasHelper
    {

        public static string ObtenerSiguienteCodigoCursada(string codigoActual)
        {
            if (string.IsNullOrEmpty(codigoActual)) return "A";
            char codigo = codigoActual[0];
            if (codigo == 'Z') throw new InvalidOperationException("Se acabaron las letras, profe.");
            return ((char)(codigo + 1)).ToString();
        }

        public static bool EstaLleno(MateriaCursada cursada)
        {
            if (cursada.Inscripciones == null || cursada.Materia == null)
                return false;

            int inscripcionesActivas = cursada.Inscripciones.Count(i => i.Activa);
            return inscripcionesActivas >= cursada.Materia.CupoMaximo;
        }

        public static MateriaCursada CrearNuevaCursadaSiEstaLleno(MateriaCursada cursada)
        {
            if (!EstaLleno(cursada)) return null;

            return new MateriaCursada
            {
                MateriaId = cursada.MateriaId,
                Materia = cursada.Materia,
                Anio = cursada.Anio,
                Cuatrimestre = cursada.Cuatrimestre,
                CodigoCursada = ObtenerSiguienteCodigoCursada(cursada.CodigoCursada),
                Activo = true,
                ProfesorId = cursada.ProfesorId,
                Profesor = cursada.Profesor
            };
        }

        public static string GenerarNombreCursada(MateriaCursada cursada)
        {
            return $"{cursada.Materia?.CodigoMateria ?? "SinMateria"}-{cursada.Anio}-{cursada.Cuatrimestre}C-{cursada.CodigoCursada}";
        }

        public static List<MateriaCursada> FiltrarMateriasDisponiblesParaAlumno(Alumno alumno, List<MateriaCursada> todasLasCursadas)
        {
            var materiasYaCursadas = alumno.Inscripciones?
                .Where(i => i.Calificacion != null || i.Activa)
                .Select(i => i.MateriaCursada?.MateriaId)
                .Where(id => id.HasValue) // Filter out null values
                .Select(id => id.Value)  // Extract the actual values
                .Distinct()
                .ToHashSet() ?? new HashSet<int>();

            return todasLasCursadas
                .Where(mc => mc.Materia.CarreraId == alumno.CarreraId &&
                             !materiasYaCursadas.Contains(mc.MateriaId))
                .ToList();
        }



    }
}
