namespace Instituto.C.Models
{
    public class Profesor
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime FechaAlta { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DNI { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public bool Activo { get; set; }
        public string Legajo { get; set; }
        public List<MateriaCursada> MateriasCursada { get; set; }
        public List<Calificacion> Calificaciones { get; set; }
    }
}
