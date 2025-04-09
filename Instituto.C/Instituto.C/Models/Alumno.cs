namespace Instituto.C.Models
{
    public class Alumno : Persona
    {
        //declaro las propiedades del Alumno
        public int Id { get; set; }
        public int NumeroMatricula { get; set; }
        public Carrera Carrera { get; set; }
        public List<Inscripcion> Inscripciones { get; set; }
        public List<Calificacion> Calificaciones { get; set; }

        //constructor por defecto
        public Alumno() : base()
        {
            Inscripciones = new List<Inscripcion>();
            Calificaciones = new List<Calificacion>();
        }

        //constructor parametrizado
        public Alumno(int idPersona, string userName, string email, string nombre, string apellido, string dni, string telefono, string direccion, int idAlumno, int numeroMatricula, Carrera carrera) : base()
        {
            base.Id = idPersona;
            UserName = userName;
            Email = email;
            Nombre = nombre;
            Apellido = apellido;
            DNI = dni;
            Telefono = telefono;
            Direccion = direccion;
            Activo = true;
            FechaAlta = DateTime.Now;

            this.Id = idAlumno;
            Carrera = carrera;
            NumeroMatricula = numeroMatricula;
            Inscripciones = new List<Inscripcion>();
            Calificaciones = new List<Calificacion>();

        }
    }
}
