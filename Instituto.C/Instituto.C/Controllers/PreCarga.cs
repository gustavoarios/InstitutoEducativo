using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Instituto.C.Data;
using Instituto.C.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Instituto.C.Controllers
{
    public class PreCarga : Controller
    {
        private readonly UserManager<Persona> _userManager;
        private readonly RoleManager<Rol> _roleManager;
        private readonly InstitutoDb _context;  

        private List<string> roles = new List<string> { "AlumnoRol", "EmpleadoRol", "ProfesorRol" };

        public PreCarga(UserManager<Persona> userManager, RoleManager<Rol> roleManager, InstitutoDb context)
        {

            this._userManager = userManager;
            this._roleManager = roleManager;
            this._context = context;
           
        }

        public async Task<IActionResult> SeedAsync()
        {

            await CrearRoles();

            await SeedData.InitializeAsync(_context, _userManager);
           /* CrearEmpleados().Wait();
            CrearProfesores().Wait();
            CrearAlumnos().Wait();*/

            return RedirectToAction("Index", "Home", new {mensaje="proceso seed finalizado"});
        }
        //private async Task CrearEmpleados()
        //{
        //    throw new NotImplementedException();
        //}

        //private async Task CrearProfesores()
        //{
        //    throw new NotImplementedException();
        //}

        //private async Task CrearAlumnos()
        //{
        //    throw new NotImplementedException();
        //}

        

       

        private async Task CrearRoles()
        {

            foreach(var rolName in roles)
            {
                if (!await _roleManager.RoleExistsAsync(rolName))
                {
                    await _roleManager.CreateAsync(new Rol(rolName));
                }
            }

        }

    }
}
