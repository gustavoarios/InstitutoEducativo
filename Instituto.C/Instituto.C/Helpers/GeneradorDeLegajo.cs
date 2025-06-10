using Instituto.C.Models;
using System;

namespace Instituto.C.Helpers
{
    public class GeneradorDeLegajo
    {
        public static string GenerarLegajoParaProfesor(Profesor profesor)
        {
            // Formato: PROF-2025-12345678 (usando DNI)
            return $"PROF-{DateTime.Now.Year}-{profesor.DNI}";
        }
    }
}
