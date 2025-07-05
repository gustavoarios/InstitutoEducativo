using Instituto.C.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instituto.C.Helpers
{
    public static class RoleSeeder
    {
        public static async Task CrearRolesAsync(RoleManager<Rol> roleManager)
        {
            var roles = new List<string> { "AlumnoRol", "ProfesorRol", "EmpleadoRol" };

            foreach (var rolName in roles)
            {
                if (!await roleManager.RoleExistsAsync(rolName))
                {
                    await roleManager.CreateAsync(new Rol(rolName));
                }
            }
        }
    }
}
