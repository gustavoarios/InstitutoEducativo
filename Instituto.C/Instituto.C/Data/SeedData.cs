using Instituto.C.Helpers;
using Instituto.C.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instituto.C.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(InstitutoDb context, UserManager<Persona> userManager)
        {
            context.Database.Migrate();

            // === EMPLEADOS ===
            if (!context.Users.Any(u => u is Empleado))
            {
                var empleados = new[]
                {
                    new Empleado { UserName = "empleado1", Email = "empleado1@ort.edu.ar", Nombre = "Claudia", Apellido = "Rodriguez", DNI = "21212121", Telefono = "1122334455", Direccion = "Calle Falsa 123", FechaAlta = DateTime.Now, Activo = true, EmailConfirmed = true },
                    new Empleado { UserName = "empleado2", Email = "empleado2@ort.edu.ar", Nombre = "Luis", Apellido = "Martinez", DNI = "23232323", Telefono = "5566778899", Direccion = "Avenida Siempreviva 742", FechaAlta = DateTime.Now, Activo = true, EmailConfirmed = true },
                    new Empleado { UserName = "empleado3", Email = "empleado3@ort.edu.ar", Nombre = "Gerardo", Apellido = "Dominguez", DNI = "24242424", Telefono = "5566778899", Direccion = "Boulevard de las Rosas 720", FechaAlta = DateTime.Now, Activo = true, EmailConfirmed = true }
                };
                foreach (var e in empleados)
                {
                    await userManager.CreateAsync(e, "Password1!");
                    await userManager.AddToRoleAsync(e, "EmpleadoRol");
                }
            }

            // === PROFESORES ===
            if (!context.Users.Any(u => u is Profesor))
            {
                var profesores = new[]
                {
                    new Profesor { UserName = "profesor1", Email = "profesor1@ort.edu.ar", Nombre = "Walter", Apellido = "White", DNI = "23235689", Telefono = "1123748574", Direccion = "Cloruro 2114", FechaAlta = DateTime.Now, Activo = true, EmailConfirmed = true },
                    new Profesor { UserName = "profesor2", Email = "profesor2@ort.edu.ar", Nombre = "Marie", Apellido = "Curie", DNI = "25251436", Telefono = "1174859674", Direccion = "Rayos 1375", FechaAlta = DateTime.Now, Activo = true, EmailConfirmed = true },
                    new Profesor { UserName = "profesor3", Email = "profesor3@ort.edu.ar", Nombre = "Steve", Apellido = "Jobs", DNI = "74748596", Telefono = "1154896541", Direccion = "Silicon Valley 787", FechaAlta = DateTime.Now, Activo = true, EmailConfirmed = true }
                };
                foreach (var p in profesores)
                {
                    await userManager.CreateAsync(p, "Password1!");
                    await userManager.AddToRoleAsync(p, "ProfesorRol");
                }
            }

            // === CARRERAS ===
            var carreraAnalista = context.Carreras.FirstOrDefault(c => c.CodigoCarrera == "ANSIS");
            if (carreraAnalista == null)
            {
                carreraAnalista = new Carrera { Nombre = "Analista de Sistemas", CodigoCarrera = "ANSIS" };
                context.Carreras.Add(carreraAnalista);
            }

            var carreraCiencias = context.Carreras.FirstOrDefault(c => c.CodigoCarrera == "CSNT");
            if (carreraCiencias == null)
            {
                carreraCiencias = new Carrera { Nombre = "Ciencias Naturales", CodigoCarrera = "CSNT" };
                context.Carreras.Add(carreraCiencias);
            }
            context.SaveChanges();

            // === MATERIAS ===
            if (!context.Materias.Any())
            {
                var materias = new[]
                {
                    // Analista de Sistemas (cupo 5)
                    new Materia { Nombre = "Organización Empresarial", CodigoMateria = "OE101", Descripcion = "Estructura y funcionamiento de organizaciones", CupoMaximo = 5, CarreraId = carreraAnalista.Id },
                    new Materia { Nombre = "Introducción a la Informática", CodigoMateria = "INF101", Descripcion = "Conceptos básicos de informática", CupoMaximo = 5, CarreraId = carreraAnalista.Id },
                    new Materia { Nombre = "Fundamentos de Programación", CodigoMateria = "FP101", Descripcion = "Lógica y estructuras de programación", CupoMaximo = 5, CarreraId = carreraAnalista.Id },
                    new Materia { Nombre = "Taller de Herramientas de Programación", CodigoMateria = "THP101", Descripcion = "Herramientas actuales de desarrollo", CupoMaximo = 5, CarreraId = carreraAnalista.Id },
                    new Materia { Nombre = "Matemática", CodigoMateria = "MAT101", Descripcion = "Fundamentos matemáticos", CupoMaximo = 5, CarreraId = carreraAnalista.Id },
                    new Materia { Nombre = "Inglés Técnico", CodigoMateria = "ING101", Descripcion = "Inglés aplicado a sistemas", CupoMaximo = 5, CarreraId = carreraAnalista.Id },
                    new Materia { Nombre = "Taller de Creatividad e Innovación", CodigoMateria = "TCI101", Descripcion = "Creatividad aplicada a tecnología", CupoMaximo = 5, CarreraId = carreraAnalista.Id },
                    new Materia { Nombre = "Sistemas Administrativos", CodigoMateria = "SA201", Descripcion = "Sistemas de gestión", CupoMaximo = 5, CarreraId = carreraAnalista.Id },
                    new Materia { Nombre = "Arquitectura y Sistemas Operativos", CodigoMateria = "ASO201", Descripcion = "Componentes de hardware y software base", CupoMaximo = 5, CarreraId = carreraAnalista.Id },
                    new Materia { Nombre = "Programación I", CodigoMateria = "P1", Descripcion = "Estructuras básicas de programación", CupoMaximo = 5, CarreraId = carreraAnalista.Id },
                    new Materia { Nombre = "Taller de Programación I", CodigoMateria = "TP1", Descripcion = "Resolución de problemas con código", CupoMaximo = 5, CarreraId = carreraAnalista.Id },
                    new Materia { Nombre = "PP I: Programación en Nuevas Tecnologías I", CodigoMateria = "PP1", Descripcion = "Desarrollo con tecnologías actuales", CupoMaximo = 5, CarreraId = carreraAnalista.Id },
                    new Materia { Nombre = "Base de Datos I", CodigoMateria = "BD1", Descripcion = "Modelado y consultas relacionales", CupoMaximo = 5, CarreraId = carreraAnalista.Id },

                    // Ciencias Naturales
                    new Materia { Nombre = "Biología General", CodigoMateria = "BIO101", Descripcion = "Estudio de los principios básicos de la biología", CupoMaximo = 30, CarreraId = carreraCiencias.Id },
                    new Materia { Nombre = "Física I", CodigoMateria = "FIS101", Descripcion = "Conceptos fundamentales de la física clásica", CupoMaximo = 30, CarreraId = carreraCiencias.Id },
                    new Materia { Nombre = "Química General", CodigoMateria = "QUI101", Descripcion = "Conceptos básicos de química", CupoMaximo = 30, CarreraId = carreraCiencias.Id },
                    new Materia { Nombre = "Matemática Aplicada", CodigoMateria = "MAT201", Descripcion = "Aplicaciones matemáticas a las ciencias", CupoMaximo = 30, CarreraId = carreraCiencias.Id },
                    new Materia { Nombre = "Métodos Científicos", CodigoMateria = "MC202", Descripcion = "Metodología de investigación científica", CupoMaximo = 30, CarreraId = carreraCiencias.Id },
                    new Materia { Nombre = "Ecología y Medioambiente", CodigoMateria = "ECO301", Descripcion = "Relación entre seres vivos y su entorno", CupoMaximo = 30, CarreraId = carreraCiencias.Id },
                    new Materia { Nombre = "Astronomía", CodigoMateria = "AST201", Descripcion = "Cuerpos celestes y universo", CupoMaximo = 30, CarreraId = carreraCiencias.Id },
                    new Materia { Nombre = "Estadística para Ciencias", CodigoMateria = "EST204", Descripcion = "Estadística aplicada a investigación", CupoMaximo = 30, CarreraId = carreraCiencias.Id },
                    new Materia { Nombre = "Genética Molecular", CodigoMateria = "GEN302", Descripcion = "Herencia y función de los genes", CupoMaximo = 30, CarreraId = carreraCiencias.Id },
                    new Materia { Nombre = "Geología", CodigoMateria = "GEO101", Descripcion = "Estructura y procesos de la Tierra", CupoMaximo = 30, CarreraId = carreraCiencias.Id },
                    new Materia { Nombre = "Filosofía de la Ciencia", CodigoMateria = "FIL301", Descripcion = "Fundamentos epistemológicos", CupoMaximo = 30, CarreraId = carreraCiencias.Id },
                    new Materia { Nombre = "Bioquímica", CodigoMateria = "BIO201", Descripcion = "Procesos bioquímicos fundamentales", CupoMaximo = 30, CarreraId = carreraCiencias.Id },
                    new Materia { Nombre = "Antropología Biológica", CodigoMateria = "ANT401", Descripcion = "Evolución y diversidad humana", CupoMaximo = 30, CarreraId = carreraCiencias.Id }
                };
                context.Materias.AddRange(materias);
                context.SaveChanges();
            }

            // === ALUMNOS ===
            if (!context.Users.Any(u => u is Alumno))
            {
                string[] nombresTecnologia = new[]
                {
                    "Ada", "Alan", "Grace", "Linus", "Dennis", "Ken", "Margaret", "Tim", "Elon", "Bill", "Steve", "Mark", "Guido", "James", "Donald"
                };

                string[] apellidosTecnologia = new[]
                {
                    "Lovelace", "Turing", "Hopper", "Torvalds", "Ritchie", "Thompson", "Hamilton", "Berners-Lee", "Musk", "Gates", "Jobs", "Zuckerberg", "van Rossum", "Gosling", "Knuth"
                };

                string[] nombresCiencias = new[]
                {
                    "Marie", "Charles", "Isaac", "Rosalind", "Stephen", "Jane", "Gregor", "Carl", "Rachel", "Alexander", "Barbara", "Neil", "Lise", "Galileo", "Alfred"
                };

                string[] apellidosCiencias = new[]
                {
                    "Curie", "Darwin", "Newton", "Franklin", "Hawking", "Goodall", "Mendel", "Sagan", "Carson", "Fleming", "McClintock", "Tyson", "Meitner", "Galilei", "Wallace"
                };

                for (int i = 1; i <= 30; i++)
                {
                    bool esAnalista = i <= 15;
                    string nombre = esAnalista ? nombresTecnologia[i - 1] : nombresCiencias[i - 16];
                    string apellido = esAnalista ? apellidosTecnologia[i - 1] : apellidosCiencias[i - 16];
                    var carreraAsignada = esAnalista ? carreraAnalista : carreraCiencias;

                    var alumno = new Alumno
                    {
                        UserName = $"alumno{i}",
                        Email = $"alumno{i}@ort.edu.ar",
                        Nombre = nombre,
                        Apellido = apellido,
                        DNI = $"3500000{i:D2}",
                        Telefono = $"11000000{i:D2}",
                        Direccion = $"Calle {i}",
                        FechaAlta = DateTime.Now,
                        Activo = true,
                        CarreraId = carreraAsignada.Id,
                        EmailConfirmed = true
                    };

                    var gestor = new GestorAlumnos();
                    gestor.AsignarNumeroMatricula(alumno, context);

                    await userManager.CreateAsync(alumno, "Password1!");
                    await userManager.AddToRoleAsync(alumno, "AlumnoRol");

                    context.Update(alumno);
                }
                await context.SaveChangesAsync();
            }

            //aca se sigue agregando


            // === MATERIAS CURSADAS ===
            // === MATERIAS CURSADAS ===
            var codigosAnalista = new[]
            {
    "OE101", "INF101", "FP101", "THP101", "MAT101", "ING101", "TCI101",
    "SA201", "ASO201", "P1", "TP1", "PP1", "BD1"
};

            var materiasAnalista = await context.Materias
                .Where(m => codigosAnalista.Contains(m.CodigoMateria))
                .ToListAsync();

            var profesoresDisponibles = await context.Profesores.ToListAsync();
            int anioActual = DateTime.Now.Year;
            int cuatrimestre = 1;
            string codigoInicial = "A";

            int contadorCursadas = 0;
            foreach (var materia in materiasAnalista)
            {
                bool yaTieneCursada = await context.MateriasCursadas
                    .AnyAsync(mc => mc.MateriaId == materia.Id &&
                                    mc.Anio == anioActual &&
                                    mc.Cuatrimestre == cuatrimestre &&
                                    mc.CodigoCursada == codigoInicial);

                if (!yaTieneCursada)
                {
                    var profesorAsignado = profesoresDisponibles[contadorCursadas % profesoresDisponibles.Count];

                    var cursada = new MateriaCursada
                    {
                        MateriaId = materia.Id,
                        ProfesorId = profesorAsignado.Id,
                        Anio = anioActual,
                        Cuatrimestre = cuatrimestre,
                        CodigoCursada = codigoInicial,
                        Activo = true,
                        Nombre = $"{materia.CodigoMateria}-{anioActual}-{cuatrimestre}C-{codigoInicial}"
                    };

                    context.MateriasCursadas.Add(cursada);
                    contadorCursadas++;
                }
            }

            await context.SaveChangesAsync();


            // === INSCRIPCIONES PARA alumno1 a alumno5 ===
            var alumnos = await context.Alumnos
                .Where(a => a.UserName.StartsWith("alumno"))
                .OrderBy(a => a.UserName)
                .Take(5)
                .ToListAsync();

            var materiasCursadas = await context.MateriasCursadas
                .Include(mc => mc.Materia)
                .Where(mc => mc.Activo && mc.Anio == DateTime.Now.Year && mc.Cuatrimestre == 1)
                .OrderBy(mc => mc.Materia.CodigoMateria)
                .Take(5)
                .ToListAsync();

            foreach (var alumno in alumnos)
            {
                foreach (var cursada in materiasCursadas)
                {
                    bool yaInscripto = await context.Inscripciones.AnyAsync(i =>
                        i.AlumnoId == alumno.Id && i.MateriaCursadaId == cursada.Id);

                    if (!yaInscripto)
                    {
                        var inscripcion = new Inscripcion
                        {
                            AlumnoId = alumno.Id,
                            MateriaCursadaId = cursada.Id,
                            FechaInscripcion = DateTime.Now,
                            Activa = true
                        };

                        context.Inscripciones.Add(inscripcion);
                    }
                }
            }

            await context.SaveChangesAsync();


        }
    }
}
