using Instituto.C.Helpers;
using Instituto.C.Models;
using System.Collections.Generic;
using System.Linq;

namespace Instituto.C.ViewModels
{
    public class DetalleMateriaCursadaProfesorViewModel
    {
        public MateriaCursada MateriaCursada { get; set; }

        public List<Inscripcion> InscriptosActivos =>
            MateriaCursada?.Inscripciones?.Where(i => i.Activa).ToList() ?? new();

        public List<Inscripcion> InscriptosDadosDeBaja =>
            MateriaCursada?.Inscripciones?.Where(i => !i.Activa).ToList() ?? new();

        public double Promedio =>
            NotasHelper.CalcularPromedio(
            InscriptosActivos
             .Where(i => i.Calificacion != null)
             .Select(i => i.Calificacion.Nota));


        public int TotalInscriptos => MateriaCursada?.Inscripciones?.Count ?? 0;

        public int TotalActivos => InscriptosActivos.Count;

        public int TotalBajas => InscriptosDadosDeBaja.Count;

        public int TotalCalificados => InscriptosActivos.Count(i => i.Calificacion != null);
    }
}
