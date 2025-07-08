using Instituto.C.Data;
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

        //recibe el contexto y evita duplicados
        public static MateriaCursada CrearNuevaCursadaSiEstaLleno(MateriaCursada cursada, InstitutoDb context)
        {
            if (!EstaLleno(cursada)) return null;

            // Buscar si ya existe una alternativa para la misma materia, mismo año, cuatrimestre y activa
            var existentes = context.MateriasCursadas
                .Where(mc =>
                    mc.MateriaId == cursada.MateriaId &&
                    mc.Anio == cursada.Anio &&
                    mc.Cuatrimestre == cursada.Cuatrimestre &&
                    mc.Activo &&
                    mc.Id != cursada.Id)
                .ToList();

            // Si hay alguna alternativa que no esté llena, la usamos
            foreach (var alt in existentes)
            {
                context.Entry(alt).Collection(x => x.Inscripciones).Load();
                context.Entry(alt).Reference(x => x.Materia).Load();

                if (!EstaLleno(alt))
                    return alt;
            }

            // Si no hay ninguna alternativa libre, se crea una nueva
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
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToHashSet() ?? new HashSet<int>();

            return todasLasCursadas
                .Where(mc => mc.Materia.CarreraId == alumno.CarreraId &&
                             !materiasYaCursadas.Contains(mc.MateriaId))
                .ToList();
        }
    }
}

