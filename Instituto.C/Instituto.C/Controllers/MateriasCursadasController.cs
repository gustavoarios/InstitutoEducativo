using Instituto.C.Data;
using Instituto.C.Helpers;
using Instituto.C.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instituto.C.Controllers
{
    public class MateriasCursadasController : Controller
    {
        private readonly InstitutoDb _context;

        public MateriasCursadasController(InstitutoDb context)
        {
            _context = context;
        }

        // GET: MateriasCursadas
        public async Task<IActionResult> Index()
        {
            var institutoDb = _context.MateriasCursadas.Include(m => m.Materia).Include(m => m.Profesor);
            return View(await institutoDb.ToListAsync());
        }

        // GET: MateriasCursadas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materiaCursada = await _context.MateriasCursadas
                .Include(m => m.Materia)
                .Include(m => m.Profesor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materiaCursada == null)
            {
                return NotFound();
            }

            return View(materiaCursada);
        }

        // GET: MateriasCursadas/Create
        [Authorize(Roles = "EmpleadoRol")]
        public IActionResult Create()
        {
            ViewData["MateriaId"] = new SelectList(_context.Materias, "Id", "CodigoMateria");
            ViewData["ProfesorId"] = new SelectList(_context.Profesores, "Id", "Apellido");
            return View();
        }

        // POST: MateriasCursadas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EmpleadoRol")]
        public async Task<IActionResult> Create([Bind("Id,Anio,Cuatrimestre,Activo,MateriaId,ProfesorId")] MateriaCursada materiaCursada)
        {
            // Asignar CodigoCursada automáticamente
            var ultimaCursada = _context.MateriasCursadas
                .Where(mc => mc.Anio == materiaCursada.Anio &&
                             mc.Cuatrimestre == materiaCursada.Cuatrimestre &&
                             mc.MateriaId == materiaCursada.MateriaId)
                .OrderByDescending(mc => mc.CodigoCursada)
                .FirstOrDefault();

            materiaCursada.CodigoCursada = MateriasHelper.ObtenerSiguienteCodigoCursada(ultimaCursada?.CodigoCursada);

            // Traer la materia para calcular el nombre (necesita CodigoMateria)
            materiaCursada.Materia = await _context.Materias.FindAsync(materiaCursada.MateriaId);

            // Generar nombre
            var nombreGenerado = MateriasHelper.GenerarNombreCursada(materiaCursada);

            // Validación de duplicados
            bool existeDuplicado = _context.MateriasCursadas.Any(mc => mc.Nombre == nombreGenerado);
            if (existeDuplicado)
            {
                ModelState.AddModelError("", "Ya existe una cursada con ese nombre.");
            }

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(materiaCursada.CodigoCursada))
                {
                    materiaCursada.CodigoCursada = "A";
                }

                materiaCursada.Nombre = MateriasHelper.GenerarNombreCursada(materiaCursada);

                _context.Add(materiaCursada);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }


            ViewData["MateriaId"] = new SelectList(_context.Materias, "Id", "CodigoMateria", materiaCursada.MateriaId);
            ViewData["ProfesorId"] = new SelectList(_context.Profesores, "Id", "Apellido", materiaCursada.ProfesorId);
            // Si hay un error, volvemos a mostrar la vista con los datos ingresados
            return View(materiaCursada);
        }


        // GET: MateriasCursadas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materiaCursada = await _context.MateriasCursadas.FindAsync(id);
            if (materiaCursada == null)
            {
                return NotFound();
            }
            ViewData["MateriaId"] = new SelectList(_context.Materias, "Id", "CodigoMateria", materiaCursada.MateriaId);
            ViewData["ProfesorId"] = new SelectList(_context.Profesores, "Id", "Apellido", materiaCursada.ProfesorId);
            return View(materiaCursada);
        }

        // POST: MateriasCursadas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CodigoCursada,Anio,Cuatrimestre,Activo,MateriaId,ProfesorId")] MateriaCursada materiaCursada)
        {
            if (id != materiaCursada.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(materiaCursada);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MateriaCursadaExists(materiaCursada.Id))
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
            ViewData["MateriaId"] = new SelectList(_context.Materias, "Id", "CodigoMateria", materiaCursada.MateriaId);
            ViewData["ProfesorId"] = new SelectList(_context.Profesores, "Id", "Apellido", materiaCursada.ProfesorId);
            return View(materiaCursada);
        }

        // GET: MateriasCursadas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materiaCursada = await _context.MateriasCursadas
                .Include(m => m.Materia)
                .Include(m => m.Profesor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materiaCursada == null)
            {
                return NotFound();
            }

            return View(materiaCursada);
        }

        // POST: MateriasCursadas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var materiaCursada = await _context.MateriasCursadas.FindAsync(id);
            if (materiaCursada != null)
            {
                var tieneInscripciones = await _context.Inscripciones.AnyAsync(i => i.MateriaCursadaId == materiaCursada.Id);
                if (tieneInscripciones)
                {
                    TempData["Error"] = "No podés eliminar una cursada que tiene alumnos inscriptos.";
                    return RedirectToAction(nameof(Index));
                }

                _context.MateriasCursadas.Remove(materiaCursada);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MateriaCursadaExists(int id)
        {
            return _context.MateriasCursadas.Any(e => e.Id == id);
        }

        [Authorize(Roles = "ProfesorRol")]
        public async Task<IActionResult> CursadasVigentes()
        {
            var profe = await _context.Profesores.FirstOrDefaultAsync(p => p.UserName == User.Identity.Name);

            var vigentes = await _context.MateriasCursadas
                .Include(mc => mc.Materia)
                .Where(mc => mc.ProfesorId == profe.Id && mc.Activo)
                .ToListAsync();

            return View(vigentes);
        }

        [Authorize(Roles = "ProfesorRol")]
        public async Task<IActionResult> CursadasPasadas()
        {
            var profe = await _context.Profesores.FirstOrDefaultAsync(p => p.UserName == User.Identity.Name);

            var pasadas = await _context.MateriasCursadas
                .Include(mc => mc.Materia)
                .Where(mc => mc.ProfesorId == profe.Id && !mc.Activo)
                .ToListAsync();

            return View(pasadas);
        }


    }
}
