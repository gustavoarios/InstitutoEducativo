using Instituto.C.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Instituto.C.Data
{
    public class InstitutoDb(DbContextOptions options) : DbContext(options)
    {

        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<Calificacion> Calificaciones { get; set; }
        public DbSet<Carrera> Carreras { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Materia> Materias { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<Profesor> Profesores { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }
        public DbSet<MateriaCursada> MateriasCursadas { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Inscripcion>()
                .HasKey(cv => new { cv.AlumnoId, cv.MateriaCursadaId });

            modelBuilder.Entity<Inscripcion>()
                .HasOne(ai => ai.Alumno)
                .WithMany(i => i.Inscripciones)
                .HasForeignKey(ai => ai.AlumnoId);

            modelBuilder.Entity<Inscripcion>()
               .HasOne(mi => mi.MateriaCursada)
               .WithMany(m => m.Inscripciones)
               .HasForeignKey(mi => mi.MateriaCursadaId);

            modelBuilder.Entity<Alumno>()
                .HasIndex(a => a.NumeroMatricula) //indicamos a entity framework que la propiedad NumeroMatricula es un indice
                .IsUnique(); // osea no hay 2 alumnos con el mismo numero de matricula

            modelBuilder.Entity<MateriaCursada>()
                .HasIndex(mc => new { mc.MateriaId, mc.Anio, mc.Cuatrimestre, mc.CodigoCursada })
                .IsUnique(); // Asegura que no puedan existir dos cursadas con esos mismos datos
        }


    }
}
