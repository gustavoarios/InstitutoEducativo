using Instituto.C.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Instituto.C.Data
{
    public class InstitutoDb : DbContext
    {
        public InstitutoDb(DbContextOptions options): base(options){
        
        } 
        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<Calificacion> Calificaciones { get; set; }
        public DbSet<Carrera> Carreras {  get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Materia> Materias { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<Profesor> Profesores { get; set; }
    }
}
