using Instituto.C.Helpers;
using Instituto.C.Models;
using System.Linq;

namespace Instituto.C.ViewModels
{
    public class MateriaCursadaConPromedioViewModel
    {
        public MateriaCursada MateriaCursada { get; set; }

        public double Promedio
        {
            get
            {
                var notas = MateriaCursada.Inscripciones?
                            .Where(i => i.Calificacion != null && (int)i.Calificacion.Nota >= 0 && (int)i.Calificacion.Nota <= 10)
                            .Select(i => i.Calificacion.Nota) // Mantiene el tipo Nota
                            .ToList();

                return NotasHelper.CalcularPromedio(notas);

            }
        }
    }

}
