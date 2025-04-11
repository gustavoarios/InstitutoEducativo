namespace Instituto.C.Models
{
    public class Carrera
    {
        //propiedades de la Carrera
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<Materia> Materias { get; set; }
        public List<Alumno> Alumnos { get; set; }

    }
}
