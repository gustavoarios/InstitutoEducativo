namespace Instituto.C.Models
{
    public class Calificacion
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public Nota Nota { get; set; }
        public Profesor Profesor { get; set; }
        public Inscripcion Inscripcion { get; set; }
        public Alumno Alumno { get; set; }
    }
}
