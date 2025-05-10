using System.Runtime.CompilerServices;

namespace Instituto.C.Models
{
    public class Empleado : Persona
    {
        private static int _contadorLegajo = 0;
        public int Legajo { get { return _contadorLegajo; } set { } }
        public Empleado()
        {
            _contadorLegajo++;
            Legajo = _contadorLegajo;
        }
    }
}
