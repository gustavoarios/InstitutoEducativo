using System.Collections.Generic;

namespace Instituto.C.Models
{
    public class Materia
    {
        //propiedades de la Materia
        public int Id { get; set; } //Id de la materia
        public Carrera Carrera { get; set; } //Nombre de la Carrera
        public string Nombre { get; set; } //Ej: "Programacion I"
        public string CodigoMateria { get; set; } //Ej: "P1"
        public string Descripcion { get; set; } //Texto libre
        public int CupoMaximo { get; set; } //Capacidad maxima de alumnos por comision
        public List<MateriaCursada> Cursadas { get; set; }
    }
}
