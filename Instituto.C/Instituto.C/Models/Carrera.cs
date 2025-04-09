namespace Instituto.C.Models
{
    public class Carrera
    {
        //propiedades de la Carrera
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<Materia> Materias { get; set; }
        public List<Alumno> Alumnos { get; set; }

        //constructor por defecto
        public Carrera()
        {
            Materias = new List<Materia>();
            Alumnos = new List<Alumno>();
        }

        //constructor parametrizado
        public Carrera(String nombre, int id)
        {
            Nombre = nombre;
            Id = id;
            Materias = new List<Materia>();
            Alumnos = new List<Alumno>();
        }
    }
}
