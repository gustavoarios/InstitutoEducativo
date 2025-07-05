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
                    .Where(i => i.Calificacion != null && (int)i.Calificacion.Nota > 0)
                    .Select(i => i.Calificacion.Nota)
                    .ToList();

                if (notas == null || notas.Count == 0)
                    return 0;

                return NotasHelper.CalcularPromedio(notas);
            }
        }

    }
}
