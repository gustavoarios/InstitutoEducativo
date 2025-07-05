using Instituto.C.Models;
using System.Collections.Generic;

namespace Instituto.C.ViewModels
{
    public class MisMateriasViewModel
    {
        public List<MateriaCursadaConPromedioViewModel> Vigentes { get; set; } = new();
        public List<MateriaCursadaConPromedioViewModel> Pasadas { get; set; } = new();
        public List<Calificacion> Calificaciones { get; set; } = new();
    }
}

