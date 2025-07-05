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
using Instituto.C.ViewModels;
using System.Security.Claims;

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
            var activos = await _context.Profesores
                .Where(p => p.Activo)
                .Include(p => p.MateriasCursada)
                    .ThenInclude(mc => mc.Materia)
                        .ThenInclude(m => m.Carrera) // mostramos tamien la carrera del profesor 
                .ToListAsync();

            return View(activos);
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

            //actualizamos los campos editables
            profesorDb.FechaAlta = profesor.FechaAlta;
            profesorDb.Nombre = profesor.Nombre;
            profesorDb.Apellido = profesor.Apellido;
            profesorDb.DNI = profesor.DNI;
            profesorDb.Telefono = profesor.Telefono;
            profesorDb.Direccion = profesor.Direccion;
            profesorDb.Activo = profesor.Activo;
            profesorDb.Legajo = profesor.Legajo;

            //actualizanos el Email y UserName correctamente
            profesorDb.UserName = profesor.UserName;
            profesorDb.Email = profesor.Email;

            var result = await _userManager.UpdateAsync(profesorDb);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(profesor);
            }

            try
            {
                await _context.SaveChangesAsync(); // por si hay otros cambios en el contexto
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                return Problem("Hubo un problema de concurrencia al intentar guardar los cambios.");
            }
        }



        [Authorize(Roles = "Admin")] //aunque no existe, potencialmente sí y nadie los puede borrar
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
        [Authorize(Roles = "Admin")] //aunque no existe, potencialmente sí y nadie los puede borrar
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var profesor = await _context.Profesores.FindAsync(id);
            if (profesor != null)
            {
                profesor.Activo = false;
                _context.Profesores.Update(profesor);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool ProfesorExists(int id)
        {
            return _context.Profesores.Any(e => e.Id == id);
        }


        [Authorize(Roles = "ProfesorRol")]
        public async Task<IActionResult> MisMaterias()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var profesor = await _context.Profesores.FirstOrDefaultAsync(p => p.Id == userId);

            if (profesor == null)
            {
                return NotFound("Profesor no encontrado");
            }

            var materias = await _context.MateriasCursadas
                .Include(mc => mc.Materia)
                .Include(mc => mc.Inscripciones)
                    .ThenInclude(i => i.Alumno)
                .Include(mc => mc.Inscripciones)
                    .ThenInclude(i => i.Calificacion)
                .Where(mc => mc.ProfesorId == profesor.Id)
                .ToListAsync();

            // aca filtramo inscripciones activas directamente en cada materia
            foreach (var materia in materias)
            {
                materia.Inscripciones = materia.Inscripciones
                    .Where(i => i.Activa)
                    .ToList();
            }

            var model = new MisMateriasViewModel
            {
                Vigentes = materias
                    .Where(mc => mc.EstaVigente())
                    .Select(mc => new MateriaCursadaConPromedioViewModel { MateriaCursada = mc })
                    .ToList(),

                Pasadas = materias
                    .Where(mc => !mc.EstaVigente())
                    .Select(mc => new MateriaCursadaConPromedioViewModel { MateriaCursada = mc })
                    .ToList()
            };

            return View(model);
        }

        [Authorize(Roles = "ProfesorRol")]
        public async Task<IActionResult> DetallesMateriaProfesor(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var profesor = await _context.Profesores.FirstOrDefaultAsync(p => p.Id == userId);

            if (profesor == null)
                return NotFound("Profesor no encontrado");

            var materia = await _context.MateriasCursadas
                .Include(mc => mc.Materia)
                .Include(mc => mc.Inscripciones)
                    .ThenInclude(i => i.Alumno)
                .Include(mc => mc.Inscripciones)
                    .ThenInclude(i => i.Calificacion)
                .FirstOrDefaultAsync(mc => mc.Id == id && mc.ProfesorId == profesor.Id);

            if (materia == null)
                return NotFound("Materia no encontrada o no pertenece al profesor");

            var model = new DetalleMateriaCursadaProfesorViewModel
            {
                MateriaCursada = materia
            };

            return View(model);
        }


    }
}
