using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Instituto.C.Models;

namespace Instituto.C.Data
{
    public static class SeedData
    {
        public static void Initialize(InstitutoDb context)
        {
            // Verificamos si ya hay datos cargados
            if (context.Alumnos.Any()) return;

            // === Profesores ===
            var profe1 = new Profesor { Id = 1, Nombre = "Walter", Apellido = "White" };
            var profe2 = new Profesor { Id = 2, Nombre = "Marie", Apellido = "Curie" };
            var profe3 = new Profesor { Id = 3, Nombre = "Richard", Apellido = "Feynman" };
            context.Profesores.AddRange(profe1, profe2, profe3);
            context.SaveChanges();

            // === Materias ===
            var materia1 = new Materia { Id = 1, Nombre = "Química Orgánica", CodigoMateria = "QO101", Descripcion = "Introducción a la química orgánica", CupoMaximo = 30, CarreraId = 1 };
            var materia2 = new Materia { Id = 2, Nombre = "Física Cuántica", CodigoMateria = "FC202", Descripcion = "Fenómenos cuánticos", CupoMaximo = 25, CarreraId = 1 };
            context.Materias.AddRange(materia1, materia2);
            context.SaveChanges();

            // === Materias Cursadas ===
            var cursada1 = new MateriaCursada { Id = 1, CodigoCursada = "A", Anio = 2025, Cuatrimestre = 1, Activo = true, MateriaId = materia1.Id, ProfesorId = profe1.Id };
            var cursada2 = new MateriaCursada { Id = 2, CodigoCursada = "B", Anio = 2025, Cuatrimestre = 1, Activo = true, MateriaId = materia2.Id, ProfesorId = profe2.Id };
            context.MateriasCursadas.AddRange(cursada1, cursada2);
            context.SaveChanges();

            // === Alumnos ===
            var alumno1 = new Alumno { Id = 1, Nombre = "Lisa", Apellido = "Simpson", NumeroMatricula = "2025LS", CarreraId = 1 };
            var alumno2 = new Alumno { Id = 2, Nombre = "Bart", Apellido = "Simpson", NumeroMatricula = "2025BS", CarreraId = 1 };
            context.Alumnos.AddRange(alumno1, alumno2);
            context.SaveChanges();

            // === Inscripciones ===
            var insc1 = new Inscripcion { Id = 1, AlumnoId = alumno1.Id, MateriaCursadaId = cursada1.Id, FechaInscripcion = DateTime.Now.AddDays(-10), Activa = true };
            var insc2 = new Inscripcion { Id = 2, AlumnoId = alumno2.Id, MateriaCursadaId = cursada2.Id, FechaInscripcion = DateTime.Now.AddDays(-5), Activa = true };
            context.Inscripciones.AddRange(insc1, insc2);
            context.SaveChanges();

            // === Calificaciones ===
            var calif1 = new Calificacion { Id = 1, AlumnoId = alumno1.Id, ProfesorId = profe1.Id, InscripcionId = insc1.Id, Fecha = DateTime.Now, Nota = Nota.Nueve };
            var calif2 = new Calificacion { Id = 2, AlumnoId = alumno2.Id, ProfesorId = profe2.Id, InscripcionId = insc2.Id, Fecha = DateTime.Now, Nota = Nota.Seis };
            context.Calificaciones.AddRange(calif1, calif2);
            context.SaveChanges();
        }
    }
}
