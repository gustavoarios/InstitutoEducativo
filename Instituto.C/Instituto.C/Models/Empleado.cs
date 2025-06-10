using System;
using System.Runtime.CompilerServices;

namespace Instituto.C.Models
{
    public class Empleado : Persona
    {
        private string _legajo;

        public string Legajo
        {
            get
            {
                return _legajo;
            }
        }
        public Empleado()
        {
            _legajo = GenerarLegajo();
        }

        private string GenerarLegajo()
        {
            // Ejemplo básico: usar "EMP" + últimos 4 del DNI
            return this.DNI;
        }
    }
}
