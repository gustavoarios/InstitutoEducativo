namespace Instituto.C.Models
{
    public class Alumno : Persona
    {
        //declaro las propiedades del Alumno
        public int NumeroMatricula { get; set; }
        public Carrera Carrera { get; set; }
        public List<Inscripcion> Inscripciones { get; set; }
        public List<Calificacion> Calificaciones { get; set; }

    }
}
