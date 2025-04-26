using System.Collections.Generic;

namespace Instituto.C.Models
{
    public class Profesor : Persona
    {
        public string Legajo { get; set; }
        public List<MateriaCursada> MateriasCursada { get; set; }
        public List<Calificacion> Calificaciones { get; set; }
    }
}
