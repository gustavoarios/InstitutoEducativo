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

    [Authorize(Roles = "EmpleadoRol")]
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
        [Authorize(Roles = "EmpleadoRol")]

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
            ViewData["MateriaId"] = new SelectList(_context.Materias, "Id", "Nombre");
            ViewData["ProfesorId"] = new SelectList(_context.Profesores, "Id", "NombreCompleto");
            return View();
        }

        // POST: MateriasCursadas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EmpleadoRol")]
        public async Task<IActionResult> Create([Bind("Id,Anio,Cuatrimestre,Activo,MateriaId,ProfesorId")] MateriaCursada materiaCursada)
        {
            var ultimaCursada = await _context.MateriasCursadas
                .Where(mc => mc.Anio == materiaCursada.Anio &&
                             mc.Cuatrimestre == materiaCursada.Cuatrimestre &&
                             mc.MateriaId == materiaCursada.MateriaId)
                .OrderByDescending(mc => mc.CodigoCursada)
                .FirstOrDefaultAsync();

            materiaCursada.CodigoCursada = MateriasHelper.ObtenerSiguienteCodigoCursada(ultimaCursada?.CodigoCursada);
            materiaCursada.Materia = await _context.Materias.FindAsync(materiaCursada.MateriaId);
            materiaCursada.Nombre = MateriasHelper.GenerarNombreCursada(materiaCursada);

            // Validación de duplicado
            bool existeDuplicado = await _context.MateriasCursadas
                .AnyAsync(mc => mc.Nombre == materiaCursada.Nombre);
            if (existeDuplicado)
            {
                ModelState.AddModelError("Nombre", "Ya existe una cursada con ese nombre.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(materiaCursada);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MateriaId"] = new SelectList(_context.Materias, "Id", "Nombre", materiaCursada.MateriaId);
            ViewData["ProfesorId"] = new SelectList(_context.Profesores.Where(p => p.Activo), "Id", "NombreCompleto", materiaCursada.ProfesorId);
            return View(materiaCursada);
        }




        // GET: MateriasCursadas/Edit/5
        [Authorize(Roles = "EmpleadoRol")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var materiaCursada = await _context.MateriasCursadas
                .Include(mc => mc.Materia)
                .Include(mc => mc.Profesor)
                .FirstOrDefaultAsync(mc => mc.Id == id);

            if (materiaCursada == null)
                return NotFound();

            ViewData["MateriaId"] = new SelectList(_context.Materias, "Id", "Nombre", materiaCursada.MateriaId);
            ViewData["ProfesorId"] = new SelectList(_context.Profesores, "Id", "NombreCompleto", materiaCursada.ProfesorId);

            return View(materiaCursada);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EmpleadoRol")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProfesorId,Activo")] MateriaCursada datosEditados)
        {
            if (id != datosEditados.Id)
                return NotFound();

            var materiaCursada = await _context.MateriasCursadas
                .Include(mc => mc.Materia)
                .Include(mc => mc.Profesor)
                .FirstOrDefaultAsync(mc => mc.Id == id);

            if (materiaCursada == null)
                return NotFound();

            // Solo se permiten modificar estos campos
            materiaCursada.ProfesorId = datosEditados.ProfesorId;
            materiaCursada.Activo = datosEditados.Activo;

            // Actualizar el nombre por si se ve afectado
            materiaCursada.Nombre = MateriasHelper.GenerarNombreCursada(materiaCursada);

            // Validar si existe otra cursada con el mismo nombre
            bool duplicado = await _context.MateriasCursadas
                .AnyAsync(mc => mc.Nombre == materiaCursada.Nombre && mc.Id != materiaCursada.Id);

            if (duplicado)
            {
                ModelState.AddModelError("Nombre", "Ya existe otra cursada con ese nombre.");
            }

            ModelState.Remove("Anio");
            ModelState.Remove("Cuatrimestre");
            ModelState.Remove("MateriaId");
            ModelState.Remove("CodigoCursada");


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(materiaCursada);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.MateriasCursadas.Any(mc => mc.Id == materiaCursada.Id))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewData["ProfesorId"] = new SelectList(_context.Profesores.Where(p => p.Activo), "Id", "NombreCompleto", materiaCursada.ProfesorId);
            return View(materiaCursada);
        }





        // GET: MateriasCursadas/Delete/5
        [Authorize(Roles = "EmpleadoRol")]
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
        [Authorize(Roles = "EmpleadoRol")]
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
