using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Instituto.C.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Instituto.C.Data
{
    public static class SeedData
    {

        public static async Task InitializeAsync(InstitutoDb context, UserManager<Persona> userManager)
        {
            context.Database.Migrate();

            //creo los empleados
            if (!context.Users.Any(u => u is Empleado))
            {
                var emp1 = new Empleado
                {
                    UserName = "empleado1",
                    Email = "empleado1@ort.edu.ar",
                    Nombre = "Claudia",
                    Apellido = "Rodriguez",
                    DNI = "21212121",
                    Telefono = "1122334455",
                    Direccion = "Calle Falsa 123",
                    FechaAlta = DateTime.Now,
                    Activo = true,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(emp1, "Password1!");
                await userManager.AddToRoleAsync(emp1, "EmpleadoRol");

                var emp2 = new Empleado
                {
                    UserName = "empleado2",
                    Email = "empleado2@ort.edu.ar",
                    Nombre = "Luis",
                    Apellido = "Martinez",
                    DNI = "23232323",
                    Telefono = "5566778899",
                    Direccion = "Avenida Siempreviva 742",
                    FechaAlta = DateTime.Now,
                    Activo = true,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(emp2, "Password1!");
                await userManager.AddToRoleAsync(emp2, "EmpleadoRol");

                var emp3 = new Empleado
                {
                    UserName = "empleado3",
                    Email = "empleado3@ort.edu.ar",
                    Nombre = "Gerardo",
                    Apellido = "Dominguez",
                    DNI = "24242424",
                    Telefono = "5566778899",
                    Direccion = "Boulevard de las Rosas 720",
                    FechaAlta = DateTime.Now,
                    Activo = true,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(emp3, "Password1!");
                await userManager.AddToRoleAsync(emp3, "EmpleadoRol");
            }

            //creo los profesores

            if (!context.Users.Any(u => u is Profesor))
            {
                var profesor1 = new Profesor
                {
                    UserName = "profesor1",
                    Email = "profesor1@ort.edu.ar",
                    Nombre = "Walter",
                    Apellido = "White",
                    DNI = "23235689",
                    Telefono = "1123748574",
                    Direccion = "Cloruro 2114",
                    FechaAlta = DateTime.Now,
                    Activo = true,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(profesor1, "Password1!");
                await userManager.AddToRoleAsync(profesor1, "ProfesorRol");

                var profesor2 = new Profesor
                {
                    UserName = "profesor2",
                    Email = "profesor2@ort.edu.ar",
                    Nombre = "Marie",
                    Apellido = "Curie",
                    DNI = "25251436",
                    Telefono = "1174859674",
                    Direccion = "Rayos 1375",
                    FechaAlta = DateTime.Now,
                    Activo = true,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(profesor2, "Password1!");
                await userManager.AddToRoleAsync(profesor2, "ProfesorRol");

                var profesor3 = new Profesor
                {
                    UserName = "profesor3",
                    Email = "profesor3@ort.edu.ar",
                    Nombre = "Steave",
                    Apellido = "Jobs",
                    DNI = "74748596",
                    Telefono = "1154896541",
                    Direccion = "Silicon Valley 787",
                    FechaAlta = DateTime.Now,
                    Activo = true,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(profesor3, "Password1!");
                await userManager.AddToRoleAsync(profesor3, "ProfesorRol");

            }

            Carrera carrera = null;
            Carrera carrera1 = null;
            Carrera carrera2 = null;

            // === CARRERAS ===
            if (!context.Carreras.Any())
            {
                carrera = new Carrera
                {
                    Nombre = "Ciencias Naturales",
                    CodigoCarrera = "CSNT"
                };

                carrera1 = new Carrera
                {
                    Nombre = "Ingeniería Química",
                    CodigoCarrera = "INGQUI"
                };

                carrera2 = new Carrera
                {
                    Nombre = "Analista de Sistemas",
                    CodigoCarrera = "ANSIS"
                };

                context.Carreras.AddRange(carrera, carrera1, carrera2);
                context.SaveChanges();
            }
            else
            {
                carrera = context.Carreras.FirstOrDefault(c => c.CodigoCarrera == "CSNT");
                carrera1 = context.Carreras.FirstOrDefault(c => c.CodigoCarrera == "INGQUI");
                carrera2 = context.Carreras.FirstOrDefault(c => c.CodigoCarrera == "ANSIS");
            }

            // === MATERIAS ===

            // === DECLARACIÓN DE VARIABLES ===
            Materia materia1 = null;
            Materia materia2 = null;
            Materia materia3 = null;
            Materia materia4 = null;
            Materia materia5 = null;
            Materia materia6 = null;
            Materia materia7 = null;
            Materia materia8 = null;
            Materia materia9 = null;
            Materia materia10 = null;
            Materia materia11 = null;
            Materia materia12 = null;
            Materia materia13 = null;
            Materia materia14 = null;
            Materia materia15 = null;
            Materia materia16 = null;
            Materia materia17 = null;
            Materia materia18 = null;
            Materia materia19 = null;
            Materia materia20 = null;
            Materia materia21 = null;
            Materia materia22 = null;
            Materia materia23 = null;
            Materia materia24 = null;
            Materia materia25 = null;
            Materia materia26 = null;
            Materia materia27 = null;
            Materia materia28 = null;
            Materia materia29 = null;
            Materia materia30 = null;
            Materia materia31 = null;
            Materia materia32 = null;

            if (!context.Materias.Any())
            {
                // === Ciencias Naturales ===
                materia1 = new Materia { Nombre = "Biología General", CodigoMateria = "BIO101", Descripcion = "Estudio de los principios básicos de la biología", CupoMaximo = 40, CarreraId = carrera.Id };
                materia2 = new Materia { Nombre = "Física I", CodigoMateria = "FIS101", Descripcion = "Conceptos fundamentales de la física clásica", CupoMaximo = 35, CarreraId = carrera.Id };
                materia3 = new Materia { Nombre = "Química General", CodigoMateria = "QUI101", Descripcion = "Introducción a los conceptos básicos de química", CupoMaximo = 30, CarreraId = carrera.Id };
                materia4 = new Materia { Nombre = "Matemática Aplicada", CodigoMateria = "MAT201", Descripcion = "Herramientas matemáticas aplicadas a las ciencias naturales", CupoMaximo = 30, CarreraId = carrera.Id };
                materia5 = new Materia { Nombre = "Ecología y Medioambiente", CodigoMateria = "ECO301", Descripcion = "Relaciones entre los seres vivos y su entorno", CupoMaximo = 30, CarreraId = carrera.Id };
                materia6 = new Materia { Nombre = "Métodos Científicos", CodigoMateria = "MET401", Descripcion = "Técnicas y métodos para la investigación científica", CupoMaximo = 25, CarreraId = carrera.Id };
                materia7 = new Materia { Nombre = "Genética Molecular", CodigoMateria = "GEN302", Descripcion = "Estudio de la herencia y la función de los genes a nivel molecular", CupoMaximo = 25, CarreraId = carrera.Id };
                materia8 = new Materia { Nombre = "Astronomía", CodigoMateria = "AST201", Descripcion = "Exploración de los cuerpos celestes y el universo", CupoMaximo = 30, CarreraId = carrera.Id };
                materia9 = new Materia { Nombre = "Geología", CodigoMateria = "GEO101", Descripcion = "Estudio de la estructura y procesos de la Tierra", CupoMaximo = 35, CarreraId = carrera.Id };
                materia10 = new Materia { Nombre = "Estadística para Ciencias", CodigoMateria = "EST204", Descripcion = "Fundamentos estadísticos aplicados a la investigación científica", CupoMaximo = 30, CarreraId = carrera.Id };
                materia11 = new Materia { Nombre = "Filosofía de la Ciencia", CodigoMateria = "FIL301", Descripcion = "Reflexión crítica sobre los métodos y fundamentos de la ciencia", CupoMaximo = 20, CarreraId = carrera.Id };

                // === Ingeniería Química ===
                materia12 = new Materia { Nombre = "Química General", CodigoMateria = "QUI101", Descripcion = "Fundamentos de la química general", CupoMaximo = 30, CarreraId = carrera1.Id };
                materia13 = new Materia { Nombre = "Física I", CodigoMateria = "FIS101", Descripcion = "Mecánica clásica y leyes del movimiento", CupoMaximo = 35, CarreraId = carrera1.Id };
                materia14 = new Materia { Nombre = "Matemática Aplicada", CodigoMateria = "MAT201", Descripcion = "Aplicaciones matemáticas a problemas de ingeniería", CupoMaximo = 40, CarreraId = carrera1.Id };
                materia15 = new Materia { Nombre = "Termodinámica", CodigoMateria = "TER202", Descripcion = "Principios de la termodinámica en procesos químicos", CupoMaximo = 30, CarreraId = carrera1.Id };
                materia16 = new Materia { Nombre = "Operaciones Unitarias I", CodigoMateria = "OPU301", Descripcion = "Procesos físicos fundamentales de la ingeniería química", CupoMaximo = 25, CarreraId = carrera1.Id };
                materia17 = new Materia { Nombre = "Química Analítica", CodigoMateria = "QAN203", Descripcion = "Técnicas para análisis cualitativo y cuantitativo", CupoMaximo = 30, CarreraId = carrera1.Id };
                materia18 = new Materia { Nombre = "Ingeniería de Procesos", CodigoMateria = "IPR302", Descripcion = "Diseño y análisis de procesos químicos industriales", CupoMaximo = 25, CarreraId = carrera1.Id };
                materia19 = new Materia { Nombre = "Transferencia de Calor", CodigoMateria = "TCA304", Descripcion = "Estudio de mecanismos de transmisión de calor", CupoMaximo = 25, CarreraId = carrera1.Id };
                materia20 = new Materia { Nombre = "Bioquímica Industrial", CodigoMateria = "BIO303", Descripcion = "Aplicaciones de la bioquímica en la industria química", CupoMaximo = 20, CarreraId = carrera1.Id };
                materia21 = new Materia { Nombre = "Control de Procesos", CodigoMateria = "CPL305", Descripcion = "Sistemas de control y automatización de procesos químicos", CupoMaximo = 25, CarreraId = carrera1.Id };

                // === Analista de Sistemas ===
                materia22 = new Materia { Nombre = "Programación 1", CodigoMateria = "P1", Descripcion = "Fundamentos de la programación", CupoMaximo = 25, CarreraId = carrera2.Id };
                materia23 = new Materia { Nombre = "Lógica Computacional", CodigoMateria = "LC101", Descripcion = "Conceptos básicos de lógica aplicados a la computación", CupoMaximo = 25, CarreraId = carrera2.Id };
                materia24 = new Materia { Nombre = "Sistemas Operativos", CodigoMateria = "SO201", Descripcion = "Funcionamiento interno de los sistemas operativos", CupoMaximo = 25, CarreraId = carrera2.Id };
                materia25 = new Materia { Nombre = "Base de Datos 1", CodigoMateria = "BD1", Descripcion = "Modelado y consultas sobre bases de datos relacionales", CupoMaximo = 25, CarreraId = carrera2.Id };
                materia26 = new Materia { Nombre = "Redes de Computadoras", CodigoMateria = "RED301", Descripcion = "Principios y configuración de redes informáticas", CupoMaximo = 25, CarreraId = carrera2.Id };
                materia27 = new Materia { Nombre = "Ingeniería de Software", CodigoMateria = "IS401", Descripcion = "Metodologías para el desarrollo de software a gran escala", CupoMaximo = 25, CarreraId = carrera2.Id };
                materia28 = new Materia { Nombre = "Programación 2", CodigoMateria = "P2", Descripcion = "Programación orientada a objetos y estructuras avanzadas", CupoMaximo = 25, CarreraId = carrera2.Id };
                materia29 = new Materia { Nombre = "Análisis de Sistemas", CodigoMateria = "AS501", Descripcion = "Técnicas de relevamiento, análisis y modelado de sistemas", CupoMaximo = 25, CarreraId = carrera2.Id };
                materia30 = new Materia { Nombre = "Proyecto Final", CodigoMateria = "PF601", Descripcion = "Desarrollo completo de un sistema informático en equipo", CupoMaximo = 20, CarreraId = carrera2.Id };
                materia31 = new Materia { Nombre = "Ética y Legislación Informática", CodigoMateria = "ELI701", Descripcion = "Aspectos legales y éticos del ejercicio profesional en informática", CupoMaximo = 25, CarreraId = carrera2.Id };
                materia32 = new Materia { Nombre = "Base de Datos 2", CodigoMateria = "BD2", Descripcion = "Bases de datos avanzadas y no relacionales", CupoMaximo = 25, CarreraId = carrera2.Id };

                context.Materias.AddRange(
                    materia1, materia2, materia3, materia4, materia5, materia6,
                    materia7, materia8, materia9, materia10, materia11, materia12,
                    materia13, materia14, materia15, materia16, materia17, materia18,
                    materia19, materia20, materia21, materia22, materia23, materia24,
                    materia25, materia26, materia27, materia28, materia29, materia30,
                    materia31, materia32
                );

                context.SaveChanges();
            }





        }

    }
}
