using Instituto.C.Data;
using Instituto.C.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instituto.C.Controllers
{
    public class MateriasController : Controller
    {
        private readonly InstitutoDb _context;
        private readonly UserManager<Persona> _userManager;


        public MateriasController(InstitutoDb context, UserManager<Persona> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: Materias
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("AlumnoRol"))
            {
                var user = await _userManager.GetUserAsync(User);
                var alumno = await _context.Alumnos
                    .Include(a => a.Carrera)
                    .FirstOrDefaultAsync(a => a.Id == user.Id);

                if (alumno != null)
                {
                    var materiasAlumno = await _context.Materias
                        .Include(m => m.Carrera)
                        .Where(m => m.CarreraId == alumno.CarreraId)
                        .ToListAsync();

                    return View(materiasAlumno);
                }
            }

            // Si no es alumno o no se encontró el alumno, mostrar todas
            var todasLasMaterias = await _context.Materias
                .Include(m => m.Carrera)
                .ToListAsync();

            return View(todasLasMaterias);
        }

        [Authorize(Roles = "EmpleadoRol")]
        // GET: Materias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materia = await _context.Materias
                .Include(m => m.Carrera)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materia == null)
            {
                return NotFound();
            }

            return View(materia);
        }

        // GET: Materias/Create
        [Authorize(Roles = "EmpleadoRol")]
        public IActionResult Create()
        {
            var carreras = _context.Carreras
    .Select(c => new {
        c.Id,
        Display = c.CodigoCarrera + " - " + c.Nombre
    })
    .ToList();

            ViewData["CarreraId"] = new SelectList(carreras, "Id", "Display");
            return View();
        }

        // POST: Materias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EmpleadoRol")]
        public async Task<IActionResult> Create([Bind("Id,Nombre,CodigoMateria,Descripcion,CupoMaximo,CarreraId")] Materia materia)
        {
            if (ModelState.IsValid)
            {
                bool materiaRepetida = await _context.Materias
                    .AnyAsync(m => m.Nombre == materia.Nombre && m.CarreraId == materia.CarreraId);

                if (materiaRepetida)
                {
                    ModelState.AddModelError("Nombre", "Ya existe una materia con ese nombre en esta carrera.");

                    var carreras = _context.Carreras
                        .Select(c => new { c.Id, Display = c.CodigoCarrera + " - " + c.Nombre })
                        .ToList();

                    ViewData["CarreraId"] = new SelectList(carreras, "Id", "Display", materia.CarreraId);
                    return View(materia);
                }

                _context.Add(materia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var allCarreras = _context.Carreras
                .Select(c => new { c.Id, Display = c.CodigoCarrera + " - " + c.Nombre })
                .ToList();

            ViewData["CarreraId"] = new SelectList(allCarreras, "Id", "Display", materia.CarreraId);
            return View(materia);
        }


        // GET: Materias/Edit/5
        [Authorize(Roles = "EmpleadoRol")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materia = await _context.Materias.FindAsync(id);
            if (materia == null)
            {
                return NotFound();
            }
            ViewData["CarreraId"] = new SelectList(_context.Carreras, "Id", "CodigoCarrera", materia.CarreraId);
            return View(materia);
        }

        // POST: Materias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EmpleadoRol")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,CodigoMateria,Descripcion,CupoMaximo,CarreraId")] Materia materia)
        {
            if (id != materia.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // validamoos para evitar duplicado al editar (excluyendo el actual)
                bool materiaRepetida = await _context.Materias
                    .AnyAsync(m => m.Id != materia.Id && m.Nombre == materia.Nombre && m.CarreraId == materia.CarreraId);

                if (materiaRepetida)
                {
                    ModelState.AddModelError("Nombre", "Ya existe una materia con ese nombre en esta carrera.");
                    ViewData["CarreraId"] = new SelectList(_context.Carreras, "Id", "CodigoCarrera", materia.CarreraId);
                    return View(materia);
                }

                try
                {
                    _context.Update(materia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MateriaExists(materia.Id))
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

            ViewData["CarreraId"] = new SelectList(_context.Carreras, "Id", "CodigoCarrera", materia.CarreraId);
            return View(materia);
        }

        // GET: Materias/Delete/5
        
        [Authorize(Roles = "Admin")] //aunque no existe, potencialmente sí y nadie los puede borrar
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materia = await _context.Materias
                .Include(m => m.Carrera)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materia == null)
            {
                return NotFound();
            }

            return View(materia);
        }

        // POST: Materias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] //aunque no existe, potencialmente sí y nadie los puede borrar
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var materia = await _context.Materias.FindAsync(id);
            if (materia != null)
            {
                _context.Materias.Remove(materia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MateriaExists(int id)
        {
            return _context.Materias.Any(e => e.Id == id);
        }
    }
}
