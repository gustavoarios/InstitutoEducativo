namespace Instituto.C.Models
{
    public class Inscripcion
    {
        public int Id { get; set; }
        public Alumno Alumno { get; set; }
        public MateriaCursada MateriaCursada { get; set; }
        public DateTime FechaInscripcion { get; set; }
        public bool Activo { get; set; }

    }
}
