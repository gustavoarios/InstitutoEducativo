using Instituto.C.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Instituto.C.Data
{
    public class InstitutoDb : IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>
    {
        public InstitutoDb(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<Calificacion> Calificaciones { get; set; }
        public DbSet<Carrera> Carreras { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Materia> Materias { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<Profesor> Profesores { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }
        public DbSet<MateriaCursada> MateriasCursadas { get; set; }
        public DbSet<Rol> MisRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Inscripcion
            modelBuilder.Entity<Inscripcion>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<Inscripcion>()
                .HasIndex(i => new { i.AlumnoId, i.MateriaCursadaId })
                .IsUnique();

            modelBuilder.Entity<Inscripcion>()
                .HasOne(i => i.Alumno)
                .WithMany(a => a.Inscripciones)
                .HasForeignKey(i => i.AlumnoId);

            modelBuilder.Entity<Inscripcion>()
                .HasOne(i => i.MateriaCursada)
                .WithMany(mc => mc.Inscripciones)
                .HasForeignKey(i => i.MateriaCursadaId);

            // Alumno
            modelBuilder.Entity<Alumno>()
                .HasIndex(a => a.NumeroMatricula)
                .IsUnique();

            // MateriaCursada
            modelBuilder.Entity<MateriaCursada>()
                .HasOne(mc => mc.Profesor)
                .WithMany()
                .HasForeignKey(mc => mc.ProfesorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MateriaCursada>()
                .HasOne(mc => mc.Materia)
                .WithMany()
                .HasForeignKey(mc => mc.MateriaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Materia
            modelBuilder.Entity<Materia>()
                .HasIndex(m => new { m.CarreraId, m.Nombre })
                .IsUnique();

            // Calificacion
            modelBuilder.Entity<Calificacion>()
                .HasOne(c => c.Alumno)
                .WithMany(a => a.Calificaciones)
                .HasForeignKey(c => c.AlumnoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Calificacion>()
                .HasOne(c => c.Profesor)
                .WithMany(p => p.Calificaciones)
                .HasForeignKey(c => c.ProfesorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Calificacion>()
                .HasOne(c => c.Inscripcion)
                .WithMany(i => i.Calificaciones)
                .HasForeignKey(c => c.InscripcionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Identity mappings
            modelBuilder.Entity<IdentityUser<int>>().ToTable("Personas");
            modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("PersonasRoles");
        }
    }
}