using Instituto.C.Models;
using System.Collections.Generic;
using System.Linq;

namespace Instituto.C.Helpers
{
    public class NotasHelper
    {
        public static string MostrarNota(Nota nota)
        {
            return nota switch
            {
                Nota.Ausente => "A",
                Nota.Baja => "B",
                Nota.Pendiente => "P",
                _ => ((int)nota).ToString()
            };
        }

        public static bool EsNotaValidaParaPromedio(Nota nota)
        {
            return (int)nota >= 0 && (int)nota <= 10;
        }

        public static double? ObtenerValorNumerico(Nota nota)
        {
            return EsNotaValidaParaPromedio(nota) ? (double)nota : null;
        }

        public static double CalcularPromedio(IEnumerable<Nota> notas)
        {
            var notasNumericas = notas
                .Select(n => NotasHelper.ObtenerValorNumerico(n))
                .Where(v => v.HasValue)
                .Select(v => v.Value);

            return notasNumericas.Any() ? notasNumericas.Average() : 0.0;
        }
    }
}
