using System.Runtime.CompilerServices;

namespace Instituto.C.Models
{
    public abstract class Empleado : Persona
    {
        public int _contadorLegajo = 0;
        public int Legajo { get { return GetLegajo(); } set { } }
        public Empleado()
        {
            _contadorLegajo++;
            Legajo = _contadorLegajo;
        }

        public int GetLegajo()
        {
            return Legajo;
        }
    }
}
