using System.Collections.Generic;

namespace Instituto.C.Models
{
    public class Profesor : Empleado
    {
        public List<MateriaCursada> MateriasCursada { get; set; }
        public List<Calificacion> Calificaciones { get; set; }
    }
}
