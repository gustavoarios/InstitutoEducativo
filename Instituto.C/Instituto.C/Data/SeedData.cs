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
            // Si ya hay alumnos, se asume que los datos están cargados
            if (context.Alumnos.Any()) return;

            // === Profesores ===
            var profe1 = new Profesor { Nombre = "Walter", Apellido = "White" };
            var profe2 = new Profesor { Nombre = "Marie", Apellido = "Curie" };
            var profe3 = new Profesor { Nombre = "Richard", Apellido = "Feynman" };
            context.Profesores.AddRange(profe1, profe2, profe3);
            context.SaveChanges();

            // === Carreras ===
            var carrera = new Carrera { Nombre = "Ciencias Naturales" };
            context.Carreras.Add(carrera);
            context.SaveChanges();

            // === Materias ===
            var materia1 = new Materia
            {
                Nombre = "Química Orgánica",
                CodigoMateria = "QO101",
                Descripcion = "Introducción a la química orgánica",
                CupoMaximo = 30,
                CarreraId = carrera.Id
            };
            var materia2 = new Materia
            {
                Nombre = "Física Cuántica",
                CodigoMateria = "FC202",
                Descripcion = "Fenómenos cuánticos",
                CupoMaximo = 25,
                CarreraId = carrera.Id
            };
            context.Materias.AddRange(materia1, materia2);
            context.SaveChanges();

            // === Materias Cursadas ===
            var cursada1 = new MateriaCursada
            {
                CodigoCursada = "A",
                Anio = 2025,
                Cuatrimestre = 1,
                Activo = true,
                MateriaId = materia1.Id,
                ProfesorId = profe1.Id
            };
            var cursada2 = new MateriaCursada
            {
                CodigoCursada = "B",
                Anio = 2025,
                Cuatrimestre = 1,
                Activo = true,
                MateriaId = materia2.Id,
                ProfesorId = profe2.Id
            };
            context.MateriasCursadas.AddRange(cursada1, cursada2);
            context.SaveChanges();

            // === Alumnos ===
            var alumno1 = new Alumno { Nombre = "Lisa", Apellido = "Simpson", NumeroMatricula = "2025LS", CarreraId = carrera.Id };
            var alumno2 = new Alumno { Nombre = "Bart", Apellido = "Simpson", NumeroMatricula = "2025BS", CarreraId = carrera.Id };
            context.Alumnos.AddRange(alumno1, alumno2);
            context.SaveChanges();

            // === Inscripciones ===
            var insc1 = new Inscripcion
            {
                AlumnoId = alumno1.Id,
                MateriaCursadaId = cursada1.Id,
                FechaInscripcion = DateTime.Now.AddDays(-10),
                Activa = true
            };
            var insc2 = new Inscripcion
            {
                AlumnoId = alumno2.Id,
                MateriaCursadaId = cursada2.Id,
                FechaInscripcion = DateTime.Now.AddDays(-5),
                Activa = true
            };
            context.Inscripciones.AddRange(insc1, insc2);
            context.SaveChanges();

            // === Calificaciones ===
            var calif1 = new Calificacion
            {
                AlumnoId = alumno1.Id,
                MateriaCursadaId = cursada1.Id,
                ProfesorId = profe1.Id,
                Fecha = DateTime.Now,
                Nota = Nota.Nueve
            };
            var calif2 = new Calificacion
            {
                AlumnoId = alumno2.Id,
                MateriaCursadaId = cursada2.Id,
                ProfesorId = profe2.Id,
                Fecha = DateTime.Now,
                Nota = Nota.Seis
            };
            context.Calificaciones.AddRange(calif1, calif2);
            context.SaveChanges();
        }
    }
}
