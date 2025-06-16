using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Instituto.C.Data;
using Instituto.C.Models;
using Microsoft.AspNetCore.Authorization;
using Instituto.C.Helpers;
using Microsoft.AspNetCore.Identity;

namespace Instituto.C.Controllers
{
    public class ProfesoresController : Controller
    {
        private readonly InstitutoDb _context;
        private readonly UserManager<Persona> _userManager;

        public ProfesoresController(InstitutoDb context, UserManager<Persona> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Profesores.ToListAsync());
        }

        [Authorize(Roles = "EmpleadoRol")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profesor = await _context.Profesores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (profesor == null)
            {
                return NotFound();
            }

            return View(profesor);
        }

        [Authorize(Roles = "EmpleadoRol")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EmpleadoRol")]
        public async Task<IActionResult> Create([Bind("Legajo,Id,UserName,Email,FechaAlta,Nombre,Apellido,DNI,Telefono,Direccion,Activo")] Profesor profesor)
        {
            if (ModelState.IsValid)
            {
                profesor.Legajo = GeneradorDeLegajo.GenerarLegajoParaProfesor(profesor);
                profesor.EmailConfirmed = true;

                var resultado = await _userManager.CreateAsync(profesor, "Password1!"); // contraseñas temporales o definidas
                if (resultado.Succeeded)
                {
                    await _userManager.AddToRoleAsync(profesor, "ProfesorRol");
                    return RedirectToAction(nameof(Index));
                }

                // Si hay errores, los mostramos
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(profesor);
        }

        [Authorize(Roles = "EmpleadoRol")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profesor = await _context.Profesores.FindAsync(id);
            if (profesor == null)
            {
                return NotFound();
            }
            return View(profesor);
        }

        /*[HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EmpleadoRol")]
        public async Task<IActionResult> Edit(int id, [Bind("Legajo,Id,UserName,Email,FechaAlta,Nombre,Apellido,DNI,Telefono,Direccion,Activo")] Profesor profesor)
        {
            if (id != profesor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(profesor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfesorExists(profesor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(profesor);
        }*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EmpleadoRol")]
        public async Task<IActionResult> Edit(int id, [Bind("Legajo,Id,UserName,Email,FechaAlta,Nombre,Apellido,DNI,Telefono,Direccion,Activo")] Profesor profesor)
        {
            if (id != profesor.Id)
            {
                return NotFound();
            }

            var profesorDb = await _context.Profesores.FindAsync(id);
            if (profesorDb == null)
            {
                return NotFound();
            }

            // Solo campos que se pueden editar
            profesorDb.UserName = profesor.UserName;
            profesorDb.Email = profesor.Email;
            profesorDb.FechaAlta = profesor.FechaAlta;
            profesorDb.Nombre = profesor.Nombre;
            profesorDb.Apellido = profesor.Apellido;
            profesorDb.DNI = profesor.DNI;
            profesorDb.Telefono = profesor.Telefono;
            profesorDb.Direccion = profesor.Direccion;
            profesorDb.Activo = profesor.Activo;
            profesorDb.Legajo = profesor.Legajo;

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                return Problem("Hubo un problema de concurrencia al intentar guardar los cambios.");
            }
        }


        [Authorize(Roles = "EmpleadoRol")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profesor = await _context.Profesores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (profesor == null)
            {
                return NotFound();
            }

            return View(profesor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EmpleadoRol")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var profesor = await _context.Profesores.FindAsync(id);
            if (profesor != null)
            {
                _context.Profesores.Remove(profesor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProfesorExists(int id)
        {
            return _context.Profesores.Any(e => e.Id == id);
        }
    }
}
